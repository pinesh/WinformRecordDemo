using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;

namespace MoodMe_NETDemo
{
    public class EntityDatabase
    {
        public class recordings
        {
            [Key]
            public long id { get; set; }
            [Required()]
            public string video { get; set; }
            [Required()]
            public string tag { get; set; }

            [NotMapped]
            public string creationtime => DateTime.FromFileTimeUtc(id).ToLocalTime().ToString();
        }

        public class RecordingContext : DbContext
        {
            public RecordingContext(string s) :
                base(new SQLiteConnection
                {
                    ConnectionString = new SQLiteConnectionStringBuilder { DataSource = s }.ConnectionString
                }, true)
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
                base.OnModelCreating(modelBuilder);
            }

            public DbSet<EntityDatabase.recordings> Recordings { get; set; }
        }


    }
}