using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirWatcher.Models;
using DirWatcherNew.Models;
using Microsoft.EntityFrameworkCore;

namespace DirWatcher.Controllers
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskRunDetail> TaskRunDetails { get; set; }
        public DbSet<FileChange> FileChanges { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("ConnectionStrings");
            }
        }
    }
}