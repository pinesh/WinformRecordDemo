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

namespace MoodMe_NETDemo
{
    public partial class RecordingScreen : Form
    {
        RecordingHandler x = new RecordingHandler();

        public RecordingScreen()
        {
            InitializeComponent();
            x.CompleteStatus += ShowRecordingSuccess;
            x.ErrorStatus += ShowRecordingError;
        }

        private void RecordingScreen_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            button1.Enabled = false;
            button2.Enabled = true;
            x.StartRecording(Path.GetTempPath());
        }

        private void ShowRecordingError(object sender, EventArgs e)
        {
            var s = ((RecordingFailedEventArgs) e).Error;
            MessageBox.Show(s);
            this.Close();
        }

        private void ShowRecordingSuccess(object sender, EventArgs e)
        {
            var s =((RecordingCompleteEventArgs) e).FilePath;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            x.EndRecording();
            button1.Enabled = true;
            button2.Enabled = false;
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
            string videoPath = Path.Combine(p, _name + ".mp4");
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
            MessageBox.Show(e.FilePath);
            CompleteStatus.Invoke(this, e);
            string path = e.FilePath;
        }

        /// <summary>
        /// Default ScreenRecorderLib Function
        /// </summary>
        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            string error = e.Error;
            ErrorStatus.Invoke(this,e);
        }

        /// <summary>
        /// Default ScreenRecorderLib Function
        /// </summary>
        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
        }
    }
}