using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Курс.Models
{
    public partial class ComputerRepairShopContext : DbContext
    {
        public ComputerRepairShopContext()
        {
        }

        public ComputerRepairShopContext(DbContextOptions<ComputerRepairShopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Request> Requests { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["ConnectionLocalDb"].ToString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>(entity =>
            {
                entity.Property(e => e.RequestId).HasColumnName("REQUEST_ID");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("date")
                    .HasColumnName("CREATION_DATE");

                entity.Property(e => e.LastChangeDate)
                    .HasColumnType("date")
                    .HasColumnName("LAST_CHANGE_DATE");

                entity.Property(e => e.ProblemDescription)
                    .HasMaxLength(255)
                    .HasColumnName("PROBLEM_DESCRIPTION");

                entity.Property(e => e.Status).HasColumnName("STATUS");

                entity.Property(e => e.TroubleDevices)
                    .HasMaxLength(255)
                    .HasColumnName("TROUBLE_DEVICES");

                entity.Property(e => e.UserId).HasColumnName("USER_ID");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Requests_ToStates");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Requests_ToUsers");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("ROLE_ID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(100)
                    .HasColumnName("ROLE_NAME");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__tmp_ms_x__D8827E714F1CA802");

                entity.Property(e => e.StatusId)
                    .ValueGeneratedNever()
                    .HasColumnName("STATUS_ID");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS_NAME");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("USER_ID");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Login)
                    .HasMaxLength(255)
                    .HasColumnName("LOGIN");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("NAME");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.Role).HasColumnName("ROLE");

                entity.Property(e => e.Surname)
                    .HasMaxLength(255)
                    .HasColumnName("SURNAME");

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_ToRoles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
