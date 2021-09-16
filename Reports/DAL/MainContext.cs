using Microsoft.EntityFrameworkCore;
using Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reports.DAL
{
    public class MainContext: DbContext
    {
        public DbSet<SomeModel> SomeModels { get; set; }

        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SomeModel>().HasNoKey();
        }
    }
}
