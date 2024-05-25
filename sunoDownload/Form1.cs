using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace sunoDownload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string shareLink = textBox1.Text;
            string songName = textBox2.Text;
            string songId = ExtractSongId(shareLink);

            if (string.IsNullOrWhiteSpace(songId))
            {
                MessageBox.Show("Invalid share link. Please provide a valid Suno share link.");
                return;
            }

            string audioUrl = $"https://cdn1.suno.ai/{songId}.mp3";
            string outputFolder = Path.Combine(Application.StartupPath, "Downloads");
            string outputFileName = string.IsNullOrWhiteSpace(songName) ? $"{songId}.wav" : $"{songName}.wav";
            string outputPath = Path.Combine(outputFolder, outputFileName);

            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(audioUrl);

                    response.EnsureSuccessStatusCode();

                    byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();

                    if (!Directory.Exists(outputFolder))
                    {
                        Directory.CreateDirectory(outputFolder);
                    }

                    // Save the audio bytes to a file in the "Downloads" folder
                    await File.WriteAllBytesAsync(outputPath, audioBytes);

                    MessageBox.Show("Audio track downloaded successfully.");
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Error downloading audio track: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private string ExtractSongId(string shareLink)
        {
            if (string.IsNullOrWhiteSpace(shareLink))
                return string.Empty;

            const string songPrefix = "https://suno.com/song/";

            if (shareLink.StartsWith(songPrefix))
            {
                return shareLink.Substring(songPrefix.Length);
            }

            return string.Empty;
        }
    }
}