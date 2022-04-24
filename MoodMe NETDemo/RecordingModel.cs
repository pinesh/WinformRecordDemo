using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Migrations;
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

        private BindingSource _bindingSource1 = new BindingSource();

        private bool _tagEnabled;
        private bool _submitState;
        private bool _tagEmpty;
        public string RecordingPath;

        private readonly EntityDatabase.RecordingContext _db;

        internal void RefreshTable()
        {
            var query = from b in _db.Recordings select b;
            BindSource.DataSource = query.ToList();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RecordingModel(string name = "demo.db")
        {
            RecordingPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "Recordings");
            var dbPath = Path.Combine(RecordingPath, name);
            //Establish and validate the internal database. 
            RecordingDbManager m = new RecordingDbManager(name);

            //Attach Entity Framework to database.
            _db = new EntityDatabase.RecordingContext(dbPath);
            RefreshTable();
        }

        public BindingSource BindSource
        {
            get => _bindingSource1;
            set
            {
                _bindingSource1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BindSource"));
            }
        }

        /// <summary>
        /// Current recordings File Label Property
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
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentRecording)));
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
        /// LINQ Expression to check if a new entry is distinct.
        /// </summary>
        /// <param name="includeTag"> Bool flag to check beyond Primary key</param>
        /// <returns></returns>
        public bool Found(bool includeTag)
        {
            if (!LocalId.HasValue) return false;
            if (!includeTag)
            {
                var query = from b in _db.Recordings where b.id == LocalId.Value select b;
                return query.Any();
            }
            else
            {
                var query = from b in _db.Recordings where b.id == LocalId.Value && b.tag == _tag select b;
                return query.Any();
            }

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
                _db.Recordings.Remove(_db.Recordings.First(c => c.id == id));
                _db.SaveChanges();
                this.RefreshTable();
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

            if (e.RowIndex > ((List<EntityDatabase.recordings>)BindSource.DataSource).Count - 1 || e.RowIndex < 0) return;

            if (e.ColumnIndex == ((DataGridView)sender).Columns["del_col"].Index)
            {
                var result = MessageBox.Show(@"Are you sure you want to delete?", @"Delete recordings", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes) return;
                var b =BindSource.DataSource.ToString();
                DeleteEntry(((List<EntityDatabase.recordings>)BindSource.DataSource)[e.RowIndex].id, Path.Combine(RecordingPath, ((List<EntityDatabase.recordings>)BindSource.DataSource)[e.RowIndex].video));
                return;
            }

            if (!Found(true) && _currentRecording != "")
            {
                var result = MessageBox.Show(@"You have an unsaved changes, do you want to discard?", @"Discard Previous recordings", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes) return;
            }

            var r = ((List<EntityDatabase.recordings>)BindSource.DataSource)[e.RowIndex];
            _initialTag = r.tag;
            LocalId = r.id;
            CurrentRecording = Path.Combine(RecordingPath,(string)r.video);
            TagText = (string)r.tag;
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

        protected internal void CleanProps()
        {
            LocalId = null;
            _initialTag = "";
            TagText = "";
            CurrentRecording = "";
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
            _db.Recordings.AddOrUpdate(new EntityDatabase.recordings(){id=LocalId.Value,tag = TagText,video = Path.GetFileName(CurrentRecording)});
            _db.SaveChanges();
            this.RefreshTable();
            //Reset Locals
            this.CleanProps();
            c();
        }

        /// <summary>
        /// Facilitates Cancel  Button and form closing.
        /// </summary>
        /// <param name="e"></param>
        public void ClearCancel(FormClosingEventArgs e)
        {
            if (Found(true) || _currentRecording == "") return;
            var result = MessageBox.Show(@"You have an unsaved changes, do you want to discard?", @"Discard Previous recordings", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            var f = new FileInfo(CurrentRecording);
            if (f.Exists) 
                f.Delete();
            this.CleanProps();
            Cursor.Current = Cursors.Default;
        }
    }
    }