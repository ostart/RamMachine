using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using RamMachine.Model;

namespace RamMachine.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Settings _appSettings;
        private readonly DispatcherTimer _timer;
        private readonly Model.RamMachine _ramMachine;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Tick += TimerTick;
            _appSettings = new Settings();
            _ramMachine = new Model.RamMachine();
            _ramMachine.OutputWriteOccured += RamMachineOnOutputWriteOccured;
        }

        private void RamMachineOnOutputWriteOccured(object sender, OutputEventArgs e)
        {
            tbResult.Text += $"{_ramMachine.GetAdder()}{Environment.NewLine}";
        }

        private void TimerTick(object sender, EventArgs e)
        {
            try
            {
                tbCurrentExecutedLine.Text = _ramMachine.GetCurrentExecutedCommand();
                _ramMachine.NextStep();
            }
            catch (Exception ex)
            {
                _timer.Stop();
                MessageBox.Show(ex.Message);
            }
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_appSettings);
            settingsWindow.Show();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                tbCommands.Text = File.ReadAllText(filePath, Encoding.UTF8);
            }
            _ramMachine.SetState(tbCommands.Text, tbData.Text, true);
        }

        private void SaveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, tbCommands.Text, Encoding.UTF8);
            }
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbCommands.Text)) return;

            Settings.IsEnabled = false;
            New.IsEnabled = false;
            Step.IsEnabled = false;
            FileMenuItem.IsEnabled = false;
            Run.IsEnabled = false;
            tbCommands.IsEnabled = false;
            tbData.IsEnabled = false;

            _ramMachine.SetState(tbCommands.Text, tbData.Text, true);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, (int) (_appSettings.TimerTicksInSeconds * 1000));
            _timer.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Settings.IsEnabled = true;
            New.IsEnabled = true;
            Step.IsEnabled = true;
            FileMenuItem.IsEnabled = true;
            Run.IsEnabled = true;
            tbCommands.IsEnabled = true;
            tbData.IsEnabled = true;

            _timer.Stop();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            tbCommands.Text = string.Empty;
            tbData.Text = string.Empty;
            tbResult.Text = string.Empty;
            tbCurrentExecutedLine.Text = string.Empty;
        }

        private void Step_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbCommands.Text)) return;
            try
            {
                _ramMachine.SetState(tbCommands.Text, tbData.Text);
                tbCurrentExecutedLine.Text = _ramMachine.GetCurrentExecutedCommand();
                _ramMachine.NextStep();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
