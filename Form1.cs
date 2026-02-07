using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            public MediaItem Item;
            
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

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Convert.ToString(pictureBox1.Image.Size));
        }

        private void MediaListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearPreview();

            int index = MediaListBox.SelectedIndex; // Assign the selected file index

            if (index < 0)
            {
                return;
            }

            MediaItem selectedItem = MediaLibrary[index]; // Get MediaItem object from MediaLibrary List at selected index

        switch (selectedItem.Type)
            {
                case MediaType.Image:
                    pictureBox1.Image = Image.FromFile(selectedItem.FilePath);
                    break;

                case MediaType.Audio:
                    axWindowsMediaPlayer1.URL = selectedItem.FilePath;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    axWindowsMediaPlayer1.uiMode = "mini";
                    break;

                case MediaType.Video:
                    MessageBox.Show("Video playback is not Implemented yet");
                    break;
            }                              
        }

        private void ClearPreview()
        {
            pictureBox1.Image = null;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }
    }
}
