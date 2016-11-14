using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoboothFree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _isMenuVisible = false;

        public static RoutedCommand ShowHideMenuCommand = new RoutedCommand();
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            WindowStartupLocation = WindowStartupLocation.Manual;
            this.Top = 0;
            this.Left = 0;
            ShowHideMenuCommand.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(ShowHideMenuCommand, ShowHideMenuCommandExecuted));
            if (!Settings.ReadSettings())
            {
                System.Windows.MessageBox.Show("Settings file either doesn't exist or it is broken. The settings window will now be shown so that you can create your initial setup.", "Photobooth Free", MessageBoxButton.OK);
                SettingsWindow sw = new SettingsWindow();
                sw.ShowDialog();
            }
        }

        private void ShowHideMenuCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_isMenuVisible)
            {
                _isMenuVisible = false;
                MainMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                _isMenuVisible = true;
                MainMenu.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var p = new PhotoboothWindow();
            p.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowHideMenuCommandExecuted(this, null);
        }
    }
}
