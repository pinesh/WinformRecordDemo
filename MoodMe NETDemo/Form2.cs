using System.Windows.Forms;

namespace MoodMe_NETDemo
{
    public partial class Form2 : Form
    {
        private readonly RecordingModel _m;
        public Form2(ref RecordingModel pass)
        {
             _m = pass;
            InitializeComponent();
            //Recording Label
            LBLSelectedRecording.DataBindings.Add(new Binding("Text", pass, "CurrentRecording"));

            //Tag TextBox Binding
            TagTextBox.DataBindings.Add(new Binding("Text", pass, "TagText"));
            TagTextBox.DataBindings.Add(new Binding("Enabled", pass, "TagFieldEnabled"));
            axWindowsMediaPlayer1.URL = _m.CurrentRecording;

            //Submit button
            BTNSubmitRecording.DataBindings.Add(new Binding("Enabled", pass, "SubmitState"));

            //Empty Tag Display
            textBox1.DataBindings.Add(new Binding("Visible", pass, "TagFieldEmpty"));

            //Event Binding
            BTNSubmitRecording.Click += (s, e) => _m.Submit(Closing);
            BTNCancel.Click += (s, e) => Close(); 
            TagTextBox.TextChanged += (s, e) => _m.TagTextChanged(s, e);
            FormClosing += (s, e) => _m.ClearCancel(e);
        }
        private new void Closing()
        {
            Close();
        }

    }
}
