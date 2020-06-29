using System.Globalization;
using System.Windows;
using RamMachine.Model;

namespace RamMachine.UI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly Settings _appSettings;

        public SettingsWindow(Settings appSettings)
        {
            _appSettings = appSettings;
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _appSettings.TimerTicksInSeconds = decimal.Parse(tbTimerTicksInSec.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            Close();
        }

        private void SettingsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            tbTimerTicksInSec.Text = _appSettings.TimerTicksInSeconds.ToString(CultureInfo.InvariantCulture);
        }
    }
}
