using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoterRegistrationSystem.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base(DatabaseConnection.ConnectionString()) {}
        public DbSet<Voter> Voters { get; set; }
        public DbSet<Fingerprint> Fingerprints { get; set; }
        public DbSet<Lga> LGAs { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<PollingUnit> PollingUnits { get; set; }
        public DbSet<MaritalStatus> MaritalStatus { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<RegistrationOfficer> RegistrationOfficers { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure StudentId as PK for StudentAddress
            modelBuilder.Entity<Fingerprint>().HasKey(r => r.VoterID);
            modelBuilder.Entity<State>().HasMany<Lga>(r => r.LGAs).WithRequired(r => r.State).HasForeignKey(r => r.StateID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Lga>().HasMany<Ward>(r => r.Wards).WithRequired(r => r.Lga).HasForeignKey(r => r.LgaID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Ward>().HasMany<PollingUnit>(r => r.PollingUnits).WithRequired(r => r.Ward).HasForeignKey(r => r.WardID).WillCascadeOnDelete(false);

            // Configure StudentId as FK for StudentAddress
            modelBuilder.Entity<Voter>().HasOptional(s => s.VoterFingerprint).WithRequired(ad => ad.Voter); // Create inverse relationship

        }
    }

    public class DatabaseContextInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext> {
        protected override void Seed(DatabaseContext context)
        {
            context.Sexes.Add(new Sex() { SexTitle = "Male" });
            context.Sexes.Add(new Sex() { SexTitle = "Female" });

            context.MaritalStatus.Add(new MaritalStatus() { StatusTitle = "Single" });
            context.MaritalStatus.Add(new MaritalStatus() { StatusTitle = "Married" });

            context.States.Add(new State() { StateName = "Niger" });

            base.Seed(context);
        }
    }
}
