using System;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision;
using AForge.Vision.Motion;
using System.IO.Ports;

namespace Goruntuisleme
{
    public partial class Form1 : Form
    {
        private VideoCaptureDevice kameraAygit;
        private FilterInfoCollection webcamAygit;
        private Byte[] buffer = new Byte[1];
        private bool serialok = false;
        private float map = 0f;

        public Form1()
        {
            InitializeComponent();
        }

        //*****************************************

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (kameraAygit != null && kameraAygit.IsRunning)
            {
                kameraAygit.SignalToStop();
                kameraAygit = null;                
            }
        }

        //*****************************************

        private void Form1_Load(object sender, EventArgs e)
        {
            webcamAygit = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in webcamAygit)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name); // WebCamleri listele
                comboBox1.SelectedIndex = 0;
            }          
                    
        }

        //*****************************************

        void cam_goruntu_new_frame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;

            if (radioButton1.Checked == true)
            {
                ColorFiltering filter = new ColorFiltering();
                filter.Red = new IntRange(255, 0);
                filter.Green = new IntRange(0, 75);
                filter.Blue = new IntRange(0, 75);
                filter.ApplyInPlace(image1);
                nesnebul(image1);
            }

            if (radioButton2.Checked == true)
            {
                ColorFiltering filter = new ColorFiltering();
                filter.Red = new IntRange(0, 75);
                filter.Green = new IntRange(0, 75);
                filter.Blue = new IntRange(100, 255);
                filter.ApplyInPlace(image1);
                nesnebul(image1);
            }

            if (radioButton3.Checked == true)
            {
                ColorFiltering filter = new ColorFiltering();
                filter.Red = new IntRange(80  , 255);
                filter.Green = new IntRange(80, 255);
                filter.Blue = new IntRange(30, 80);
                filter.ApplyInPlace(image1);
                nesnebul(image1);
            }
        }
        
        //*****************************************

       public void nesnebul(Bitmap image)
       {
        BlobCounter blobCounter = new BlobCounter();
        blobCounter.MinWidth = 2;
        blobCounter.MinHeight = 2;
        blobCounter.FilterBlobs = true;
        blobCounter.ObjectsOrder = ObjectsOrder.Size;
        Grayscale griFiltre = new Grayscale(0.2125, 0.7154, 0.0721);
        Bitmap griImage = griFiltre.Apply(image);
        blobCounter.ProcessImage(griImage);
        Rectangle[] rects = blobCounter.GetObjectsRectangles();
        pictureBox2.Image = griImage;
        foreach (Rectangle recs in rects)
            {
                if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0];
                        //Graphics g = Graphics.FromImage(image);
                        Graphics g = pictureBox1.CreateGraphics();
                        using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                        {
                            g.DrawRectangle(pen, objectRect);

                            //Cizdirilen Dikdörtgenin Koordinatlari aliniyor.
                            int objectX = objectRect.X + (objectRect.Width / 2);
                            int objectY = objectRect.Y + (objectRect.Height / 2);
                            g.DrawString(objectX.ToString() + "X" + objectY.ToString(), new Font("Arial", 12), Brushes.Red, new System.Drawing.Point(250, 1));

                            if (serialok == true)
                            {
                                int second = 0;
                                int offset = 300;
                                second = offset - Math.Abs(objectX);
                                map = (float)0.85 * second;
                                buffer[0] = (byte)Math.Abs((int)map);
                                serialPort1.Write(buffer, 0, 1);
                            }
                        }

                    g.Dispose();


                    }
            }
        }

       private void button1_Click(object sender, EventArgs e)
       {
           kameraAygit = new VideoCaptureDevice(webcamAygit[comboBox1.SelectedIndex].MonikerString);//webcam listesinden kafadan birinciyi al diyoruz.
           kameraAygit.NewFrame += new NewFrameEventHandler(cam_goruntu_new_frame);
           kameraAygit.DesiredFrameRate = 30;//saniyede kaç görüntü alsın istiyorsanız. FPS
           kameraAygit.DesiredFrameSize = new Size(320, 240);//görüntü boyutları
           kameraAygit.Start();   
       }

       private void button2_Click(object sender, System.EventArgs e)
       {
           try
           {
               serialPort1.PortName = "COM8";
               serialPort1.BaudRate = 9600 ;
               serialPort1.Open();
               button2.BackColor = Color.Green;
               serialok = true;
           }
           catch (Exception t)
           {
               MessageBox.Show(t.ToString());
           }

       }
      
    }
}



