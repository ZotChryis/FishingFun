using FishingFun.Configuration;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FishingFun
{
    public class Macro1KeyBindChooser : KeyBindChooser {
        public Macro1KeyBindChooser() : base()
        {
            StorageIndex = 1;
            ReadConfiguration();
        }
    }

    public class Macro2KeyBindChooser : KeyBindChooser
    {
        public Macro2KeyBindChooser() : base()
        {
            StorageIndex = 2;
            ReadConfiguration();
        }
    }

    public partial class KeyBindChooser : UserControl
    {
        protected int StorageIndex { get; set; } = 0;

        public ConsoleKey CastKey { get; set; } = ConsoleKey.D4;

        public EventHandler CastKeyChanged;

        public KeyBindChooser()
        {
            CastKeyChanged += (s, e) => { };

            InitializeComponent();
            ReadConfiguration();
        }

        protected void ReadConfiguration()
        {
            try
            {
                var config = ConfigurationManager.Instance.Current;

                switch (StorageIndex)
                {
                    case 0:
                        CastKey = config.KeyBinds.CastKey;
                        break;
                    case 1:
                        CastKey = config.KeyBinds.Macro1Key;
                        break;
                    case 2:
                        CastKey = config.KeyBinds.Macro2Key;
                        break;
                }

                KeyBind.Text = GetCastKeyText(this.CastKey);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                CastKey = ConsoleKey.D4;
                KeyBind.Text = GetCastKeyText(this.CastKey);
            }
        }

        private void WriteConfiguration()
        {
            var config = ConfigurationManager.Instance.Current;

            switch (StorageIndex)
            {
                case 0:
                    config.KeyBinds.CastKey = CastKey;
                    break;
                case 1:
                    config.KeyBinds.Macro1Key = CastKey;
                    break;
                case 2:
                    config.KeyBinds.Macro2Key = CastKey;
                    break;
            }

            ConfigurationManager.Instance.Save();
        }

        private void CastKey_Focus(object sender, RoutedEventArgs e)
        {
            KeyBind.Text = "";
        }

        private void KeyBind_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var key = e.Key.ToString();
            ProcessKeybindText(key);
        }

        private void ProcessKeybindText(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                ConsoleKey ck;
                if (Enum.TryParse<ConsoleKey>(key, out ck))
                {
                    // Read first so we pick up changes from all 3 classes that share this file. Maybe a shared storage manager
                    // would be best, but this is quick and dirty to allow multiple keybinds.
                    ReadConfiguration();
                    this.CastKey = ck;
                    KeyBind.Text = GetCastKeyText(this.CastKey);
                    WriteConfiguration();
                    CastKeyChanged?.Invoke(this, null);
                    return;
                }
            }
            KeyBind.Text = "";
        }


        private string GetCastKeyText(ConsoleKey ck)
        {
            string keyText = ck.ToString();
            if (keyText.Length == 1) { return keyText; }
            if (keyText.StartsWith("D") && keyText.Length == 2)
            {
                return keyText.Substring(1, 1);
            }
            return "?";
        }
    }
}