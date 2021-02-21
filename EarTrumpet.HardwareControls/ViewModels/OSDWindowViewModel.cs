using EarTrumpet.UI.Helpers;
using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;

namespace EarTrumpet.HardwareControls.ViewModels
{
    public class OSDWindowViewModel : BindableBase
    {
        public event EventHandler<object> StateChanged;
        public event EventHandler<object> ContentChanged;

        public ICommand DisplaySettingsChanged { get; }
        public FlyoutViewState State { get; private set; }
        public object OSDContent { get; private set; }


        private readonly DispatcherTimer _closeTimer = new DispatcherTimer();

        public OSDWindowViewModel()
        {
            DisplaySettingsChanged = new RelayCommand(() => BeginClose());
            _closeTimer.Interval = TimeSpan.FromSeconds(Addon.Current.Settings.Get("DisplayTime", 2));
            _closeTimer.Tick += (_, __) =>
            {
                _closeTimer.Stop();
                BeginClose();
            };
        }

        public void ShowForContent(object content)
        {
            if (!Addon.Current.Settings.Get("EnableOverlay", true))
            {
                return;
            }
            
            bool isChanged = content != OSDContent;

            OSDContent = content;
            RaisePropertyChanged(nameof(OSDContent));

            // Extend timeout
            _closeTimer.Stop();
            _closeTimer.Interval = TimeSpan.FromSeconds(Addon.Current.Settings.Get("DisplayTime", 2));
            _closeTimer.Start();

            if (State == FlyoutViewState.Open)
            {
                if (isChanged)
                {
                    ContentChanged?.Invoke(this, null);
                }
            }
            else
            {
                BeginOpen();
            }
        }

        public void ChangeState(FlyoutViewState state)
        {
            Trace.WriteLine($"OSDWindowViewModel: ChangeState {State} -> {state}");
            // TODO: should assert all state transitions are legal/valid/expected, like FlyoutVieModel.cs.
            State = state;
            StateChanged?.Invoke(this, state);
        }

        private void BeginOpen()
        {
            if (State == FlyoutViewState.Hidden)
            {
                ChangeState(FlyoutViewState.Opening);
            }
            // TODO: 'trigger open while closing animation playing' scenario is unhandled.
        }

        private void BeginClose()
        {
            if (State == FlyoutViewState.Open)
            {
                ChangeState(FlyoutViewState.Closing_Stage1);
            }
        }

        public void ImmediateClose()
        {
            BeginClose();
        }
    }
}
