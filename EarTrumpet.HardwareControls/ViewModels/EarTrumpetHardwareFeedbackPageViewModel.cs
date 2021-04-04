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
    public class EarTrumpetHardwareFeedbackPageViewModel : SettingsPageViewModel
    {
        public enum ItemModificationWays
        {
            NEW_EMPTY,
            NEW_FROM_EXISTING,
            EDIT_EXISTING
        }

        public ICommand NewCommandFeedback { get; }
        public ICommand EditSelectedCommandFeedback { get; }
        public ICommand DeleteSelectedCommandFeedback { get; }
        public ICommand NewFromSelectedCommandFeedback { get; }
        public ItemModificationWays ItemModificationWay { get; set; }
        public int SelectedIndex { get; set; }

        public ObservableCollection<string> HardwareFeedbackCommands
        {
            get
            {
                return _CommandFeedbackList;
            }

            set
            {
                _CommandFeedbackList = value;
                RaisePropertyChanged("HardwareFeedback");
            }
        }

        private WindowHolder _hardwareFeedbackSettingsWindow;
        private readonly ISettingsBag _settings;
        private DeviceCollectionViewModel _devices;
        ObservableCollection<String> _CommandFeedbackList = new ObservableCollection<string>();

        public EarTrumpetHardwareFeedbackPageViewModel() : base(null)
        {
            _settings = Addon.Current.Settings;
            _devices = Addon.Current.DeviceCollection;
            Glyph = "\xE9A1";
            Title = Properties.Resources.HardwareFeedbackTitle;

            NewCommandFeedback = new RelayCommand(NewFeedback);
            EditSelectedCommandFeedback = new RelayCommand(EditSelectedFeedback);
            DeleteSelectedCommandFeedback = new RelayCommand(DeleteSelectedFeedback);
            NewFromSelectedCommandFeedback = new RelayCommand(NewFromSelectedFeedback);

            _hardwareFeedbackSettingsWindow = new WindowHolder(CreateHardwareSettingsExperience);

            UpdateCommandFeedbackList();

            // The command controls list should have no item selected on startup.
            SelectedIndex = -1;
        }

        public void CommandFeedbackMappingSelectedCallback(CommandFeedbackMappingElement commandFeedbackMappingElement)
        {
            switch (ItemModificationWay)
            {
                case ItemModificationWays.NEW_EMPTY:
                case ItemModificationWays.NEW_FROM_EXISTING:
                    HardwareManager.Current.AddCommandFeedback(commandFeedbackMappingElement);
                    break;
                case ItemModificationWays.EDIT_EXISTING:
                    // Notify the hardware controls page about the new assignment.
                    HardwareManager.Current.ModifyCommandFeedbackAt(SelectedIndex, commandFeedbackMappingElement);
                    break;
            }

            UpdateCommandFeedbackList();

            _hardwareFeedbackSettingsWindow.OpenOrClose();
        }

        private Window CreateHardwareSettingsExperience()
        {
            var viewModel = new HardwareSettingsViewModel(_devices, this);
            return new HardwareSettingsWindow {DataContext = viewModel};
        }

        private void NewFeedback()
        {
            ItemModificationWay = ItemModificationWays.NEW_EMPTY;
            _hardwareFeedbackSettingsWindow.OpenOrBringToFront();
        }
        private void EditSelectedFeedback()
        {
            var selectedIndex = SelectedIndex;

            if (selectedIndex < 0)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.NoCommandFeedbackSelectedMessage, "EarTrumpet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ItemModificationWay = ItemModificationWays.EDIT_EXISTING;
            _hardwareFeedbackSettingsWindow.OpenOrBringToFront();
        }

        private void NewFromSelectedFeedback()
        {
            var selectedIndex = SelectedIndex;

            if (selectedIndex < 0)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.NoCommandFeedbackSelectedMessage, "EarTrumpet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ItemModificationWay = ItemModificationWays.NEW_FROM_EXISTING;
            _hardwareFeedbackSettingsWindow.OpenOrBringToFront();
        }
        
        private void DeleteSelectedFeedback()
        {
            var selectedIndex = SelectedIndex;

            if(selectedIndex < 0)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.NoCommandFeedbackSelectedMessage, "EarTrumpet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            HardwareManager.Current.RemoveCommandFeedbackAt(selectedIndex);
            UpdateCommandFeedbackList();
        }

        private void UpdateCommandFeedbackList()
        {
            var commandFeedbackList = HardwareManager.Current.GetCommandFeedbackMappings();

            ObservableCollection<String> commandsFeedbackStringList = new ObservableCollection<string>();

            foreach (var item in commandFeedbackList)
            {
                string commandControlsString =
                    "Audio Device=" + item.audioDevice +
                    ", Command=" + item.command +
                    ", Mode=" + item.mode +
                    ", Selection=" + item.indexApplicationSelection +
                    ", Device Type=" + HardwareManager.Current.GetConfigType(item) + ", " +
                    item.hardwareConfiguration;

                commandsFeedbackStringList.Add(commandControlsString);
            }

            HardwareFeedbackCommands = commandsFeedbackStringList;
        }
    }
}
