using System;
using System.ComponentModel;
using System.Configuration;
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

        /// <summary>
        /// Constructor
        /// </summary>
        public RecordingModel()
        {
            _recordingDb = new RecordingDbManager("demo.db");
            Recordings = _recordingDb.Ds.Tables[0];
        }



        public DataTable Recordings
        {
            get => _recordings;
            set
            {
                _recordings = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Recordings"));
                }
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
                if (PropertyChanged != null)
                {
                    if (_currentRecording.Length > 0)
                        TagFieldEnabled = true;
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentRecording"));
                }
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
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SubmitState"));
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
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("TagFieldEnabled"));
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
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("TagFieldEmpty"));
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
                _tag = value.Trim();
                if (PropertyChanged != null)
                {
                    SubmitState = _tag != _initialTag;
                    TagFieldEmpty = !(_tag.Length > 0);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagText)));
                    //PropertyChanged(this, new PropertyChangedEventArgs("TagText"));
                }
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


        public void DeleteEntry(long id, string filePath)
        {
            try
            {
                FileInfo f = new FileInfo(filePath);
                if(f.Exists)
                    f.Delete();
                this._recordingDb.Remove(id);
                Recordings = _recordingDb.Ds.Tables[0];

            }
            catch (Exception e)
            {
                PromptError(e.Message);
                return;
            }
        }


        public void Switch()
        {
           
        }


        /// <summary>
        /// GridClick the current recording context to a previous entry in the database.
        /// </summary>
        /// <param name="e"> Cell info</param>
        public void GridClick(object sender ,DataGridViewCellEventArgs e)
        {
            var temp = (DataGridView)sender;
            if (e.ColumnIndex == temp.Columns["del_col"].Index)
            {
                const string message = "Are you sure you want to delete?";
                const string title = "Delete Recording";
                const MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                var result = MessageBox.Show(message, title, buttons);
                if (result != DialogResult.Yes) return;
                DeleteEntry((long)Recordings.Rows[e.RowIndex].ItemArray[0], (string)Recordings.Rows[e.RowIndex].ItemArray[1]);
                return;
            }


            if (e.RowIndex > Recordings.Rows.Count - 1 || e.RowIndex < 0) return;

            if (!Found(true) && _currentRecording != "")
            {
                const string message = "You have an unsaved changes, do you want to discard?";
                const string title = "Discard Previous Recording";
                const MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                var result = MessageBox.Show(message, title, buttons);
                if (result != DialogResult.Yes) return;
            }

            var r = Recordings.Rows[e.RowIndex];
            _initialTag = (string)r.ItemArray[2];
            LocalId = (long)r.ItemArray[0];
            CurrentRecording = (string)r.ItemArray[1];
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
            //verify integrity of currentfile, must exist. 
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

            const string message = "Are you sure you want to upload?";
            const string title = "Confirm Submit";
            const MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            var result = MessageBox.Show(message, title, buttons);
            if (result != DialogResult.Yes) return;

            _recordingDb.Transact(CurrentRecording, TagText, LocalId.Value);
            //PropertyChanged(this, new PropertyChangedEventArgs("RecordingDB"));
             Recordings = _recordingDb.Ds.Tables[0];
            //Reset Locals
            LocalId = null;
            _initialTag = "";
            TagText = "";
            CurrentRecording = "";
            c();
        }

        public void ClearCancel(Close c)
        {
            if (!Found(true) && _currentRecording != "")
            {
                const string message = "You have an unsaved changes, do you want to discard?";
                const string title = "Discard Previous Recording";
                const MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                var result = MessageBox.Show(message, title, buttons);
                if (result != DialogResult.Yes) return;
            }

            if (!Found(false))
            {
                var f = new FileInfo(CurrentRecording);
                if(f.Exists)
                    f.Delete();
            }

            LocalId = null;
            _initialTag = "";
            TagText = "";
            CurrentRecording = "";
            c();
        }
    }
    }