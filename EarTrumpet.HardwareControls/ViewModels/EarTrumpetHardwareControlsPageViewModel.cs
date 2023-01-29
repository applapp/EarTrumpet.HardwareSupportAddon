using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.ObjectModel;
using EarTrumpet.UI.Helpers;
using EarTrumpet.UI.ViewModels;
using EarTrumpet.HardwareControls.Interop.Hardware;
using EarTrumpet.HardwareControls.Views;
using EarTrumpet.DataModel.Storage;

namespace EarTrumpet.HardwareControls.ViewModels
{
    public class EarTrumpetHardwareControlsPageViewModel : SettingsPageViewModel
    {
        public enum ItemModificationWays
        {
            NEW_EMPTY,
            NEW_FROM_EXISTING,
            EDIT_EXISTING
        }

        public ICommand NewControlCommand { get; }
        public ICommand EditSelectedControlCommand { get; }
        public ICommand DeleteSelectedControlCommand { get; }
        public ICommand NewFromSelectedControlCommand { get; }
        public ItemModificationWays ItemModificationWay { get; set; }
        public int SelectedIndex { get; set; }

        public ObservableCollection<ControlMappingListEntry> HardwareControls
        {
            get
            {
                return _commandControlList;
            }

            set
            {
                _commandControlList = value;
                RaisePropertyChanged("HardwareControls");
            }
        }

        private WindowHolder _hardwareSettingsWindow;
        private readonly ISettingsBag _settings;
        private DeviceCollectionViewModel _devices;
        ObservableCollection<ControlMappingListEntry> _commandControlList = new ObservableCollection<ControlMappingListEntry>();

        public EarTrumpetHardwareControlsPageViewModel() : base(null)
        {
            _settings = Addon.Current.Settings;
            _devices = Addon.Current.DeviceCollection;
            Glyph = "\xE9A1";
            Title = Properties.Resources.HardwareControlsTitle;

            NewControlCommand = new RelayCommand(NewControl);
            EditSelectedControlCommand = new RelayCommand(EditSelectedControl);
            DeleteSelectedControlCommand = new RelayCommand(DeleteSelectedControl);
            NewFromSelectedControlCommand = new RelayCommand(NewFromSelectedControl);

            _hardwareSettingsWindow = new WindowHolder(CreateHardwareSettingsExperience);

            UpdateCommandControlsList();

            // The command controls list should have no item selected on startup.
            SelectedIndex = -1;
        }

        public void ControlCommandMappingSelectedCallback(CommandControlMappingElement commandControlMappingElement)
        {
            switch (ItemModificationWay)
            {
                case ItemModificationWays.NEW_EMPTY:
                case ItemModificationWays.NEW_FROM_EXISTING:
                    HardwareManager.Current.AddCommand(commandControlMappingElement);
                    break;
                case ItemModificationWays.EDIT_EXISTING:
                    // Notify the hardware controls page about the new assignment.
                    HardwareManager.Current.ModifyCommandAt(SelectedIndex, commandControlMappingElement);
                    break;
            }

            UpdateCommandControlsList();

            _hardwareSettingsWindow.OpenOrClose();
        }

        private Window CreateHardwareSettingsExperience()
        {
            var viewModel = new HardwareSettingsViewModel(_devices, this);
            return new HardwareSettingsWindow {DataContext = viewModel};
        }

        private void NewControl()
        {
            ItemModificationWay = ItemModificationWays.NEW_EMPTY;
            _hardwareSettingsWindow.OpenOrBringToFront();
        }
        private void EditSelectedControl()
        {
            var selectedIndex = SelectedIndex;

            if (selectedIndex < 0)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.NoControlSelectedMessage, "EarTrumpet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ItemModificationWay = ItemModificationWays.EDIT_EXISTING;
            _hardwareSettingsWindow.OpenOrBringToFront();
        }

        private void NewFromSelectedControl()
        {
            var selectedIndex = SelectedIndex;

            if (selectedIndex < 0)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.NoControlSelectedMessage, "EarTrumpet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ItemModificationWay = ItemModificationWays.NEW_FROM_EXISTING;
            _hardwareSettingsWindow.OpenOrBringToFront();
        }
        
        private void DeleteSelectedControl()
        {
            var selectedIndex = SelectedIndex;

            if(selectedIndex < 0)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.NoControlSelectedMessage, "EarTrumpet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            HardwareManager.Current.RemoveCommandAt(selectedIndex);
            UpdateCommandControlsList();
        }

        private void UpdateCommandControlsList()
        {
            var commandControlsList = HardwareManager.Current.GetCommandControlMappings();

            ObservableCollection<ControlMappingListEntry> mappings = new ObservableCollection<ControlMappingListEntry>();

            foreach (var item in commandControlsList)
            {
                var entry = new ControlMappingListEntry();

                switch (item.command) {
                    case CommandControlMappingElement.Command.SystemVolume: entry.Type = Properties.Resources.MappingsListTypeSysVolText; break;
                    case CommandControlMappingElement.Command.SystemMute: entry.Type = Properties.Resources.MappingsListTypeSysMuteText; break;
                    case CommandControlMappingElement.Command.ApplicationVolume: entry.Type = Properties.Resources.MappingsListTypeAppVolText; break;
                    case CommandControlMappingElement.Command.ApplicationMute: entry.Type = Properties.Resources.MappingsListTypeAppMuteText; break;
                    case CommandControlMappingElement.Command.SetDefaultDevice: entry.Type = Properties.Resources.MappingsListTypeSetDevText; break;
                    case CommandControlMappingElement.Command.CycleDefaultDevice: entry.Type = Properties.Resources.MappingsListTypeCycleDevText; break;
                    default: entry.Type = item.command.ToString(); break;
                }

                switch (item.command) {
                    case CommandControlMappingElement.Command.ApplicationVolume:
                    case CommandControlMappingElement.Command.ApplicationMute:
                        entry.Type = Properties.Resources.MappingsListTypeAppVolText;
                        if (item.mode == CommandControlMappingElement.Mode.Indexed) {
                            entry.Context = "[ " + item.indexApplicationSelection + " ]";
                        } else {
                            entry.Context = item.indexApplicationSelection;
                        }
                        break;
                    case CommandControlMappingElement.Command.SystemVolume:
                    case CommandControlMappingElement.Command.SystemMute:
                    case CommandControlMappingElement.Command.SetDefaultDevice:
                    case CommandControlMappingElement.Command.CycleDefaultDevice:
                    default:
                        entry.Context = item.audioDevice;
                        break;
                }

                entry.Control = item.hardwareConfiguration.ToStringCompact();

                mappings.Add(entry);
            }

            HardwareControls = mappings;
        }
    }
}
