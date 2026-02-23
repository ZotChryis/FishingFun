using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

#nullable enable
namespace FishingFun
{
    public class SearchBobberFinder : IBobberFinder, IImageProvider
    {
        private readonly IPixelClassifier pixelClassifier;

        private static ILog logger = LogManager.GetLogger("Fishbot");

        private Point previousLocation;

        public event EventHandler<BobberBitmapEvent> BitmapEvent;

        public SearchBobberFinder(IPixelClassifier pixelClassifier)
        {
            this.pixelClassifier = pixelClassifier;
            BitmapEvent += (s, e) => { };
        }

        public void Reset()
        {
            this.previousLocation = Point.Empty;
        }

        public Point Find(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Point.Empty;
            }

            using (var bitmap = WowScreen.GetBitmap())
            {
                Score? best = Score.ScorePoints(FindRedPoints(bitmap, cancellationToken));

                if (previousLocation != Point.Empty && best == null)
                {
                    previousLocation = Point.Empty;
                    best = Score.ScorePoints(FindRedPoints(bitmap, cancellationToken));
                }

                previousLocation = Point.Empty;
                if (best != null)
                {
                    previousLocation = best.point;
                }

                // Clone bitmap for event handlers - they are responsible for disposing it
                var bitmapClone = (Bitmap)bitmap.Clone();
                BitmapEvent?.Invoke(this, new BobberBitmapEvent { Point = new Point(previousLocation.X, previousLocation.Y), Bitmap = bitmapClone });

                return previousLocation == Point.Empty ? Point.Empty : WowScreen.GetScreenPositionFromBitmapPostion(previousLocation);
            }
        }

        private List<Score> FindRedPoints(Bitmap bitmap, CancellationToken cancellationToken)
        {
            var points = new List<Score>();

            var hasPreviousLocation = previousLocation != Point.Empty;

            // search around last found location
            var minX = Math.Max(hasPreviousLocation ? previousLocation.X - 40 : 0, 0);
            var maxX = Math.Min(hasPreviousLocation ? previousLocation.X + 40 : bitmap.Width, bitmap.Width);
            var minY = Math.Max(hasPreviousLocation ? previousLocation.Y - 40 : 0, 0);
            var maxY = Math.Min(hasPreviousLocation ? previousLocation.Y + 40 : bitmap.Height, bitmap.Height);

            //System.Diagnostics.Debug.WriteLine($"Search from X {minX}-{maxX}, Y {minY}-{maxY}");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = minX; x < maxX && !cancellationToken.IsCancellationRequested; x++)
            {
                for (int y = minY; y < maxY && !cancellationToken.IsCancellationRequested; y++)
                {
                    ProcessPixel(bitmap, points, x, y);
                }
            }
            sw.Stop();

            if (sw.ElapsedMilliseconds > 200)
            {
                var prevText = hasPreviousLocation ? " using previous location" : "";
                Debug.WriteLine($"Feather points found: {points.Count} in {sw.ElapsedMilliseconds}{prevText}.");
            }

            if (points.Count>1000)
            {
                logger.Error("Error: Too much of the feather colour in this image, please adjust the colour configuration !");
                points.Clear();
            }

            return points;
        }

        private void ProcessPixel(Bitmap bitmap, List<Score> points, int x, int y)
        {
            var p = bitmap.GetPixel(x, y);

            bool isMatch = this.pixelClassifier.IsMatch(p.R, p.G, p.B);

            if (isMatch)
            {
                points.Add(new Score { point = new Point(x, y) });
                bitmap.SetPixel(x, y, this.pixelClassifier.Mode == PixelClassifier.ClassifierMode.Blue ? Color.Blue : Color.Red);
            }
        }

        private class Score
        {
            public Point point;
            public int count = 0;

            public static Score? ScorePoints(List<Score> points)
            {
                foreach (Score p in points)
                {
                    p.count = points.Where(s => Math.Abs(s.point.X - p.point.X) < 10) // + or - 10 pixels horizontally
                        .Where(s => Math.Abs(s.point.Y - p.point.Y) < 10) // + or - 10 pixels vertically
                        .Count();
                }

                var best = points.OrderByDescending(s => s.count).FirstOrDefault();

                if (best != null)
                {
                    //System.Diagnostics.Debug.WriteLine($"best score: {best.count} at {best.point.X},{best.point.Y}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No red found");
                }

                return best;
            }
        }
    }
}