using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace MoodMe_NETDemo
{
    internal class RecordingDbManager
        {
            private string _connectionString;
            private readonly string _dbname;

            //SQL Statements Used
            private const string CreateState =
                @"CREATE TABLE recordings(id INTEGER PRIMARY KEY,video TEXT NOT NULL UNIQUE, tag TEXT NOT NULL)";
            private const string SelectAll = @"SELECT * FROM recordings"; 
            private const string InsertOrUpdate =
                @"INSERT INTO recordings(id,video,tag) VALUES($id,$video,$tag)  ON CONFLICT(id) DO UPDATE SET tag=excluded.tag;";
            private const string GetSchema = @"SELECT sql FROM sqlite_master WHERE tbl_name = 'recordings';";
            private const string DeleteById = @"DELETE FROM recordings WHERE id = @id";

            public string RecordingFolder;
            public DataSet Ds;

            public RecordingDbManager(string name)
            {
                _dbname = name;
                EstablishDb();
                PopulateRecordingsDb();
            }

            /// <summary>
            /// Pulls the contents of the database to the local object. 
            /// </summary>
            internal void PopulateRecordingsDb()
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    var dap = new SQLiteDataAdapter(SelectAll, _connectionString);
                    Ds = new DataSet();
                    var creationCol = new DataColumn("creation_date", typeof(string));
                    creationCol.AllowDBNull = true;
                    dap.Fill(Ds);
                    Ds.Tables[0].Columns.Add(creationCol);
                    foreach (DataRow row in Ds.Tables[0].Rows)
                        row["creation_date"] = DateTime.FromFileTimeUtc((long)row[0]).ToLocalTime().ToString();
                    creationCol.AllowDBNull = false;
                    conn.Close();
                }
            }

            /// <summary>
            ///     Remove a DB entry by ID
            /// </summary>
            /// <param name="id">Long Primary Key</param>
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
                                comm.CommandText = DeleteById;
                                comm.Parameters.AddWithValue("@id", id);
                                comm.Prepare();
                                comm.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }

                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                PopulateRecordingsDb();
            }

            /// <summary>
            ///     Inserts an element into the database.
            /// </summary>
            /// <param name="video"> The filename</param>
            /// <param name="tag">The required tag</param>
            /// <param name="id">The creation time (doubles as primary key)</param>
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
                                comm.CommandText = InsertOrUpdate;
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                PopulateRecordingsDb();
            }

            /// <summary>
            ///     Asserts the database file found matches the expected schema.
            /// </summary>
            internal void VerifyDb()
            {
                try
                {
                    using (var conn = new SQLiteConnection(_connectionString))
                    {
                        var dap = new SQLiteDataAdapter(GetSchema, _connectionString);
                        var ds = new DataSet();
                        dap.Fill(ds);
                        if ((string)ds.Tables[0].Rows[0][0] != CreateState)
                            throw new Exception(
                                "Error, database schema cannot be read or does not match expected schema");

                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception();
                }
            }


            /// <summary>
            ///     Looks for the application database, if not present, creates it.
            /// </summary>
            internal void EstablishDb()
            {
                var dir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "Recordings");
                RecordingFolder = dir;
                try
                {
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    var dbPath = Path.Combine(dir, _dbname);
                    _connectionString = "Data Source=" + dbPath;
                    if (File.Exists(dbPath))
                    {
                        VerifyDb();
                        return;
                    }

                    SQLiteConnection.CreateFile(dbPath);

                    using (var conn = new SQLiteConnection(_connectionString))
                    {
                        conn.Open();
                        using (var comm = new SQLiteCommand())
                        {
                            comm.Connection = conn;
                            comm.CommandText = CreateState;
                            comm.ExecuteNonQuery();
                        }

                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception();
                }
            }
        }
    }