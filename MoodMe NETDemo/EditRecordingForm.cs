using System.Windows.Forms;

namespace MoodMe_NETDemo
{
    public partial class EditRecordingForm : Form
    {
        private readonly RecordingModel _m;

        private EditRecordingForm(ref RecordingModel pass)
        {
             _m = pass;
            InitializeComponent();
            //recordings Label
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

        public static EditRecordingForm CreateInstance(ref RecordingModel pass)
        {
            return new EditRecordingForm(ref pass);
        }

        /// <summary>
        /// Closing Delegate for passing to model
        /// </summary>
        private new void Closing()
        {
            Close();
        }

        private void LBLSelectedRecording_DoubleClick(object sender, System.EventArgs e)
        {
            toolTip1.Show("Copied!",LBLSelectedRecording,500);
            toolTip1.SetToolTip(LBLSelectedRecording,"Double click to copy");
        }
    }
}
