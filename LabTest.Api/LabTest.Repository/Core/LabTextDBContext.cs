using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LabTest.Repository.Core
{
    public class LabTextDBContext : IdentityDbContext<ApplicationUser>
    {
        public LabTextDBContext(DbContextOptions<LabTextDBContext> options) : base(options)
        {

        }
    }
}
