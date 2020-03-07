using Microsoft.EntityFrameworkCore;
using PatientsApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientsApp.Data
{
    public class PatientsContext : DbContext
    {
        public PatientsContext(DbContextOptions<PatientsContext> options)
            : base(options)
        { }

        public DbSet<PatientEntity> Patients { get; set; }
    }
}
