using Microsoft.EntityFrameworkCore;
using System;

#nullable disable

namespace Database.Model.Context
{
    public partial class DatabaseContext : DbContext
    {
        private string ConnnectionString { get; set; }
        public DatabaseContext(string connectionStrinng)
        {
            ConnnectionString = connectionStrinng;
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<AppointmentStep> AppointmentSteps { get; set; }
        public virtual DbSet<AppointmentStepFile> AppointmentStepFiles { get; set; }
        public virtual DbSet<FileContent> FileContents { get; set; }
        public virtual DbSet<FileDetail> FileDetails { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServicesStep> ServicesSteps { get; set; }
        public virtual DbSet<Step> Steps { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(this.ConnnectionString, ServerVersion.AutoDetect(this.ConnnectionString));
            }
            // TODO: remove on production
            optionsBuilder.LogTo(Console.WriteLine);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("appointments_client");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("appointments_service");
            });

            modelBuilder.Entity<AppointmentStep>(entity =>
            {
                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.AppointmentSteps)
                    .HasForeignKey(d => d.AppointmentId)
                    .HasConstraintName("appointment_steps_appointment");

                entity.HasOne(d => d.Step)
                    .WithMany(p => p.AppointmentSteps)
                    .HasForeignKey(d => d.StepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("appointment_steps_steps");
            });

            modelBuilder.Entity<AppointmentStepFile>(entity =>
            {
                entity.HasOne(d => d.AppointmentStep)
                    .WithMany(p => p.AppointmentStepFiles)
                    .HasForeignKey(d => d.AppointmentStepId)
                    .HasConstraintName("appointment_step_files_appointment");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.AppointmentStepFiles)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("appointment_step_files_file");
            });

            modelBuilder.Entity<FileContent>(entity =>
            {
                entity.Property(e => e.FileGuid).IsFixedLength(true);
            });

            modelBuilder.Entity<FileDetail>(entity =>
            {
                entity.Property(e => e.Guid).IsFixedLength(true);

                entity.HasOne(d => d.Content)
                    .WithMany(p => p.FileDetails)
                    .HasForeignKey(d => d.ContentId)
                    .HasConstraintName("files_details_content");

                entity.HasOne(d => d.Uploader)
                    .WithMany(p => p.FileDetails)
                    .HasForeignKey(d => d.UploaderId)
                    .HasConstraintName("files_user");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasOne(d => d.Appointment)
                    .WithOne(p => p.Rating)
                    .HasForeignKey<Rating>(d => d.AppointmentId)
                    .HasConstraintName("ratings_appointment");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasOne(d => d.Picture)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.PictureId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("service_picture");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_service");
            });

            modelBuilder.Entity<ServicesStep>(entity =>
            {
                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServicesSteps)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("services_steps_service");

                entity.HasOne(d => d.Step)
                    .WithMany(p => p.ServicesSteps)
                    .HasForeignKey(d => d.StepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("services_steps_steps");
            });

            modelBuilder.Entity<Step>(entity =>
            {
                entity.Property(e => e.TargetUser).HasDefaultValueSql("'client'");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Steps)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("steps_user");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.EmailConfirmationCode).IsFixedLength(true);

                entity.HasOne(d => d.ProfilePicture)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ProfilePictureId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("user_profile_picture");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
