using System;
using System.Drawing;
using System.Threading;

namespace FishingFun
{
    public class BobberColourPointFinder : IBobberFinder, IImageProvider
    {
        private Color targetColor;

        public BobberColourPointFinder(Color targetColor)
        {
            this.targetColor = targetColor;
            BitmapEvent += (s, e) => { };
        }

        public event EventHandler<BobberBitmapEvent> BitmapEvent;

        public Point Find(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Point.Empty;
            }

            using (var bmp = WowScreen.GetBitmap())
            {
                const int targetOffset = 15;

                var widthLower = 0;
                var widthHigher = bmp.Width;
                var heightLower = 0;
                var heightHigher = bmp.Height;

                var targetRedLb = targetColor.R - targetOffset;
                var targetRedHb = targetColor.R + targetOffset;
                var targetBlueLb = targetColor.B - targetOffset;
                var targetBlueHb = targetColor.B + targetOffset;
                var targetGreenLb = targetColor.G - targetOffset;
                var targetGreenHb = targetColor.G + targetOffset;

                var pos = new Point(0, 0);

                for (int i = widthLower; i < widthHigher && !cancellationToken.IsCancellationRequested; i++)
                {
                    for (int j = heightLower; j < heightHigher && !cancellationToken.IsCancellationRequested; j++)
                    {
                        pos.X = i;
                        pos.Y = j;
                        var colorAt = WowScreen.GetColorAt(pos, bmp);
                        if (colorAt.R > targetRedLb &&
                            colorAt.R < targetRedHb &&
                            colorAt.B > targetBlueLb &&
                            colorAt.B < targetBlueHb &&
                            colorAt.G > targetGreenLb &&
                            colorAt.G < targetGreenHb)
                        {
                            // Clone bitmap for event handlers - they are responsible for disposing it
                            var bitmapClone = (Bitmap)bmp.Clone();
                            BitmapEvent?.Invoke(this, new BobberBitmapEvent { Point = new Point(i, j), Bitmap = bitmapClone });
                            return WowScreen.GetScreenPositionFromBitmapPostion(pos);
                        }
                    }
                }

                // Clone bitmap for event handlers - they are responsible for disposing it
                var bitmapCloneEmpty = (Bitmap)bmp.Clone();
                BitmapEvent?.Invoke(this, new BobberBitmapEvent { Point = Point.Empty, Bitmap = bitmapCloneEmpty });
                return Point.Empty;
            }
        }

        public Bitmap GetBitmap()
        {
            // This method is part of IImageProvider but should not be used with the new pattern
            // Return empty bitmap to avoid breaking existing code
            return new Bitmap(1, 1);
        }

        public void Reset()
        {
        }
    }
}