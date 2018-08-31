using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alvas.Audio;

namespace VoIPFileConverter
{
    public partial class Form1 : Form
    {
        public const string path = @"\\\\192.168.3.241\\ManualMix\\Good\\";

        public Form1()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            

            // OPEN FILE
            using (openFileDialog1)
            {
                openFileDialog1.InitialDirectory = path;
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // DO STUFF
                textBox1.Text = openFileDialog1.FileName.ToString();
            }
            else
            {
                MessageBox.Show("There was a problem opening the file explorer");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // CONVERT TO CCITT
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConvertToMP3("", "", 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openFileDialog1_HelpRequest(object sender, EventArgs e)
        {

        }
        private void ConvertToCCITT(String f, String FileName)
        {
            try
            {

                WaveReader newWr = new WaveReader(File.OpenRead(f));
                IntPtr oldPcm = newWr.ReadFormat();
                byte[] oldPcmData = newWr.ReadData();
                IntPtr newFormat = AudioCompressionManager.GetPcmFormat(1, 16, 8000);
                byte[] newData = { };

                WaveFormat wf = AudioCompressionManager.GetWaveFormat(oldPcm);

                newWr.Close();

                if (FileName != "0")
                    f = FileName;

                if (File.Exists(textBox1.Text + Path.GetFileName(f)))
                    File.Delete(textBox1.Text + Path.GetFileName(f));

                // **** debug ****
                //txtStatus.Text += f + eol;

                int samp = wf.nSamplesPerSec;
                int bps = wf.wBitsPerSample;

                // sample rate is > 8000
                if (samp > 8000)
                {
                    newData = AudioCompressionManager.Resample(oldPcm, oldPcmData, newFormat);
                }

                IntPtr ccittOut = AudioCompressionManager.GetCompatibleFormat(newFormat, AudioCompressionManager.MuLawFormatTag);
                byte[] finalData = AudioCompressionManager.Convert(newFormat, ccittOut, newData, false);
                WaveWriter finalWr = new WaveWriter(File.Create(textBox1.Text + Path.GetFileName(f)),
                    AudioCompressionManager.FormatBytes(ccittOut));

                finalWr.WriteData(finalData);
                finalWr.Close();

                // **** debug *****
                //TextOps(dirUlaw + Path.GetFileName(f) + eol2);

                //DeleteFile(f);

            }
            catch (NullReferenceException nex)
            {
                //TextOps("NullReferenceException: " + nex.Message.ToString() + cr);

            }
            catch (IOException iex)
            {
                //TextOps("IOException: " + iex.Message.ToString() + cr);

            }
            catch (AudioException ex)
            {
                //TextOps("AudioException: " + ex.Message.ToString() + cr);

            }
        }

        // convert to MP3
        private void ConvertToMP3(String f, String FileName, int LocType)
        {
            try
            {

                WaveReader wr = new WaveReader(File.OpenRead(f));
                IntPtr oldFormat = wr.ReadFormat();
                byte[] oldData = wr.ReadData();
                wr.Close();

                if (FileName != "0")
                    f = FileName;

                // if voip, write to voip dir otherwise write to MP3 (N7 & iLink) dir
                if (LocType == 2)
                    textBox1.Text = textBox2.Text;
                else
                    textBox1.Text = textBox1.Text;

                if (File.Exists(textBox1.Text + Path.GetFileName(f)))
                    File.Delete(textBox1.Text + Path.GetFileName(f));
                //txtStatus.Text += f + eol;

                IntPtr pcmFormat = AudioCompressionManager.GetCompatibleFormat(oldFormat,
                    AudioCompressionManager.PcmFormatTag);
                byte[] pcmData = AudioCompressionManager.Convert(oldFormat, pcmFormat, oldData, false);

                IntPtr mp3Format = AudioCompressionManager.GetMp3Format(1, 112, 44100);

                f = f.Replace("wav", "mp3");

                Mp3Writer mw = new Mp3Writer(File.Create(textBox1.Text + Path.GetFileName(f)));

                byte[] newData = AudioCompressionManager.Convert(pcmFormat, mp3Format, oldData, false);
                mw.WriteData(newData);
                mw.Close();

                // **** debug *****
                //TextOps(dirMp3 + Path.GetFileName(f) + eol2);

                //DeleteFile(f);
            }
            catch (NullReferenceException nex)
            {
                //TextOps("NullReferenceException: " + nex.Message.ToString() + cr);

            }
            catch (IOException iex)
            {
                //TextOps("IOException: " + iex.Message.ToString() + cr);

            }
            catch (AudioException ex)
            {
                //TextOps("AudioException: " + ex.Message.ToString() + cr);

            }
        }

        // CONVERT TO PCM 8KHz 16-bit mono
        private void ConvertToPcm(String f, String FileName)
        {
            try
            {
                WaveReader wr = new WaveReader(File.OpenRead(f));
                IntPtr oldFormat = wr.ReadFormat();
                byte[] oldData = wr.ReadData();
                IntPtr newFormat = AudioCompressionManager.GetPcmFormat(1, 16, 8000);
                byte[] newData = { };
                WaveFormat wf = AudioCompressionManager.GetWaveFormat(oldFormat);

                wr.Close();

                if (FileName != "0")
                    f = FileName;

                if (File.Exists(textBox1.Text + Path.GetFileName(f)))
                    File.Delete(textBox1.Text + Path.GetFileName(f));

                // **** debug ****
                //txtStatus.Text += f + eol;

                int samp = wf.nSamplesPerSec;
                int bps = wf.wBitsPerSample;

                // sample rate is > 8000
                if (samp > 8000)
                {
                    newData = AudioCompressionManager.Resample(oldFormat, oldData, newFormat);
                    WaveWriter ww = new WaveWriter(File.Create(textBox1.Text + Path.GetFileName(f)),
                        AudioCompressionManager.FormatBytes(newFormat));
                    ww.WriteData(newData);
                    ww.Close();

                    // **** debug *****
                    //TextOps(dirPcm + Path.GetFileName(f) + eol2);
                }
                else
                {
                    //TextOps(Path.GetFileName(f) + " already exists at 8KHz" + cr);
                }

                //DeleteFile(f);

            }
            catch (NullReferenceException nex)
            {
                //TextOps("NullReferenceException: " + nex.Message.ToString() + cr);

            }
            catch (IOException iex)
            {
                //TextOps("IOException: " + iex.Message.ToString() + cr);

            }
            catch (AudioException ex)
            {
                //TextOps("AudioException: " + ex.Message.ToString() + cr);

            }
        }

    }
}
