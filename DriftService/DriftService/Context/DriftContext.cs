using DriftService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DriftService.Context
{
    public class DriftContext :DbContext
        
    {
        public DriftContext() : base("DriftServiceDb")
        {
            Database.SetInitializer<DriftContext>(new CreateDatabaseIfNotExists<DriftContext>());
        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<ContactServiceType> ContactServiceTypes { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}