using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LeadRetrieve.Models;

public partial class LeadAdContext : DbContext
{
    public LeadAdContext()
    {
    }

    public LeadAdContext(DbContextOptions<LeadAdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<Leadfielddata> Leadfielddata { get; set; }

    public virtual DbSet<Paginginfo> Paginginfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("leads_pkey");

            entity.ToTable("leads");

            entity.HasIndex(e => e.LeadId, "leads_lead_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_time");
            entity.Property(e => e.JsonResponse).HasColumnName("json_response");
            entity.Property(e => e.LeadId)
                .HasMaxLength(255)
                .HasColumnName("lead_id");
        });

        modelBuilder.Entity<Leadfielddata>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("leadfielddata_pkey");

            entity.ToTable("leadfielddata");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FieldName)
                .HasMaxLength(255)
                .HasColumnName("field_name");
            entity.Property(e => e.FieldValue).HasColumnName("field_value");
            entity.Property(e => e.LeadId)
                .HasMaxLength(255)
                .HasColumnName("lead_id");

            entity.HasOne(d => d.Lead).WithMany(p => p.Leadfielddata)
                .HasPrincipalKey(p => p.LeadId)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("leadfielddata_lead_id_fkey");
        });

        modelBuilder.Entity<Paginginfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("paginginfo_pkey");

            entity.ToTable("paginginfo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CursorAfter)
                .HasMaxLength(255)
                .HasColumnName("cursor_after");
            entity.Property(e => e.CursorBefore)
                .HasMaxLength(255)
                .HasColumnName("cursor_before");
            entity.Property(e => e.LeadId)
                .HasMaxLength(255)
                .HasColumnName("lead_id");

            entity.HasOne(d => d.Lead).WithMany(p => p.Paginginfos)
                .HasPrincipalKey(p => p.LeadId)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("paginginfo_lead_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
