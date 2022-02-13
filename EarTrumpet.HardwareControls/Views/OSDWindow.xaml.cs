using EarTrumpet.Extensions;
using EarTrumpet.Interop;
using EarTrumpet.Interop.Helpers;
using EarTrumpet.HardwareControls.ViewModels;
using EarTrumpet.UI.Helpers;
using System.Windows;
using System.Windows.Forms;

namespace EarTrumpet.HardwareControls.Views
{
    public partial class OSDWindow : Window
    {
        private OSDWindowViewModel _viewModel;

        public OSDWindow(OSDWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;

            InitializeComponent();

            _viewModel.StateChanged += OnStateChanged;
            _viewModel.ContentChanged += OnOSDContentChanged;

            SourceInitialized += (_, __) => this.Cloak();
            UI.Themes.Manager.Current.ThemeChanged += () => EnableAcrylicIfApplicable(WindowsTaskbar.Current);
        }

        public void Initialize()
        {
            Show();
            Hide();
            // Prevent showing up in Alt+Tab.
            this.ApplyExtendedWindowStyle(User32.WS_EX_TOOLWINDOW);

            _viewModel.ChangeState(FlyoutViewState.Hidden);
        }

        private void OnOSDContentChanged(object sender, object e)
        {
            if (_viewModel.State == FlyoutViewState.Open)
            {
                PositionWindowRelativeToTaskbar(WindowsTaskbar.Current);
            }
        }

        private void OnStateChanged(object sender, object e)
        {
            switch (_viewModel.State)
            {
                case FlyoutViewState.Opening:
                    var taskbar = WindowsTaskbar.Current;

                    Show();
                    EnableAcrylicIfApplicable(taskbar);
                    PositionWindowRelativeToTaskbar(taskbar);

                    WindowAnimationLibrary.BeginWindowEntranceAnimation(this, () =>
                    {
                        _viewModel.ChangeState(FlyoutViewState.Open);
                    });
                    break;

                case FlyoutViewState.Closing_Stage1:
                    WindowAnimationLibrary.BeginWindowExitAnimation(this, () =>
                    {
                        this.Cloak();
                        AccentPolicyLibrary.DisableAcrylic(this);
                        _viewModel.ChangeState(FlyoutViewState.Hidden);
                    });

                    break;
            }
        }

        private void PositionWindowRelativeToTaskbar(WindowsTaskbar.State taskbar)
        {
            // We're not ready if we don't have a taskbar and monitor. (e.g. RDP transition)
            if (taskbar.ContainingScreen == null)
            {
                return;
            }

            // Force layout so we can be sure lists have created/removed containers.
            UpdateLayout();
            LayoutRoot.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double newWidth = Width * this.DpiX();
            double newHeight = LayoutRoot.DesiredSize.Height * this.DpiY();

            // Distance from corner to offset for each configuration.
            double OffsetX = Addon.Current.Settings.Get("OffsetX", 64);
            double OffsetY = Addon.Current.Settings.Get("OffsetY", 64);

            var resolution = Screen.PrimaryScreen.WorkingArea;
            
            double x = 0;
            double y = 0;

            switch (Addon.Current.Settings.Get("OverlayPosition", "TopLeft"))
            {
                case "TopLeft":
                    x = resolution.Left + OffsetX;
                    y = resolution.Top + OffsetY;
                    break;
                
                case "TopRight":
                    x = resolution.Right - newWidth - OffsetX;
                    y = resolution.Top + OffsetY;
                    break;
                
                case "Center":
                    x = ((resolution.Right - resolution.Left - newWidth) / 2) + OffsetX;
                    y = ((resolution.Bottom - resolution.Top - newHeight) / 2) + OffsetY;
                    break;
                
                case "BottomLeft":
                    x = resolution.Left + OffsetX;
                    y = resolution.Bottom - newHeight - OffsetY;
                    break;

                case "BottomRight":
                    x = resolution.Right - newWidth - OffsetX;
                    y = resolution.Bottom - newHeight - OffsetY;
                    break;
            }
            
            this.SetWindowPos(y, x, newHeight, newWidth);
        }

        private void EnableAcrylicIfApplicable(WindowsTaskbar.State taskbar)
        {
            // Note: Enable when in Opening as well as Open in case we get a theme change during a show cycle.
            if (_viewModel.State == FlyoutViewState.Opening || _viewModel.State == FlyoutViewState.Open)
            {
                AccentPolicyLibrary.EnableAcrylic(
                    this,
                    UI.Themes.Manager.Current.ResolveRef(this, "AcrylicColor_Flyout"),
                    User32.AccentFlags.DrawAllBorders);
            }
            else
            {
                // Disable to avoid visual issues like showing a pane of acrylic while we're Hidden+cloaked.
                AccentPolicyLibrary.DisableAcrylic(this);
            }
        }
    }
}
