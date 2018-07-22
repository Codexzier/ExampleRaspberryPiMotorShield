using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ExampleRaspberryPiMotorShield
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _viewModel = new MainPageViewModel();
        private MotorShield _motorShield = new MotorShield();

        private DispatcherTimer _timer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = this._viewModel;

            this._viewModel.StatusText = "Ini Motor driver";

            this._motorShield.Init();

            this._timer.Interval = TimeSpan.FromSeconds(1);
            this._timer.Tick += this._timer_Tick;
            this._timer.Start();
        }
        
        private async void _timer_Tick(object sender, object e)
        {
            this._timer.Stop();

            this._viewModel.StatusText = "Forward";
            this._motorShield.SetForeward();
            await Task.Delay(2000);

            this._viewModel.StatusText = "Backward";
            this._motorShield.SetBackward();
            await Task.Delay(2000);

            this._viewModel.StatusText = "Turn left";
            this._motorShield.SetTurnLeft();
            await Task.Delay(2000);

            this._viewModel.StatusText = "Turn Right";
            this._motorShield.SetTurnRight();
            await Task.Delay(2000);

            this._timer.Start();
        }
    }
}
