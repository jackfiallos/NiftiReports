using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Reports.Models;

namespace Reports.DAL
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Param> Params { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuider)
        {
            base.OnModelCreating(modelBuider);
        }
    }
}
