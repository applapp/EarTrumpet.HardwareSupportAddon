using EarTrumpet.DataModel.Storage;
using EarTrumpet.DataModel.WindowsAudio;
using EarTrumpet.Extensibility;
using EarTrumpet.HardwareControls.Interop.Hardware;
using EarTrumpet.Shared.Extensions;
using EarTrumpet.UI.ViewModels;
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
                DisplayName = "Hardware Controls",
                PublisherName = "svenwml-and-skief",
                Id = Namespace,
                HelpLink = "https://github.com/File-New-Project/EarTrumpet/pull/624",
                AddonVersion = new Version(1, 0, 0, 0),
            };
        }

        public static Addon Current { get; private set; }
        public ISettingsBag Settings { get; private set; }
        public DeviceCollectionViewModel DeviceCollection { get; private set; }

        private HardwareManager m_hardwareManager;

        public void OnApplicationLifecycleEvent(ApplicationLifecycleEvent evt)
        {
            if (evt == ApplicationLifecycleEvent.Startup)
            {
                Current = this;
                Settings = StorageFactory.GetSettings(Namespace);

                DeviceCollection = ((App)App.Current).GetDeviceCollection();
                m_hardwareManager = new HardwareManager(DeviceCollection, WindowsAudioFactory.Create(AudioDeviceKind.Playback));
            }
        }
    }
}
