using EarTrumpet.Extensibility;
using EarTrumpet.HardwareControls.ViewModels;
using EarTrumpet.Extensibility.Shared;
using EarTrumpet.UI.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace EarTrumpet.HardwareControls
{
    [Export(typeof(IAddonSettingsPage))]
    class SettingsPageAddon : IAddonSettingsPage
    {
        public SettingsCategoryViewModel Get(AddonInfo info)
        {
            ResourceLoader.Load(Addon.Namespace);
            return new SettingsCategoryViewModel(
                "Hardware Support",
                "\xE9A1", 
                "MIDI and other hardware devices", 
                Addon.Current.Info.Id, new List<SettingsPageViewModel> {
                new EarTrumpetHardwareControlsPageViewModel(),
                new AddonAboutPageViewModel(info),
            });
        }
    }
}
