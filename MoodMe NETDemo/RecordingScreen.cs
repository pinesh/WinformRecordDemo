using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using ScreenRecorderLib;
using Timer = System.Timers.Timer;

namespace MoodMe_NETDemo
{
    public partial class RecordingScreen : Form
    {
        private readonly RecordingHandler _x = new RecordingHandler();
        private readonly string _saveFolder;
        public string SavePath { get; private set; }
        public long StartTime { get; private set; }

 
        private DateTime _sTime;

        private readonly Timer _timer = new Timer(1000);
        public RecordingScreen(string s)
        {
            InitializeComponent();
            _x.CompleteStatus += OnRecordingSuccess;
            _x.ErrorStatus += OnRecordingError;
            _saveFolder = s;
            LBLTimer.Text = "00:00:00";
        }

        //Sets recording panel to always be in front of applications
        private static readonly IntPtr HwndTopmost = new IntPtr(-1);
        private const UInt32 TopmostFlags = 0x0002 | 0x0001;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        private void RecordingScreen_Load(object sender, EventArgs e)
        {
            SetWindowPos(Handle, HwndTopmost, 0, 0, 0, 0, TopmostFlags);
        }

        private void OnRecordingError(object sender, EventArgs e)
        {
            var s = ((RecordingFailedEventArgs) e).Error;
            MessageBox.Show(s);
            _timer.Stop();
            Close();
        }

        private void OnRecordingSuccess(object sender, EventArgs e)
        {
            SavePath = ((RecordingCompleteEventArgs) e).FilePath;
            if (SavePath.Length == 0)
                throw new Exception("Error recordings Path Failed");
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
            while (SavePath == null)
            {
                var duration = DateTime.Now - timeout;
                if (duration > TimeSpan.FromSeconds(5))
                {
                    MessageBox.Show(@"recordings Timeout Failure");
                    break;
                }
                Thread.Sleep(500);
            }

            Cursor.Current = Cursors.Default;
            Close();
        }
        private void OnTimerElapsed(object o, ElapsedEventArgs s)
        {
            var duration = DateTime.Now - _sTime;
            try
            {
                LBLTimer.Invoke((MethodInvoker)delegate { LBLTimer.Text = duration.ToString(@"hh\:mm\:ss"); });
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Start button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            StartTime = DateTime.UtcNow.ToFileTime();
            
            _sTime = DateTime.Now;
            _timer.Elapsed += (o, s) => Task.Factory.StartNew(() => OnTimerElapsed(o, s));
            _timer.Start();
            _x.StartRecording(_saveFolder);
            button1.Enabled = false;
            button2.Enabled = true;
        }
    }
}