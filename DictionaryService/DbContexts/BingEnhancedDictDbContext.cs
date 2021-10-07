using DictionaryService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.DbContexts
{
    public partial class BingEnhancedDictDbContext : DbContext
    {
        public BingEnhancedDictDbContext()
        {
        }

        public BingEnhancedDictDbContext(DbContextOptions<BingEnhancedDictDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dict> Dict { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={EnhancedDictHelper.DatabasePath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dict>(entity =>
            {
                entity.HasKey(e => e.Word);

                entity.ToTable("Dict");

                entity.HasIndex(e => e.Word).HasName("wordIndex");

                entity.Property(e => e.Word).HasColumnName("word");

                entity.Property(e => e.AutoSugg).HasColumnName("autoSugg");

                entity.Property(e => e.Defi).IsRequired();

                entity.Ignore(e => e.Freq);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
