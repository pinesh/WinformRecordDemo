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
        */
        /// <summary>
        /// Verify integrity of database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {

        }

     
    }

    public class RecordingDataBase
    {
        private string connectionString;
        private string dbname = "demo.db";
        



        public RecordingDataBase()
        {
            EstablishDB();
        }


        internal void Construct()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand comm = new SQLiteCommand())
                {
                        comm.Connection = conn;
                        comm.CommandText = " create table test (n integer) ";
                        comm.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        
        internal void Transact(string video,string tag)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand comm = new SQLiteCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = "INSERT INTO recordings(id,video,tag, creationDate) VALUES($id,$video,$tag)";
                        comm.Parameters.AddWithValue("$id", DateTime.UtcNow.ToFileTime());
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

        internal void EstablishDB()
        {
            var dir = Application.ExecutablePath;
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var dbPath = Path.Combine(dir,dbname);
                connectionString = "Data Source=" + dbPath;
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        using (SQLiteCommand comm = new SQLiteCommand())
                        {
                            comm.Connection = conn;
                            comm.CommandText = @"CREATE TABLE recordings(id INTEGER PRIMARY KEY,video TEXT, tag TEXT)";
                            comm.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
              
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(x.Message);
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
