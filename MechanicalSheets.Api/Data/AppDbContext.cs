using Microsoft.EntityFrameworkCore;
using MechanicalSheets.Api.Models;
using MechanicalSheets.Api.Enums;

namespace MechanicalSheets.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Sheet> Sheets => Set<Sheet>();
    public DbSet<SheetDefectItem> SheetDefectItems => Set<SheetDefectItem>();
    public DbSet<DefectCatalog> DefectCatalogs => Set<DefectCatalog>();
    public DbSet<SheetTechnician> SheetTechnicians => Set<SheetTechnician>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

    
        modelBuilder.Entity<Sheet>()
            .HasIndex(s => s.Code)
            .IsUnique();

        modelBuilder.Entity<Sheet>()
            .Property(s => s.SheetStatus)
            .HasConversion<int>(); 

        modelBuilder.Entity<Sheet>()
            .HasOne(s => s.CreatedBy)
            .WithMany(u => u.CreatedSheets)
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Sheet>()
            .HasOne(s => s.ReviewedBy)
            .WithMany(u => u.ReviewedSheets)
            .HasForeignKey(s => s.ReviewedById)
            .OnDelete(DeleteBehavior.SetNull);

       
        modelBuilder.Entity<DefectCatalog>()
            .HasIndex(d => d.Code)
            .IsUnique();

        modelBuilder.Entity<DefectCatalog>()
            .Property(d => d.Category)
            .HasConversion<int>();

   
        modelBuilder.Entity<SheetDefectItem>()
            .HasOne(i => i.Sheet)
            .WithMany(s => s.DefectItems)
            .HasForeignKey(i => i.SheetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SheetDefectItem>()
            .HasOne(i => i.DefectCatalog)
            .WithMany(d => d.DefectItems)
            .HasForeignKey(i => i.DefectCatalogId)
            .OnDelete(DeleteBehavior.Restrict);

     
        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.SheetDefectItem)
            .WithMany(i => i.Attachments)
            .HasForeignKey(a => a.SheetDefectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.UploadedBy)
            .WithMany(u => u.Attachments)
            .HasForeignKey(a => a.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);

       
        modelBuilder.Entity<SheetTechnician>()
            .HasKey(st => new { st.SheetId, st.UserId });

        modelBuilder.Entity<SheetTechnician>()
            .HasOne(st => st.Sheet)
            .WithMany(s => s.Technicians)
            .HasForeignKey(st => st.SheetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SheetTechnician>()
            .HasOne(st => st.User)
            .WithMany(u => u.SheetTechnicians)
            .HasForeignKey(st => st.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}