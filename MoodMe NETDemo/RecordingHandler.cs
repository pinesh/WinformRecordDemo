using System;
using System.IO;
using ScreenRecorderLib;

namespace MoodMe_NETDemo
{
    internal class RecordingHandler : IDisposable
    {
        Recorder _rec;
        private string _name;
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
        }

        public void StartRecording(string p)
        {
            _name = DateTime.UtcNow.ToFileTime().ToString();
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
            //var path = e.FilePath;
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