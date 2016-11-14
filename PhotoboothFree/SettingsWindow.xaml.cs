using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Printing;
using AForge.Video.DirectShow;
using System.Windows.Forms;
using System.Threading;
using AForge.Video;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoboothFree
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        FilterInfoCollection cams;
        VideoCaptureDevice LocalWebCam;

        public SettingsWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var serv = new PrintServer();
            var printer = serv.GetPrintQueues();
            foreach (var s in printer)
            {
                PrinterComboBox.Items.Add(s.Name);
            }
            cams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo c in cams)
            {
                CameraComboBox.Items.Add(c.Name);
            }
            TopMarginTextBox.Text = Settings.TopMargin.ToString();
            CountDownTextBox.Text = Settings.WaitTimeInSeconds.ToString();
            SideMarginTextBox.Text = Settings.SideMargin.ToString();
            ImageGapTextBox.Text = Settings.ImageGap.ToString();
            PrinterComboBox.Text = Settings.PrinterName;
            BackgroundImageTextBox.Text = Settings.BackgroundImagePath;
            SaveFolderTextBox.Text = Settings.SaveFolder;
            CameraComboBox.Text = Settings.CameraName;
            TestModeCheckBox.IsChecked = Settings.TestMode;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PrinterComboBox.Text == string.Empty)
                {
                    System.Windows.MessageBox.Show("No Printer is selected.", "Photobooth Free");
                    return;
                }
                if (CameraComboBox.Text == string.Empty)
                {
                    System.Windows.MessageBox.Show("No Camera is selected.", "Photobooth Free");
                    return;
                }
                Settings.TopMargin = Int32.Parse(TopMarginTextBox.Text);
                Settings.SideMargin = Int32.Parse(SideMarginTextBox.Text);
                Settings.ImageGap = Int32.Parse(ImageGapTextBox.Text);
                Settings.WaitTimeInSeconds = Int32.Parse(CountDownTextBox.Text);
                Settings.PrinterName = PrinterComboBox.Text;
                if (System.IO.File.Exists(BackgroundImageTextBox.Text))
                    Settings.BackgroundImagePath = BackgroundImageTextBox.Text;
                if (SaveFolderTextBox.Text == string.Empty || System.IO.Directory.Exists(SaveFolderTextBox.Text))
                    Settings.SaveFolder = SaveFolderTextBox.Text;
                Settings.CameraName = CameraComboBox.Text;
                Settings.VideoCapabilityIndex = CameraCapabilityComboBox.SelectedIndex;
                Settings.TestMode = (bool)TestModeCheckBox.IsChecked;
                Settings.WriteSettings();
                this.Close();
            }
            catch
            {
                // TODO proper parsing of each number
                System.Windows.MessageBox.Show("Error occured while creating settings. Probably due to an incorrect value type entered into a field.", "Photobooth Free");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LocalWebCam != null)
            {
                LocalWebCam.Stop();
                LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame);
            }
            if (CameraComboBox.SelectedIndex == -1)
            {
                CameraCapabilityComboBox.Items.Clear();
                CameraCapabilityComboBox.Text = string.Empty;
                WebCamOptionsButton.IsEnabled = false;
            }
            else
            {
                FilterInfo i = cams[0];
                bool found = false;
                foreach (FilterInfo c in cams)
                {
                    if (c.Name == CameraComboBox.SelectedItem.ToString())
                    {
                        i = c;
                        found = true;
                    }
                }
                if (found)
                {
                    LocalWebCam = new VideoCaptureDevice(i.MonikerString);
                    foreach (VideoCapabilities vc in LocalWebCam.VideoCapabilities)
                    {
                        CameraCapabilityComboBox.Items.Add(vc.FrameSize.Width + "x" + vc.FrameSize.Height + " at " + vc.MaximumFrameRate + " fps");
                    }
                    LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                    LocalWebCam.Start();
                    CameraCapabilityComboBox.SelectedIndex = Settings.VideoCapabilityIndex;
                    WebCamOptionsButton.IsEnabled = true;
                }
            }

        }

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                System.Drawing.Image img = (Bitmap)eventArgs.Frame.Clone();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    frameHolder.Source = bi;
                }));
            }
            catch (Exception ex)
            {
            }
        }

        private void CameraCapabilityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BroweImageButton_Click(object sender, RoutedEventArgs e)
        {
            var i = new OpenFileDialog();
            i.InitialDirectory = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().EscapedCodeBase).LocalPath);
            var result = i.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                BackgroundImageTextBox.Text = i.FileName;
            }
            //    if (i != null)
        }

        private void BrosweFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var i = new FolderBrowserDialog();
            var result = i.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SaveFolderTextBox.Text = i.SelectedPath;
            }
        }

        private void WebCamOptionButton_Click(object sender, RoutedEventArgs e)
        {
            LocalWebCam.DisplayPropertyPage(IntPtr.Zero);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (LocalWebCam != null)
            {
                LocalWebCam.Stop();
                LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame);
            }
        }
    }
}       
