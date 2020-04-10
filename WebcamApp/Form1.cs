using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebcamApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /* AForge.NET 參考來源：
         * https://www.youtube.com/watch?v=Thu6DXTnM_M
         * http://www.zendei.com/article/10552.html
         * https://codertw.com/%E4%BA%BA%E5%B7%A5%E6%99%BA%E6%85%A7/129851/
         */


        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        private void Form1_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
            {

                cboCamera.Items.Add(filterInfo.Name);
            }

            cboCamera.SelectedIndex = 0;
            videoCaptureDevice = new VideoCaptureDevice();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);

            //解析度，VideoResolution 預設為 null
            //指定解析度與 webcam 相同
            videoCaptureDevice.VideoResolution = videoCaptureDevice.VideoCapabilities[cboCamera.SelectedIndex];
            //自訂解析度
            //videoCaptureDevice.VideoResolution = selectResolution(videoCaptureDevice);  

            //左邊畫面
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();

            //右邊畫面，截圖來源
            videoSourcePlayer1.VideoSource = videoCaptureDevice;
            videoSourcePlayer1.Start();
        }

        private static VideoCapabilities selectResolution(VideoCaptureDevice device)
        {
            foreach (var cap in device.VideoCapabilities)
            {
                if (cap.FrameSize.Height == 1080)
                    return cap;
                if (cap.FrameSize.Width == 1920)
                    return cap;
            }
            return device.VideoCapabilities.Last();
        }

        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            pic.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice == null)
                return;

            //尚未啟動攝影機
            if (videoCaptureDevice.Source == null)
                return;

            Bitmap bitmap = videoSourcePlayer1.GetCurrentVideoFrame();
            string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ff") + ".jpg";
            bitmap.Save(@"H:\temp\" + fileName, ImageFormat.Jpeg);
            bitmap.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCaptureDevice.IsRunning == true)
                videoCaptureDevice.Stop();
        }
    }
}
