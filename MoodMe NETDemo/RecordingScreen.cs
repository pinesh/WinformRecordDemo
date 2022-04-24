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
using ScreenRecorderLib;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
namespace MoodMe_NETDemo
{
    public partial class RecordingScreen : Form
    {
        RecordingHandler _x = new RecordingHandler();

        public string SavePath { get; private set; }
        public long StartTime { get; private set; }

        private static readonly IntPtr HwndTopmost = new IntPtr(-1);
        private const UInt32 SwpNosize = 0x0001;
        private const UInt32 SwpNomove = 0x0002;
        private const UInt32 TopmostFlags = SwpNomove | SwpNosize;
        private DateTime _sTime;

        private System.Timers.Timer _timer = new System.Timers.Timer(1000);
        public RecordingScreen()
        {
            InitializeComponent();
            _x.CompleteStatus += OnRecordingSuccess;
            _x.ErrorStatus += OnRecordingError;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        private void RecordingScreen_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HwndTopmost, 0, 0, 0, 0, TopmostFlags);
            StartTime = DateTime.UtcNow.ToFileTime();
            LBLTimer.Text = "00:00:00";
            _sTime = DateTime.Now;
            _timer.Elapsed += (o, s) => Task.Factory.StartNew(() => OnTimerElapsed(o, s));
            _timer.Start();
            _x.StartRecording(Path.GetTempPath());
        }

        private void OnRecordingError(object sender, EventArgs e)
        {
            var s = ((RecordingFailedEventArgs) e).Error;
            MessageBox.Show(s);
            _timer.Stop();
            this.Close();
        }

        private void OnRecordingSuccess(object sender, EventArgs e)
        {
            SavePath = ((RecordingCompleteEventArgs) e).FilePath;
            if (SavePath.Length == 0)
                throw new Exception("Error Recording Path Failed");
         
         
        }


        /// <summary>
        /// Button Event that ends a screen recording, saving the file and returning to the base screen. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            var timeout = DateTime.Now;
            _x.EndRecording();
            Cursor.Current = Cursors.WaitCursor;
            while (this.SavePath == null)
            {
                var duration = DateTime.Now - timeout;
                if (duration > TimeSpan.FromSeconds(5))
                {
                    MessageBox.Show("Recording Timeout Failure");
                    break;
                }
                Thread.Sleep(1000);
            }

            Cursor.Current = Cursors.Default;
            this.Close();
        }
        private void OnTimerElapsed(object o, ElapsedEventArgs s)
        {
            var duration = DateTime.Now - _sTime;
            try
            {
                LBLTimer.Invoke((MethodInvoker)delegate { LBLTimer.Text = duration.ToString(@"hh\:mm\:ss"); });
            }
            catch (Exception ex)
            {
                return;
                //here we catch thread termination
            }

        }

    }

    internal class RecordingHandler : IDisposable
    {

        Recorder _rec;
        private String _name;
        public EventHandler CompleteStatus;
        public EventHandler ErrorStatus;
        public RecordingHandler()
        {
            CreateRecording();
        }

        /// <summary>
        /// Default ScreenRecorderLib Function
        /// </summary>
        public void CreateRecording()
        {

            _rec = Recorder.CreateRecorder();
            _rec.OnRecordingComplete += Rec_OnRecordingComplete;
            _rec.OnRecordingFailed += Rec_OnRecordingFailed;
            _rec.OnStatusChanged += Rec_OnStatusChanged;
            //Record to a file

        }

        public void StartRecording(string p)
        {
            _name = DateTime.UtcNow.ToFileTime().ToString();
            //TODO make it use time for name instead of test
            var videoPath = Path.Combine(p, _name + ".mp4");
            var f = new FileInfo(videoPath);
            if (f.Exists)
            {
                throw new Exception("File Already Present");
            }

            _rec.Record(videoPath);
        }

        /// <summary>
        /// Default ScreenRecorderLib Function
        /// </summary>
        public void EndRecording()
        {
            _rec.Stop();
        }

        public void Dispose()
        {
            //dispose of recording
            if (_rec.Status != RecorderStatus.Idle)
                EndRecording();
            _rec.Dispose();
        }

        /// <summary>
        /// Default ScreenRecorderLib Function
        /// </summary>
        public void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            //Get the file path if recorded to a file
            //MessageBox.Show(e.FilePath);
            CompleteStatus.Invoke(this, e);
            var path = e.FilePath;
        }

        /// <summary>
        /// Default ScreenRecorderLib Function
        /// </summary>
        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            var error = e.Error;
            ErrorStatus.Invoke(this,e);
        }

        /// <summary>
        /// Default ScreenRecorderLib Function
        /// </summary>
        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            var status = e.Status;
        }
    }
}