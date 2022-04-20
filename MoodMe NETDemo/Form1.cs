using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScreenRecorderLib;

namespace MoodMe_NETDemo
{
    public partial class Form1 : Form
    {
        private RecordingViewModel vm;

        public Form1()
        {
            InitializeComponent();
            vm = new RecordingViewModel();
            //vm.RecordingBindingSource = 
            // this.Load +=
            //store paths of video
        }

        /*
        <Grid Background = "White" HorizontalAlignment="Right" VerticalAlignment="Top"  >
        <!-- overlay with hint text -->
        <TextBlock Margin = "5,2" MinWidth="50" Text="Suche..." 
        Foreground="LightSteelBlue" Visibility="{Binding ElementName=txtSearchBox, Path=Text.IsEmpty, Converter={StaticResource MyBoolToVisibilityConverter}}" IsHitTestVisible="False"/>
        <!-- enter term here -->
        <TextBox MinWidth = "50" Name="txtSearchBox" Background="Transparent" />
        </Grid>

        class ideas, 
        Files should be saved to an appdata folder, the database will then store their paths for retrieval.
        full mvvm

        */
        /// <summary>
        /// Verify integrity of database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            RecordingDataBase d = new RecordingDataBase();
            dataGridView1.DataSource = d.ds.Tables[0];
        }


        /// <summary>
        /// Submit a new/updated Row INSERT/UPDATE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void TagTextBox_TextChanged(object sender, EventArgs e)
        {
            if (TagTextBox.Text.Length > 0)
            {
                textBox1.Visible = false;
                button1.Enabled = true;

            }
            else
            {
                textBox1.Visible = true;
                button1.Enabled = false;
            }
        }

        public class RecordingDataBase
        {
            private string connectionString;
            private string dbname = "demo.db";
            public DataSet ds;

            public RecordingDataBase()
            {
                EstablishDB();
                PopulateDB();
            }

            internal void PopulateDB()
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    SQLiteDataAdapter dap = new SQLiteDataAdapter("SELECT * FROM recordings", connectionString);
                    ds = new DataSet();
                    dap.Fill(ds);
                    conn.Close();
                }
            }

            internal void Transact(string video, string tag, int id)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        using (SQLiteTransaction tran = conn.BeginTransaction())
                        {
                            using (SQLiteCommand comm = new SQLiteCommand())
                            {
                                comm.Connection = conn;
                                comm.CommandText =
                                    "INSERT INTO recordings(id,video,tag, creationDate) VALUES($id,$video,$tag)";
                                //comm.Parameters.AddWithValue("$id", DateTime.UtcNow.ToFileTime());
                                comm.Parameters.AddWithValue("$id", id);
                                comm.Parameters.AddWithValue("$video", video);
                                comm.Parameters.AddWithValue("$tag", tag);
                                comm.Prepare();
                                comm.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }

                        conn.Close();
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            internal void EstablishDB()
            {
                var dir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Recordings");
                try
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    var dbPath = Path.Combine(dir, dbname);
                    connectionString = "Data Source=" + dbPath;
                    if (!File.Exists(dbPath))
                    {
                        SQLiteConnection.CreateFile(dbPath);

                        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                        {
                            conn.Open();
                            using (SQLiteCommand comm = new SQLiteCommand())
                            {
                                comm.Connection = conn;
                                comm.CommandText =
                                    @"CREATE TABLE recordings(id INTEGER PRIMARY KEY,video TEXT, tag TEXT)";
                                comm.ExecuteNonQuery();
                            }

                            conn.Close();
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public class RecordingViewModel : IDisposable
        {
            private string db;


            public RecordingViewModel() => db = "hehe";
            public BindingSource RecordingBindingSource { get; set; }

            public void Load()
            {
                //set recording binding source here

            }

            public void Remove() => RecordingBindingSource.RemoveCurrent();
            public void New() => RecordingBindingSource.AddNew();

            public void Save()
            {
                RecordingBindingSource.EndEdit();

                //save db changes and push them back
            }

            public void Dispose()
            {
                //dispose of db entity
            }
        }
    }

}

