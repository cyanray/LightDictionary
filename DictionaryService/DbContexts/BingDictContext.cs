using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using System.IO;
using Windows.Storage;
using System.Diagnostics;
using Windows.ApplicationModel;
using DictionaryService.Models;

namespace DictionaryService.DbContexts
{
    public partial class BingDictContext : DbContext
    {
        public BingDictContext()
        {
        }

        public BingDictContext(DbContextOptions<BingDictContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dict> Dict { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = $"Data Source={Path.Combine(Package.Current.InstalledLocation.Path, "BingDict.db")};";
                optionsBuilder.UseSqlite(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dict>(entity =>
            {
                entity.HasKey(e => e.Word);

                entity.HasIndex(e => e.Word).HasName("wordIndex"); ;

                entity.Property(e => e.Word).HasColumnName("word");

                entity.Property(e => e.AutoSugg).HasColumnName("autoSugg");

                entity.Property(e => e.Defi).IsRequired();

                entity.Property(e => e.Freq).HasColumnName("freq");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
