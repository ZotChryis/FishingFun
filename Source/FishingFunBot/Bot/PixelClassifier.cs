using FishingFun.Configuration;
using log4net;
using System;

namespace FishingFun
{
    public partial class PixelClassifier : IPixelClassifier
    {
        private static ILog logger = LogManager.GetLogger("Fishbot");

        public double ColourMultiplier { get; set; }
        public double ColourClosenessMultiplier { get; set; }
        public ClassifierMode Mode { get; set; }

        public PixelClassifier()
        {
            LoadFromConfiguration();
        }

        public void LoadFromConfiguration()
        {
            var config = ConfigurationManager.Instance.Current.Color;
            this.Mode = config.Mode;
            this.ColourMultiplier = config.ColourMultiplier;
            this.ColourClosenessMultiplier = config.ColourClosenessMultiplier;

            if (config.IsWowClassic)
            {
                logger.Info("Wow Classic configuration");
                this.ColourMultiplier = Constants.ClassicColourMultiplier;
                this.ColourClosenessMultiplier = Constants.ClassicColourClosenessMultiplier;
            }
            else
            {
                logger.Info("Wow Standard configuration");
            }
        }

        public bool IsMatch(byte red, byte green, byte blue)
        {
            if (Mode == ClassifierMode.Red)
            {
                return isBigger(red, green) && isBigger(red, blue) && areClose(blue, green);
            }
            else
            {
                return isBigger(blue, green) && isBigger(blue, red) && areClose(red, green);
            }
        }

        public void SetConfiguration(bool isWowClassic)
        {
            var config = ConfigurationManager.Instance.Current;
            config.Color.IsWowClassic = isWowClassic;
            LoadFromConfiguration();
            ConfigurationManager.Instance.Save();
        }

        private bool isBigger(byte red, byte other)
        {
            return (red * ColourMultiplier) > other;
        }

        private bool areClose(byte color1, byte color2)
        {
            var max = Math.Max(color1, color2);
            var min = Math.Min(color1, color2);

            return min * ColourClosenessMultiplier > max - 20;
        }
    }
}