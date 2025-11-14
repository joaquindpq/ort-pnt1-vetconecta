using AppMascotas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppMascotas.Context
{
    public class EscuelaDatabaseContext : IdentityDbContext<Veterinaria>
    {
        public EscuelaDatabaseContext(DbContextOptions<EscuelaDatabaseContext> options) 
            : base(options)
        {
        }

        public DbSet<Dueno> Duenos { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Turno> Turnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dueno>()
                .HasOne(d => d.Veterinaria)
                .WithMany(v => v.Duenos)
                .HasForeignKey(d => d.VeterinariaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mascota>()
                .HasOne(m => m.Dueno)
                .WithMany(d => d.Mascotas)
                .HasForeignKey(m => m.DuenoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mascota>()
                .HasOne(m => m.Veterinaria)
                .WithMany(v => v.Mascotas)
                .HasForeignKey(m => m.VeterinariaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Mascota)
                .WithMany(m => m.Turnos)
                .HasForeignKey(t => t.MascotaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Veterinaria)
                .WithMany(v => v.Turnos)
                .HasForeignKey(t => t.VeterinariaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dueno>()
                .HasIndex(d => new { d.Dni, d.VeterinariaId })
                .IsUnique();
        }
    }
}