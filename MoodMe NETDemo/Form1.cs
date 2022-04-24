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
                dataGridView1.DataBindings.Add(new Binding("DataSource", _m, "Recordings"));
                dataGridView1.CellClick += (s, ef) => _m.GridClick(s, ef);
                var removeButtonColumn = new DataGridViewButtonColumn();
                {
                    removeButtonColumn.Name = "del_col";
                    removeButtonColumn.HeaderText = "Delete";
                    removeButtonColumn.Text = "Delete";
                    removeButtonColumn.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(removeButtonColumn);
                }
                dataGridView1.MultiSelect = false;
                dataGridView1.Columns[0].Visible = false;

                for (var i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
            catch (Exception)
            {
                Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1 == null || e.ColumnIndex == dataGridView1.Columns["del_col"].Index) return;
            using (var previewForm = new Form2(ref _m))
            {
                previewForm.ShowDialog();
            }
        }

        private void BTNNewRecording_Click_1(object sender, EventArgs e)
        {
            using (var recordingForm = new RecordingScreen(_m.RecordingPath))
            {
                WindowState = FormWindowState.Minimized;
                recordingForm.ShowDialog();
                _m.TagText = "";
                WindowState = FormWindowState.Normal;
                if (recordingForm.SavePath == null) return;
                _m.CurrentRecording = recordingForm.SavePath;
                _m.LocalId = recordingForm.StartTime;
               
                using (var previewForm = new Form2(ref _m))
                {
                    previewForm.ShowDialog();
                }
            }
        }
    }

}

