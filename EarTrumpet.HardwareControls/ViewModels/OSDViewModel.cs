using EarTrumpet.UI.ViewModels;

namespace EarTrumpet.HardwareControls.ViewModels
{
    public class OSDViewModel : SettingsPageViewModel
    {
        public bool EnableOverlay
        {
            get => Addon.Current.Settings.Get("EnableOverlay", true);
            set => Addon.Current.Settings.Set("EnableOverlay", value);
        }

        public bool TopLeft
        {
            get => Addon.Current.Settings.Get("OverlayPosition", "TopLeft") == "TopLeft";
            set
            {
                if (value)
                {
                    Addon.Current.Settings.Set("OverlayPosition", "TopLeft");
                }
            }
        }

        public bool TopRight { 
            get => Addon.Current.Settings.Get("OverlayPosition", "TopLeft") == "TopRight";
            set
            {
                if (value)
                {
                    Addon.Current.Settings.Set("OverlayPosition", "TopRight");
                }
            }
        }
        public bool Center
        {
            get => Addon.Current.Settings.Get("OverlayPosition", "TopLeft") == "Center";
            set
            {
                if (value)
                {
                    Addon.Current.Settings.Set("OverlayPosition", "Center");
                }
            }
        }

        public bool BottomLeft
        {
            get => Addon.Current.Settings.Get("OverlayPosition", "TopLeft") == "BottomLeft";
            set
            {
                if (value)
                {
                    Addon.Current.Settings.Set("OverlayPosition", "BottomLeft");
                }
            }
        }

        public bool BottomRight
        {
            get => Addon.Current.Settings.Get("OverlayPosition", "TopLeft") == "BottomRight";
            set
            {
                if (value)
                {
                    Addon.Current.Settings.Set("OverlayPosition", "BottomRight");
                }
            }
        }
        
        public int OffsetX { 
            get => Addon.Current.Settings.Get("OffsetX", 64);
            set => Addon.Current.Settings.Set("OffsetX", value); 
        }
        public int OffsetY { 
            get => Addon.Current.Settings.Get("OffsetY", 64);
            set => Addon.Current.Settings.Set("OffsetY", value);
        }
        
        public int DisplayTime { 
            get => Addon.Current.Settings.Get("DisplayTime", 2);
            set => Addon.Current.Settings.Set("DisplayTime", value);
        }

        public OSDViewModel() : base(null)
        {
            // Todo glyph & localization
            Glyph = "\uE75A";
            Title = Properties.Resources.OverlayTitle;
            EnableOverlay = true;
        }
    }
}