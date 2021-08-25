using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Registration.Api.Models
{
    public class RegistrationContext : DbContext
    {
            public RegistrationContext(DbContextOptions options) : base(options) { }

            public DbSet<User> Users { get; set; }
    }
}
