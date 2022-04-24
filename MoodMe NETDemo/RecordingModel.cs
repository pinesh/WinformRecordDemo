using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MoodMe_NETDemo
{
    public class RecordingModel : INotifyPropertyChanged
    {
        public long? LocalId;
        private string _currentRecording = "";
        private string _tag = "";
        private string _initialTag;
        private DataTable _recordings;
        private readonly RecordingDbManager _recordingDb;

        private bool _tagEnabled;
        private bool _submitState;
        private bool _tagEmpty;
        public string RecordingPath;

        /// <summary>
        /// Constructor
        /// </summary>
        public RecordingModel()
        {

            _recordingDb = new RecordingDbManager("demo.db");
            Recordings = _recordingDb.Ds.Tables[0];
            RecordingPath = _recordingDb.RecordingFolder;
        }

        public DataTable Recordings
        {
            get => _recordings;
            set
            {
                _recordings = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Recordings"));
            }
        }

        /// <summary>
        /// LINQ Expression to check if a new entry is distinct.
        /// </summary>
        /// <param name="includeTag"> Bool flag to check beyond Primary key</param>
        /// <returns></returns>
        public bool Found(bool includeTag)
        {
            if (!LocalId.HasValue) return false;

            if (!includeTag)
            {
                var selectRows = Recordings.Rows.Cast<DataRow>()
                    .Where(row => row.Field<long>("id") == LocalId.Value);
                return selectRows.Any();
            }
            else
            {
                var selectRows = Recordings.Rows.Cast<DataRow>()
                    .Where(row => row.Field<long>("id") == LocalId.Value && row.Field<string>("tag") == _tag);
                return selectRows.Any();
            }

        }

        /// <summary>
        /// Current Recording File Label Property
        /// </summary>
        public string CurrentRecording
        {
            get => _currentRecording;
            set
            {
                _currentRecording = value;
                if (PropertyChanged == null) return;
                if (_currentRecording.Length > 0)
                    TagFieldEnabled = true;
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentRecording"));
            }
        }

        /// <summary>
        /// Submit Button Enabled Property
        /// </summary>
        public bool SubmitState
        {
            get => _submitState;
            set
            {
                _submitState = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubmitState)));
            }
        }

        /// <summary>
        /// Tag Text Enabled Property
        /// </summary>
        public bool TagFieldEnabled
        {
            get => _tagEnabled;
            set
            {
                _tagEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagFieldEnabled)));
            }
        }

        /// <summary>
        /// Tag Text Field Empty Checker Property
        /// </summary>
        public bool TagFieldEmpty
        {
            get => _tagEmpty;
            set
            {
                _tagEmpty = value;
                if (_tagEmpty)
                    SubmitState = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagFieldEmpty)));
            }
        }

        /// <summary>
        /// Tag Text Field Property
        /// </summary>
        public string TagText
        {
            get => _tag;
            set
            {
                _tag = value;
                if (PropertyChanged == null) return;
                SubmitState = _tag != _initialTag;
                TagFieldEmpty = !(_tag.Length > 0);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagText)));
            }
        }


        /// <summary>
        /// PropertyChangedTrigger
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Remove an entry from the Database, deleting the local recording file
        /// </summary>
        /// <param name="id"> The primary key time ID</param>
        /// <param name="filePath">The path of the file</param>
        public void DeleteEntry(long id, string filePath)
        {
            try
            {
                var f = new FileInfo(filePath);
                if(f.Exists)
                    f.Delete();
                _recordingDb.Remove(id);
                Recordings = _recordingDb.Ds.Tables[0];
            }
            catch (Exception e)
            {
                PromptError(e.Message);
            }
        }

        /// <summary>
        /// GridClick the current recording context to a previous entry in the database or otherwise flags delete.
        /// </summary>
        /// <param name="e"> Cell info</param>
        public void GridClick(object sender ,DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > Recordings.Rows.Count - 1 || e.RowIndex < 0) return;

            if (e.ColumnIndex == ((DataGridView)sender).Columns["del_col"].Index)
            {
                var result = MessageBox.Show(@"Are you sure you want to delete?", @"Delete Recording", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes) return;
                DeleteEntry((long)Recordings.Rows[e.RowIndex].ItemArray[0], (string)Recordings.Rows[e.RowIndex].ItemArray[1]);
                return;
            }

            if (!Found(true) && _currentRecording != "")
            {
                var result = MessageBox.Show(@"You have an unsaved changes, do you want to discard?", @"Discard Previous Recording", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes) return;
            }

            var r = Recordings.Rows[e.RowIndex];
            _initialTag = (string)r.ItemArray[2];
            LocalId = (long)r.ItemArray[0];
            CurrentRecording = Path.Combine(RecordingPath,(string)r.ItemArray[1]);
            TagText = (string)r.ItemArray[2];
        }

        /// <summary>
        /// Tag Text Box text changed event handler.
        /// </summary>
        /// <param name="sender">Text box</param>
        /// <param name="e">Change event</param>
        public void TagTextChanged(object sender, EventArgs e)
        {
            TagText = ((TextBox)sender).Text;
        }

        /// <summary>
        /// Internal prompter for error display.
        /// </summary>
        /// <param name="message"></param>
        internal void PromptError(string message)
        {
            const string title = "Error";
            const MessageBoxButtons buttons = MessageBoxButtons.OK;
            const MessageBoxIcon icon = MessageBoxIcon.Error;
            MessageBox.Show(message, title, buttons, icon);
        }

        public delegate void Close();

        /// <summary>
        /// Submit a new entry to the db. 
        /// </summary>
        public void Submit(Close c)
        {
            //verify integrity of current file, must exist. 
            var f = new FileInfo(CurrentRecording);
            if (!f.Exists)
            {
                PromptError("Error, File not present locally");
                return;
            }

            if (TagText.Length == 0)
            {
                PromptError("Error, Tag Empty");
                return;
            }

            if (!LocalId.HasValue)
            {
                PromptError("Error, ID Empty");
                return;
            }

            var result = MessageBox.Show(@"Are you sure you want to upload?", @"Confirm Submit", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

            _recordingDb.Transact(Path.GetFileName(CurrentRecording), TagText, LocalId.Value);
            //PropertyChanged(this, new PropertyChangedEventArgs("RecordingDB"));
             Recordings = _recordingDb.Ds.Tables[0];
            //Reset Locals
            LocalId = null;
            _initialTag = "";
            TagText = "";
            CurrentRecording = "";
            c();
        }

        /// <summary>
        /// Facilitates Cancel  Button and form closing.
        /// </summary>
        /// <param name="e"></param>
        public void ClearCancel(FormClosingEventArgs e)
        {
            if (Found(true) || _currentRecording == "") return;
            var result = MessageBox.Show(@"You have an unsaved changes, do you want to discard?", @"Discard Previous Recording", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            var f = new FileInfo(CurrentRecording);
            if (f.Exists) 
                f.Delete();
            LocalId = null;
            _initialTag = "";
            TagText = "";
            CurrentRecording = "";
            Cursor.Current = Cursors.Default;
        }
    }
    }