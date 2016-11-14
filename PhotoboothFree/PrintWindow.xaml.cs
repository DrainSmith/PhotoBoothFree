using System;
using System.Collections.Generic;
using System.IO;
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

namespace PhotoboothFree
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        private System.Timers.Timer CountDownTimer;
        BitmapSource _image;
        public PrintWindow(BitmapSource image)
        {
            InitializeComponent();
            Background = new SolidColorBrush(Colors.Black);
            WindowState = WindowState.Maximized;
            ImageDisplay.Source = image;
            _image = image;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            Guid photoID = System.Guid.NewGuid();
            String photolocation = photoID.ToString() + ".jpg";  //file name 
            photolocation = System.IO.Path.Combine(Settings.SaveFolder, photolocation);
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var filestream = new FileStream(photolocation, FileMode.Create))
                encoder.Save(filestream);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vis = new DrawingVisual();
            var dc = vis.RenderOpen();
            dc.DrawImage(_image, new Rect { Width = _image.Width, Height = _image.Height });
            dc.Close();

            if (!Settings.TestMode)
            {
                var dlg = new PrintDialog();
                var serv = new PrintServer();
                var printer = serv.GetPrintQueue(Settings.PrinterName);
                dlg.PrintQueue = printer;
                //dlg.PrintTicket.CopyCount = 1;
                dlg.PrintTicket.PageOrientation = PageOrientation.Portrait;
                dlg.PrintTicket.PageResolution = new PageResolution(300, 300);
                //PageMediaSize pageSize = new PageMediaSize(PageMediaSizeName.);
                dlg.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.NorthAmerica4x6);
                dlg.PrintVisual(vis, "Photobooth Free");
            }
            CountDownTimer = new System.Timers.Timer();
            CountDownTimer.Interval = 8000;
            CountDownTimer.Enabled = true;
            CountDownTimer.Elapsed += CountDownTimer_Elapsed;

        }

        private void CountDownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate {
                this.Close();
            }));
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
