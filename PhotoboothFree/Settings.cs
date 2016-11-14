using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace PhotoboothFree
{
    public static class Settings
    {
        public static int SideMargin = 32;
        public static int TopMargin = 32;
        public static int ImageGap = 48;
        public static int WaitTimeInSeconds = 4;
        public static string PrinterName = "";
        public static string BackgroundImagePath = "StripBackground.png";
        public static string CameraName = "";
        public static string SaveFolder = "";
        public static int VideoCapabilityIndex = 0;
        public static bool TestMode = false;

        public static void WriteSettings()
        {
            string settingsPath = "PhotoboothSettings.xml";

            var xmlSave = new PhotoBoothSetttings();
            xmlSave.SideMargin = SideMargin;
            xmlSave.TopMargin = TopMargin;
            xmlSave.ImageGap = ImageGap;
            xmlSave.WaitTimeInSeconds = WaitTimeInSeconds;
            xmlSave.PrinterName = PrinterName;
            xmlSave.BackgroundImagePath = BackgroundImagePath;
            xmlSave.CameraName = CameraName;
            xmlSave.VideoCapabilityIndex = VideoCapabilityIndex;
            xmlSave.SaveFolder = SaveFolder;
            xmlSave.TestMode = TestMode;
            XmlSerializer xmlSerial = XmlSerializer.FromTypes(new[] { typeof(PhotoBoothSetttings) })[0];
            try
            {
                using (Stream fStream = new FileStream(settingsPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    xmlSerial.Serialize(fStream, xmlSave);
                }

            }
            catch (Exception)
            {

            }
        }

        public static bool ReadSettings()
        {
            if (File.Exists("PhotoboothSettings.xml"))
            {
                try
                {
                    string settingsPath = "PhotoboothSettings.xml";
                    var xmlSave = new PhotoBoothSetttings();
                    XmlSerializer xmlSerial = XmlSerializer.FromTypes(new[] { typeof(PhotoBoothSetttings) })[0];
                    using (Stream fStream = new FileStream(settingsPath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        xmlSave = (PhotoBoothSetttings)xmlSerial.Deserialize(fStream);
                    }
                    SideMargin = xmlSave.SideMargin;
                    TopMargin = xmlSave.TopMargin;
                    ImageGap = xmlSave.ImageGap;
                    WaitTimeInSeconds = xmlSave.WaitTimeInSeconds;
                    PrinterName = xmlSave.PrinterName;
                    BackgroundImagePath = xmlSave.BackgroundImagePath;
                    CameraName = xmlSave.CameraName;
                    VideoCapabilityIndex = xmlSave.VideoCapabilityIndex;
                    SaveFolder = xmlSave.SaveFolder;
                    TestMode = xmlSave.TestMode;
                    return true;
                }
                catch (Exception ex)
                {

                }
            }
            return false;
        }
    }

    public class PhotoBoothSetttings
    {
        public int SideMargin = 32;
        public int TopMargin = 32;
        public int ImageGap = 48;
        public int WaitTimeInSeconds = 4;
        public string PrinterName = "";
        public string BackgroundImagePath = "StripBackground.png";
        public string CameraName = "";
        public int VideoCapabilityIndex = 0;
        public string SaveFolder = "";
        public bool TestMode = false;
    }
}
