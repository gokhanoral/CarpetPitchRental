using CarpetPitchRental_EL.IdentityModels;
using CarpetPitchRental_EL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_DAL
{
    public class MyContext:IdentityDbContext<AppUser,AppRole,string>
    {
        public MyContext(DbContextOptions<MyContext> options)
            :base(options)
        {

        }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Facility> Facilities { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<ReservationDate> ReservationDates { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Reservation ile ReservationDate arasında 1'e 1 bağlantı kurduk ve ReservationDate tablosuna Reservation tablosunun Id'sini verdik.Bu yüzden Reservation tablosundan bir column sildiğimizde ReservationDate tablosunda bu column'a ait Id kalacak ve karşılığı olmayacak.
            //Bu sebeple Introducing FOREIGN KEY constraint 'FK_ReservationDates_Reservations_ReservationId' on table 'ReservationDates' may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints. Could not create constraint or index. See previous errors. hatasını aldık.
            //Bu hatanın çözümü için aşağıdaki kod bloğunu yazdık.
                 modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ReservationDate)
                .WithOne(i => i.Reservation)
                .HasForeignKey<ReservationDate>(r => r.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

