using bookstore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bookstore.DataAccess.AppContext
{
    public class DataContext: IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
    }
}
