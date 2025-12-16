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
            
            public MediaItem(String mediaPath)
            {
                FilePath = mediaPath;
                FileName = Path.GetFileName(mediaPath); // Get name of the file from selected path
            }
        }

        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.png;*.bmp";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MediaItem item = new MediaItem(dialog.FileName); // Assign the file to the MediaItem class
                MediaLibrary.Add(item); // Add object to the MediaLibrary list

                MediaListBox.Items.Add(item.FileName); // Display objects in Media List field
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
            int index = MediaListBox.SelectedIndex; // Assign the selected file index

            if (index < 0)
            {
                return;
            }

            MediaItem selectedItem = MediaLibrary[index]; // Get MediaItem object from MediaLibrary List at selected index
            pictureBox1.Image = Image.FromFile(selectedItem.FilePath); // Load the image file stored in the selected MediaItem and display it in the PictureBox
        }
    }
}
