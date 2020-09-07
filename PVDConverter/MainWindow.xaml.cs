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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Accord.Video.FFMPEG;
using NAudio.Wave;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;
using FFMpegSharp;
using FFMpegSharp.FFMPEG;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace PVDConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DataModel d = new DataModel();
        
        private string inputfname = "";
        private string outputfname = "";
        Thread C;
        Thread B;
        Thread A; 
        public MainWindow()
        {
            InitializeComponent();
            Textout.DataContext = d;
            Percentage.DataContext = d;
            d.TextOut = "Hello.";
            d.Percent = 0;
            B = new Thread(uv);
            A = new Thread(WriteFromWav);
            B.Start();
            C = new Thread(WavToColor);
        }
         void WriteFromWav()
        {
            d.TextOut = "";
            int i = 0;
           
            SplitWaves(inputfname);
            FileStream wav = new FileStream("TempVideo.wav", FileMode.Open);
            byte[] buffer = new byte[5880];
            wav.Position += 44 + 1358 + 1300;

            VideoFileWriter vwrite = new VideoFileWriter();

            vwrite.Open(outputfname, 80, 80, 15, VideoCodec.MPEG4);


            int bytesRead;
            while ((bytesRead =  wav.Read(buffer, 0, 5880)) > 0)
            {
                int pos = ffstart(buffer);

                if (pos == -1)
                {
                    if (bytesRead < 5880)
                    {
                        break;
                    }
                    continue;
                }
                else if (pos == -2)
                {
                    wav.Position -= 3250;
                    if (bytesRead < 5880)
                    {
                        break;
                    }
                    continue;
                }
                else
                {
                   d.Percent = (int) (((float)(wav.Position * 100)) / wav.Length);
                    d.TextOut = "Writing Frames... " + d.Percent + "%";
                    byte[] frame = new byte[6400];
                    byte[] tf = new byte[3200];
                    for (int j = 0; j < 3200; j++)
                    {
                        tf[j] = buffer[j + pos];
                    }



                    for (int x = 0; x < 80; x += 1)
                    {
                        for (int y = 0; y < 80; y += 2)
                        {
                            byte[] split = splitbyte(tf[40 * x + (y / 2)]);
                            frame[80 * x + y] = (byte)(split[1]);
                            frame[80 * x + y + 1] = (byte)(split[0]);
                        }

                    }
                    var q = tf;

                    GCHandle pinnedArray = GCHandle.Alloc(frame, GCHandleType.Pinned);
                    IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                    Bitmap f1 = new Bitmap(80, 80, 80, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, pointer);
                    ColorPalette _palette = f1.Palette;
                    System.Drawing.Color[] _entries = _palette.Entries;
                    for (int a = 0; a < 256; a += 1)
                    {
                        System.Drawing.Color b = new System.Drawing.Color();
                        b = System.Drawing.Color.FromArgb((byte)a, (byte)a, (byte)a);
                        _entries[a] = b;
                    }
                    f1.Palette = _palette;

                    vwrite.WriteVideoFrame(f1);

                    pinnedArray.Free();
                }
                i++;
                
            }
            vwrite.Close();
            d.TextOut = "Writing Audio...";
            
            //VideoInfo.FromPath("tmpvideo.mp4").ReplaceAudio(new FileInfo("TempAudio.wav"), new FileInfo(outputfname));
            d.TextOut = "Removing Temporary files...";
            //File.Delete("tmpvideo.mp4");
            //File.Delete("TempAudio.wav");
            //File.Delete("TempVideo.wav");
            d.TextOut = "Done!";
            d.Percent = 100;
        }
        void SplitWaves(string fin)
        {
            int c;
            var reader = new WaveFileReader(fin);
            if(reader.WaveFormat.Channels == 1)
            {
                c = 2;
            }
            else
            {
                c = reader.WaveFormat.Channels;
            }
            var buffer = new byte[2 * reader.WaveFormat.SampleRate * c];
            
            var writers = new WaveFileWriter[c];
            var format = new WaveFormat(reader.WaveFormat.SampleRate, 16, 1);
            writers[0] = new WaveFileWriter(String.Format("TempVideo.wav"), format);
            writers[1] = new WaveFileWriter(String.Format("TempAudio.wav"), format);
            int bytesRead;
            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                int offset = 0;
                while (offset < bytesRead)
                {
                    for (int n = 0; n < writers.Length; n++)
                    {
                        writers[n].Write(buffer, offset, 2);
                        offset += 2;
                    }
                d.TextOut = "Splitting Wav channels" + (((float)((reader.Position * 100000) / reader.Length)) / 1000) + "%";
                d.Percent = (int)(((float)((reader.Position * 100000) / reader.Length)) / 1000);
                }
            }
            Console.WriteLine();
            for (int n = 0; n < writers.Length; n++)
            {
                writers[n].Dispose();
            }
            reader.Dispose();
        }
        int ffstart(byte[] buffer)
        {
            int indx = 0;
            for (indx = 0; indx < buffer.Length - 1; indx++)
            {
                if (!(buffer[indx] == 225 || buffer[indx] == 210 || buffer[indx] == 195) && !(buffer[indx + 1] == 225 || buffer[indx + 1] == 210 || buffer[indx + 1] == 195))
                {
                    return (indx > 2680 || indx < 50) ? -2 : indx;
                }
            }
            return -1;
        }
        byte[] splitbyte(byte inpt)
        {
            byte[] outpt = new byte[2];
            outpt[0] = (byte)((inpt << 4) & 240);
            outpt[1] = (byte)((inpt & 240));
            return outpt;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".wav";
            dlg.Filter = "WAVE files (*.wav)|*.wav";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                inputfname = dlg.FileName;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".wav";
            dlg.Filter = "MP4 files (*.mp4)|*.mp4";
            Nullable<bool> result = dlg.ShowDialog();
            
            if (result == true)
            {
                outputfname = dlg.FileName;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            bool noi = string.IsNullOrEmpty(inputfname);
            bool noo = string.IsNullOrEmpty(outputfname);
            if (string.IsNullOrEmpty(inputfname) || string.IsNullOrEmpty(outputfname))
            {
                if (noi && !noo)
                {
                    d.TextOut = "No Input file selected";

                }else if(noo && !noi)
                {
                    d.TextOut = "No Output file selected";
                }
                else
                {
                    d.TextOut = "No Input or Output file selected";
                }
            }
            else
            {
                if ((bool)Color.IsChecked)
                {
                    C.Start();
                }
                else
                {
                    A.Start();
                }
            }
            
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Windows[0].Close();
        }
        private void uv()
        {
            Percentage.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
            Textout.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
        }

        private void WavToColor()
        {
            d.TextOut = "";
            int i = 0;

            FileStream wav = new FileStream(inputfname, FileMode.Open);
            byte[] buffer = new byte[19200];
            wav.Position += 16;
            
            VideoFileWriter vwrite = new VideoFileWriter();

            vwrite.Open(outputfname, 216, 160, 18, VideoCodec.MPEG4);


            int bytesRead;
            while ((bytesRead = wav.Read(buffer, 0, 19200)) > 0)
            {
                
                
                    d.Percent = (int)(((float)(wav.Position * 100)) / wav.Length);
                    d.TextOut = "Writing Frames... " + d.Percent + "%";
                    byte[] f1 = new byte[19200];
                    byte[] frame = new byte[216 * 160 * 3]; ;
                    byte[] audio = new byte[1920];

                    for (int j = 0; j < 19200; j+=10)
                    {
                        f1[j+1 ] = buffer[j+1];
                        f1[j+2 ] = buffer[j+2];
                        f1[j+3 ] = buffer[j+3];
                        f1[j+4 ] = buffer[j+4];
                    f1[j+5 ] = buffer[j+5];
                    f1[j+6 ] = buffer[j+6];
                    f1[j+7 ] = buffer[j+7];
                    f1[j+8 ] = buffer[j+8];
                    f1[j+9 ] = buffer[j+9];
                    audio[(j ) / 10] = buffer[j];
                    }

                    To24bppColor(f1,frame);

                    GCHandle pinnedArray = GCHandle.Alloc(frame, GCHandleType.Pinned);
                    IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                    Bitmap fx = new Bitmap(216, 160, 216, System.Drawing.Imaging.PixelFormat.Format24bppRgb, pointer);

                    vwrite.WriteVideoFrame(fx);
                    vwrite.WriteAudioFrame(audio);
                    pinnedArray.Free();
                
                i++;

            }
            vwrite.Close();

            d.TextOut = "Done!";
            d.Percent = 100;
        }
        void To24bppColor(byte[] iframe, byte[] oframe)
        {
            oframe = new byte[(4 * (216 * 24 + 31) / 32) * 160];
            for(int s = 0; s < 160; s+=2)
            {
                byte[,] sline = new byte[2, 120];
                for(int x = 0; x < 120; x++)
                {
                    sline[0, x] = iframe[s * x];
                    sline[1, x] = iframe[(s + 1) * x];
                }
                byte[] r = new byte[216];
                byte[] g = new byte[216];
                byte[] b = new byte[216];
                int z = 0;
                for (int i =0; i < 108; i+= 3)
                {

                    byte[] j, k, l, m,n,o = new byte[2];
                    j = splitbyte(sline[0, i]);
                    k = splitbyte(sline[0, i + 1]);
                    l = splitbyte(sline[0, i + 2]);
                    m = splitbyte(sline[1, i]);
                    n = splitbyte(sline[1, i + 1]);
                    o = splitbyte(sline[1, i + 2]);
                    

                    r[z] = j[0];
                    g[z] = m[1];
                    b[z] = m[0];
                    r[z + 1] = n[1];
                    g[z + 1] = n[0];
                    b[z + 1] = j[1];
                    r[z + 2] = o[0];
                    g[z + 2] = k[1];
                    b[z + 2] = k[0];
                    r[z + 3] = l[1];
                    g[z + 3] = l[0];
                    b[z + 3] = o[1];
                    z += 4;

                }
                for(int q = 0; q < 648; q+= 3)
                {
                    System.Drawing.Color xc = new System.Drawing.Color();
                    xc = System.Drawing.Color.FromArgb(r[q / 3], g[q / 3], b[q / 3]);
                    
                    oframe[216 * s + q] = xc.B;
                    oframe[216* s + (q + 1)] = xc.G;
                    oframe[216 * s + (q + 2)] = xc.R;
                }
            }
            
        }
        
        
    }
}
