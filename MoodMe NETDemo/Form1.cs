using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ScreenRecorderLib;

namespace MoodMe_NETDemo
{
    public partial class Form1 : Form
    {
 
        private RecordingModel _m;

        public Form1()
        {
            InitializeComponent();
            _m = new RecordingModel();
            //Data Binding
            //DB Display
            dataGridView1.DataBindings.Add(new Binding("DataSource", this._m, "Recordings"));
            dataGridView1.CellClick += (s, e) =>  _m.GridClick(s,e);
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridViewButtonColumn removeButtonColumn = new DataGridViewButtonColumn();
            {
                removeButtonColumn.Name = "del_col";
                removeButtonColumn.HeaderText = "Delete";
                removeButtonColumn.Text = "Delete";
                removeButtonColumn.UseColumnTextForButtonValue = true;
            

                this.dataGridView1.Columns.Add(removeButtonColumn);
            }
            this.dataGridView1.Columns[0].Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1 == null || e.ColumnIndex == dataGridView1.Columns["del_col"].Index) return;
            using (var PreviewForm = new Form2(ref _m))
            {
                // this.WindowState = FormWindowState.Minimized;
                PreviewForm.ShowDialog();
                // dataGridView1.DataSource = null;
            }
        }

        private void BTNNewRecording_Click_1(object sender, EventArgs e)
        {
            using (var recordingForm = new RecordingScreen())
            {
                this.WindowState = FormWindowState.Minimized;
                recordingForm.ShowDialog();

                //This bit is somewhat of a poor coding practice. Full software would map these to bindings in model.
                this._m.TagText = "";
                this._m.CurrentRecording = recordingForm.SavePath;
                this._m.LocalId = recordingForm.StartTime;
                this.WindowState = FormWindowState.Normal;
                using (var PreviewForm = new Form2(ref _m))
                {
                    // this.WindowState = FormWindowState.Minimized;
                    PreviewForm.ShowDialog();
                    // dataGridView1.DataSource = null;
                }
            }
        }
    }

}

