using EarTrumpet.Interop;
using EarTrumpet.UI.ViewModels;
using System;

namespace EarTrumpet.Shared
{
    class DeviceIconhelper
    {
        public static SndVolSSO.IconId GetSndVolIcon(DeviceViewModel device)
        {
            if (device != null)
            {
                switch (device.IconKind)
                {
                    case DeviceViewModel.DeviceIconKind.Mute:
                        return SndVolSSO.IconId.Muted;
                    case DeviceViewModel.DeviceIconKind.Bar1:
                        return SndVolSSO.IconId.SpeakerOneBar;
                    case DeviceViewModel.DeviceIconKind.Bar2:
                        return SndVolSSO.IconId.SpeakerTwoBars;
                    case DeviceViewModel.DeviceIconKind.Bar3:
                        return SndVolSSO.IconId.SpeakerThreeBars;
                    default: throw new NotImplementedException(device.IconKind.ToString());
                }
            }
            return SndVolSSO.IconId.NoDevice;
        }
    }
}
