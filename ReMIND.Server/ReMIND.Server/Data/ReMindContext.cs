using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReMIND.Server.Data
{
    public class ReMindContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobArchive> JobArchives { get; set; }
        public DbSet<JobTag> JobTags { get; set; }
        public DbSet<JobGroup> JobGroups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamLink> TeamLinks { get; set; }

        public Person caller;

        public ReMindContext(DbContextOptions options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            //TEAMLINK
            modelBuilder.Entity<TeamLink>().HasOne( tl => tl.Person).WithMany( t => t.TeamLink );
            modelBuilder.Entity<TeamLink>().HasOne( tl => tl.Team).WithMany( t => t.TeamLink );

            //JOBGROUP
            modelBuilder.Entity<JobGroup>().HasOne( tg => tg.Team).WithMany( t => t.JobGroup);

            //JOB
            modelBuilder.Entity<Job>().HasOne( j => j.JobTag).WithMany( jt => jt.Job );
            modelBuilder.Entity<Job>().HasOne( j => j.Team).WithMany( t => t.Job );
            modelBuilder.Entity<Job>().HasOne( j => j.JobGroup).WithMany( tg => tg.Job );

            //JOBARCHIVE
            modelBuilder.Entity<JobArchive>().HasOne( j => j.Person).WithMany( p => p.JobArchive);

            //PERSON
            modelBuilder.Entity<Person>().HasMany( p => p.Job).WithOne( j => j.Person);


        }
    }
}
