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
        private const string GetSchema = @"SELECT sql FROM sqlite_master WHERE tbl_name = 'recordings';";
        private const string CreateState =
                @"CREATE TABLE recordings(id INTEGER PRIMARY KEY,video TEXT NOT NULL UNIQUE, tag TEXT NOT NULL)";

        public string RecordingFolder;
        public RecordingDbManager(string name)
            {
                _dbname = name;
                EstablishDb();
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
                    throw ex;
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
                    throw ex;
                }
            }
        }
    }