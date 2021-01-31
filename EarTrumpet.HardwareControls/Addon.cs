using EarTrumpet.DataModel.Storage;
using EarTrumpet.Extensibility;
using System;
using System.ComponentModel.Composition;

namespace EarTrumpet.HardwareControls
{
    [Export(typeof(IAddonLifecycle))]
    class Addon : IAddonLifecycle
    {
        public static string Namespace => "EarTrumpet.HardwareControls";

        public AddonInfo Info
        {
            get => new AddonInfo
            {
                DisplayName = "Simple",
                PublisherName = "File-New-Project",
                Id = Namespace,
                HelpLink = "https://github.com/File-New-Project/EarTrumpet",
                AddonVersion = new Version(1, 0, 0, 0),
            };
        }

        public static Addon Current { get; private set; }
        public ISettingsBag Settings { get; private set; }

        public void OnApplicationLifecycleEvent(ApplicationLifecycleEvent evt)
        {
            if (evt == ApplicationLifecycleEvent.Startup)
            {
                Current = this;
                Settings = StorageFactory.GetSettings(Namespace);

                // Do startup work here.
            }
        }
    }
}
