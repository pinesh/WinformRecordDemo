using System;
using System.Windows.Forms;

namespace MoodMe_NETDemo
{
    public partial class BaseForm : Form
    {
        private RecordingModel _m;
        public BaseForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _m = new RecordingModel();

                //Data Binding
                //DataGridViewRecordings.DataSource = _m.bindingSource1;
                DataGridViewRecordings.DataBindings.Add(new Binding("DataSource", _m, "BindSource"));
                DataGridViewRecordings.CellClick += (s, ef) => _m.GridClick(s, ef);
                var removeButtonColumn = new DataGridViewButtonColumn();
                {
                    removeButtonColumn.Name = "del_col";
                    removeButtonColumn.HeaderText = "Delete";
                    removeButtonColumn.Text = "Delete";
                    removeButtonColumn.UseColumnTextForButtonValue = true;
                    DataGridViewRecordings.Columns.Add(removeButtonColumn);
                }
               

                DataGridViewRecordings.MultiSelect = false;
                DataGridViewRecordings.Columns[0].Visible = false;

                for (var i = 0; i < DataGridViewRecordings.Columns.Count; i++)
                {
                    DataGridViewRecordings.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        /// <summary>
        /// Cell click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DataGridViewRecordings == null || e.ColumnIndex == DataGridViewRecordings.Columns["del_col"].Index) return;
            using (var previewForm = EditRecordingForm.CreateInstance(ref _m))
            {
                previewForm.ShowDialog();
            }
        }

        /// <summary>
        /// New recording form click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTNNewRecording_Click_1(object sender, EventArgs e)
        {
            using (var recordingForm = new RecordingScreen(_m.RecordingPath))
            {
                WindowState = FormWindowState.Minimized;
                recordingForm.ShowDialog();
             
                //Naughty code, could push this out of base form and into model.
                _m.TagText = "";
                WindowState = FormWindowState.Normal;
                if (recordingForm.SavePath == null) return;
                _m.CurrentRecording = recordingForm.SavePath;
                _m.LocalId = recordingForm.StartTime;
               
                using (var previewForm = EditRecordingForm.CreateInstance(ref _m))
                {
                    previewForm.ShowDialog();
                }
            }
        }

       
    }

}

