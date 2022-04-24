using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace MoodMe_NETDemo
{
    public class RecordingDbManager
        {
            private string _connectionString;
            private string _dbname;
            public DataSet Ds;

            public RecordingDbManager(string name)
            {
                _dbname = name;
                EstablishDb();
                PopulateDb();
            }

            internal void PopulateDb()
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    var dap = new SQLiteDataAdapter("SELECT * FROM recordings", _connectionString);
                    Ds = new DataSet();
                    dap.Fill(Ds);
                    conn.Close();
                }
            }

            //Remove entry from database by ID
            public void Remove(long id)
            {
                try
                {
                    using (var conn = new SQLiteConnection(_connectionString))
                    {
                        conn.Open();
                        using (var tran = conn.BeginTransaction())
                        {
                            using (var comm = new SQLiteCommand())
                            {
                                comm.Connection = conn;
                                comm.CommandText =
                                    $"DELETE FROM recordings WHERE id = @id";
                                comm.Parameters.AddWithValue("@id", id);
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
                PopulateDb();
        }
            


            internal void Transact(string video, string tag, long id)
            {
                try
                {
                    using (var conn = new SQLiteConnection(_connectionString))
                    {
                        conn.Open();
                        using (var tran = conn.BeginTransaction())
                        {
                            using (var comm = new SQLiteCommand())
                            {
                                comm.Connection = conn;
                                comm.CommandText =
                                    "INSERT INTO recordings(id,video,tag) VALUES($id,$video,$tag)  ON CONFLICT(id) DO UPDATE SET tag=excluded.tag;";
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
                PopulateDb();
            }

            internal void EstablishDb()
            {
                var dir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Recordings");
                try
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    var dbPath = Path.Combine(dir, _dbname);
                    _connectionString = "Data Source=" + dbPath;
                    if (File.Exists(dbPath)) return;
                    SQLiteConnection.CreateFile(dbPath);

                    using (var conn = new SQLiteConnection(_connectionString))
                    {
                        conn.Open();
                        using (var comm = new SQLiteCommand())
                        {
                            comm.Connection = conn;
                            comm.CommandText =
                                @"CREATE TABLE recordings(id INTEGER PRIMARY KEY,video TEXT NOT NULL UNIQUE, tag TEXT NOT NULL)";
                            comm.ExecuteNonQuery();
                        }

                        conn.Close();
                    }

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }