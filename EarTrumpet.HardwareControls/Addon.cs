using EarTrumpet.DataModel.Audio;
using EarTrumpet.DataModel.WindowsAudio;
using EarTrumpet.Extensibility;
using EarTrumpet.HardwareControls.DataModel;
using EarTrumpet.HardwareControls.Interop.Hardware;
using EarTrumpet.HardwareControls.ViewModels;
using EarTrumpet.HardwareControls.Views;
using EarTrumpet.UI.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace EarTrumpet.HardwareControls
{
    [Export(typeof(EarTrumpetAddon))]
    class Addon : EarTrumpetAddon, IEarTrumpetAddonEvents, IEarTrumpetAddonSettingsPage
    {
        public static Addon Current { get; private set; }

        public DeviceCollectionViewModel DeviceCollection { get; private set; }

        private HardwareManager m_hardwareManager;
        private OSDWindow _osdWindow;
        private OSDWindowViewModel _osdWindowViewModel;
        private FlyoutViewModel _flyoutViewModel;

        public Addon() : base()
        {
            DisplayName = Properties.Resources.HardwareControlsTitle;
        }

        public void OnAddonEvent(AddonEventKind evt)
        {
            if (evt == AddonEventKind.InitializeAddon)
            {
                Current = this;

                DeviceCollection = ((App)App.Current).CollectionViewModel;
                m_hardwareManager = new HardwareManager(DeviceCollection, WindowsAudioFactory.Create(AudioDeviceKind.Playback));
                
                
                // Monitor the EarTrumpet flyout so we don't show when the flyout is showing.
                _flyoutViewModel = (FlyoutViewModel)((App)Application.Current).FlyoutWindow.DataContext;
                _flyoutViewModel.StateChanged += OnFlyoutViewModelStateChanged;
                
                // Create a window to use as OSD.
                _osdWindowViewModel = new OSDWindowViewModel();
                _osdWindow = new OSDWindow(_osdWindowViewModel);
                _osdWindow.Initialize();
                
                // Listen to events and then present the active ViewModel to the OSD window.
                PlaybackDataModelHost.Current.AppPropertyChanged += OnAppPropertyChanged;
                PlaybackDataModelHost.Current.DevicePropertyChanged += OnDevicePropertyChanged;
            }
        }

        public SettingsCategoryViewModel GetSettingsCategory()
        {
            LoadAddonResources();
            return new SettingsCategoryViewModel(
                "Hardware Support",
                "\xE9A1",
                "MIDI and other hardware devices",
                Manifest.Id, new List<SettingsPageViewModel> {
                new EarTrumpetHardwareControlsPageViewModel(),
                new OSDViewModel(),
            });
        }

        private void OnFlyoutViewModelStateChanged(object sender, object e)
        {
            if (_flyoutViewModel.State != UI.Helpers.FlyoutViewState.Hidden)
            {
                // Any Flyout state change except the final hide should end OSD.
                _osdWindowViewModel.ImmediateClose();
            }
        }

        private void OnDevicePropertyChanged(IAudioDevice device, string propertyName)
        {
            if (propertyName == nameof(device.Volume) ||
                propertyName == nameof(device.IsMuted))
            {
                // Trace.WriteLine($"{device.DisplayName}: {device.Volume} {device.IsMuted}");
                TriggerOSDForDevice(device.Id);
            }
        }

        private void OnAppPropertyChanged(IAudioDeviceSession session, string propertyName)
        {
            if (propertyName == nameof(session.Volume) ||
                propertyName == nameof(session.IsMuted))
            {
                // Trace.WriteLine($"{session.DisplayName}: {session.Volume} {session.IsMuted}");
                TriggerOSDForApp(session.Parent.Id, session.AppId);
            }
        }

        private void TriggerOSDForApp(string deviceId, string appId)
        {
            if (CanShowOSD())
            {
                var device = DeviceCollection.AllDevices.FirstOrDefault(d => d.Id == deviceId);
                var app = device.Apps.FirstOrDefault(a => a.AppId == appId);
                _osdWindowViewModel.ShowForContent(app);
            }
        }

        private void TriggerOSDForDevice(string id)
        {
            if (CanShowOSD()) 
            {
                _osdWindowViewModel.ShowForContent(DeviceCollection.AllDevices.FirstOrDefault(d => d.Id == id));
            }
        }

        private bool CanShowOSD()
        {
            // Don't appear when EarTrumpet flyout is open.
            return _flyoutViewModel.State == UI.Helpers.FlyoutViewState.Hidden;
        }
    }
}
