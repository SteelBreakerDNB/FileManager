using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class Form1 : Form
    {
        private List<MediaItem> MediaLibrary = new List<MediaItem>(); // List for storing Media objects
        public enum MediaType
        {
            Image,
            Audio,
            Video
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public class MediaItem
        {
            public String FileName;
            public String FilePath;
            public MediaType Type;
            
            public MediaItem(String mediaPath)
            {
                FilePath = mediaPath;
                FileName = Path.GetFileName(mediaPath); // Get name of the file from selected path
                Type = DetermineMediaType();
            }

            private MediaType DetermineMediaType()
            {
                String extension = Path.GetExtension(FilePath).ToLower();

                if (extension == ".jpg" || extension == ".png" || extension == ".bmp")
                {
                    return MediaType.Image;
                }
                if (extension == ".wav" || extension == ".flac" || extension == ".mp3")
                {
                    return MediaType.Audio;
                }
                if (extension == ".mp4" || extension == ".avi")
                {
                    return MediaType.Video;
                }
                else throw new NotSupportedException("This format is not supported");
            }

            public override string ToString()
            {
                return $"[{Type}] {FileName}";
            }

        }

        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MediaItem item = new MediaItem(dialog.FileName); // Assign the file to the MediaItem class
                MediaLibrary.Add(item); // Add object to the MediaLibrary list

                MediaListBox.Items.Add(item); // Display objects in Media List field
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void MediaListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearPreview();

            if (MediaListBox.SelectedIndex == -1) // Assign the selected file index
                return;

            MediaItem selectedItem = MediaLibrary[MediaListBox.SelectedIndex]; // Get MediaItem object from MediaLibrary List at selected index

            switch (selectedItem.Type)
            {
                case MediaType.Image:
                    pictureBox1.Visible = true;
                    axWindowsMediaPlayer1.Visible = false;
                    pictureBox1.Image = Image.FromFile(selectedItem.FilePath);
                    break;

                case MediaType.Audio:
                    pictureBox1.Visible = false;
                    axWindowsMediaPlayer1.Visible = true;
                    axWindowsMediaPlayer1.URL = selectedItem.FilePath;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    axWindowsMediaPlayer1.uiMode = "full";
                    break;

                case MediaType.Video:
                    pictureBox1.Visible = false;
                    axWindowsMediaPlayer1.Visible = true;
                    axWindowsMediaPlayer1.URL = selectedItem.FilePath;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    axWindowsMediaPlayer1.uiMode = "full";
                    break;
            }
        }


        private void ClearPreview()
        {
            pictureBox1.Image = null;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void ShowImageLocation(string filePath)
        {
            try
            {
                using (Image img = Image.FromFile(filePath))
                {
                    if (!img.PropertyIdList.Contains(0x0002))
                    {
                        MessageBox.Show("Brak danych GPS w obrazie.");
                        return;
                    }

                    PropertyItem latItem = img.GetPropertyItem(0x0002);
                    PropertyItem latRefItem = img.GetPropertyItem(0x0001);
                    PropertyItem lonItem = img.GetPropertyItem(0x0004);
                    PropertyItem lonRefItem = img.GetPropertyItem(0x0003);

                    double lat = ConvertToDegrees(latItem.Value);
                    double lon = ConvertToDegrees(lonItem.Value);

                    string latRef = Encoding.ASCII.GetString(new byte[] { latRefItem.Value[0] });
                    string lonRef = Encoding.ASCII.GetString(new byte[] { lonRefItem.Value[0] });

                    if (latRef == "S") lat = -lat;
                    if (lonRef == "W") lon = -lon;

                    string url = $"https://www.openstreetmap.org/?mlat={lat}&mlon={lon}#map=15/{lat}/{lon}";

                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    };

                    System.Diagnostics.Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd GPS: " + ex.Message);
            }
        }

        private double ConvertToDegrees(byte[] value)
        {
            double degrees = BitConverter.ToUInt32(value, 0) /
                             (double)BitConverter.ToUInt32(value, 4);

            double minutes = BitConverter.ToUInt32(value, 8) /
                             (double)BitConverter.ToUInt32(value, 12);

            double seconds = BitConverter.ToUInt32(value, 16) /
                             (double)BitConverter.ToUInt32(value, 20);

            return degrees + (minutes / 60.0) + (seconds / 3600.0);
        }

        private void ShowLocation_Click_1(object sender, EventArgs e)
        {
            if (MediaListBox.SelectedIndex == -1)
                return;

            MediaItem selectedItem = MediaLibrary[MediaListBox.SelectedIndex];

            if (selectedItem.Type == MediaType.Image)
                ShowImageLocation(selectedItem.FilePath);
            else
                MessageBox.Show("GPS dane dostępne tylko dla obrazów.");
        }
    }
}
