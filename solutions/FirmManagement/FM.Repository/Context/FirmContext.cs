using FM.Domain.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace FM.Repository.Context;

public partial class FirmContext : DbContext
{
    public FirmContext()
    {
    }

    public FirmContext(DbContextOptions<FirmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FCategory> FCategories { get; set; }

    public virtual DbSet<FCategoryType> FCategoryTypes { get; set; }

    public virtual DbSet<FCity> FCities { get; set; }

    public virtual DbSet<FClient> FClients { get; set; }

    public virtual DbSet<FIntervention> FInterventions { get; set; }

    public virtual DbSet<FInvoice> FInvoices { get; set; }

    public virtual DbSet<FPurchase> FPurchases { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FCategory>(entity =>
        {
            entity.HasKey(e => e.CaId).HasName("f_category_pkey");

            entity.HasOne(d => d.CaFkCategoryType).WithMany(p => p.FCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("f_category_ca_fk_category_type_id_fkey");
        });

        modelBuilder.Entity<FCategoryType>(entity =>
        {
            entity.HasKey(e => e.CtId).HasName("f_category_type_pkey");
        });

        modelBuilder.Entity<FCity>(entity =>
        {
            entity.HasKey(e => e.CiId).HasName("f_city_pkey");
        });

        modelBuilder.Entity<FClient>(entity =>
        {
            entity.HasKey(e => e.CId).HasName("f_client_pkey");

            entity.HasOne(d => d.CFkBirthCity).WithMany(p => p.FClientCFkBirthCities).HasConstraintName("f_client_c_fk_birth_city_id_fkey");

            entity.HasOne(d => d.CFkCity).WithMany(p => p.FClientCFkCities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("f_client_c_fk_city_id_fkey");

            entity.HasOne(d => d.CFkMedia).WithMany(p => p.FClients).HasConstraintName("f_client_c_fk_media_id_fkey");
        });

        modelBuilder.Entity<FIntervention>(entity =>
        {
            entity.HasKey(e => e.IId).HasName("f_intervention_pkey");

            entity.HasOne(d => d.IFkCategory).WithMany(p => p.FInterventions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("f_intervention_i_fk_category_id_fkey");

            entity.HasOne(d => d.IFkClient).WithMany(p => p.FInterventions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("f_intervention_i_fk_client_id_fkey");

            entity.HasOne(d => d.IFkInvoice).WithMany(p => p.FInterventions).HasConstraintName("f_intervention_i_fk_invoice_id_fkey");
        });

        modelBuilder.Entity<FInvoice>(entity =>
        {
            entity.HasKey(e => e.InId).HasName("f_invoice_pkey");

            entity.HasOne(d => d.InFkPayment).WithMany(p => p.FInvoices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("f_invoice_in_fk_payment_id_fkey");
        });

        modelBuilder.Entity<FPurchase>(entity =>
        {
            entity.HasKey(e => e.PId).HasName("f_purchase_pkey");

            entity.HasOne(d => d.PFkCategory).WithMany(p => p.FPurchasePFkCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("f_purchase_p_fk_category_id_fkey");

            entity.HasOne(d => d.PFkPayment).WithMany(p => p.FPurchasePFkPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("f_purchase_p_fk_payment_id_fkey");

            entity.HasOne(d => d.PFkSupplier).WithMany(p => p.FPurchasePFkSuppliers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("f_purchase_p_fk_supplier_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
