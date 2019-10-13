using bookstore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bookstore.DataAccess.AppContext
{
    public class AppContext: IdentityDbContext<AppUser>
    {
        public AppContext(DbContextOptions<AppContext> options): base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
    }
}
