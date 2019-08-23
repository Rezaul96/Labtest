using LabTest.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LabTest.Repository.Core
{
    public class LabTestDbContext : IdentityDbContext<ApplicationUser>
    {
        public LabTestDbContext(DbContextOptions<LabTestDbContext> options) : base(options)
        {

        }
        public DbSet<Models.Registration> Registrations { get; set; }
    }
}
