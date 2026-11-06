using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class CinemaStudioContext : DbContext
{
    public CinemaStudioContext()
    {
    }

    public CinemaStudioContext(DbContextOptions<CinemaStudioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<FilmGroup> FilmGroups { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<GenresInProject> GenresInProjects { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<ShootingSchedule> ShootingSchedules { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<StatusProject> StatusProjects { get; set; }

    public virtual DbSet<TypeArea> TypeAreas { get; set; }

    public virtual DbSet<TypeExpense> TypeExpenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Достаем готовую строку из appsettings.json
            string connectionString = App.Configuration.GetConnectionString("DefaultConnection");

            // Передаем её в PostgreSQL
            optionsBuilder.UseNpgsql(connectionString);
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("areas_pkey");

            entity.ToTable("areas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.CapacityArea).HasColumnName("capacity_area");
            entity.Property(e => e.CostRentalInDay).HasColumnName("cost_rental_in_day");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Type).WithMany(p => p.Areas)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("areas_type_id_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.HasIndex(e => e.Login, "employees_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Fees).HasColumnName("fees");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(100)
                .HasColumnName("patronymic");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Surname)
                .HasMaxLength(100)
                .HasColumnName("surname");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Employees)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("employees_specialization_id_fkey");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("employees_status_id_fkey");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("expenses_pkey");

            entity.ToTable("expenses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Sum).HasColumnName("sum");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Project).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("expenses_project_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("expenses_type_id_fkey");
        });

        modelBuilder.Entity<FilmGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("film_groups_pkey");

            entity.ToTable("film_groups");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RoleInProject)
                .HasMaxLength(255)
                .HasColumnName("role_in_project");

            entity.HasOne(d => d.Employee).WithMany(p => p.FilmGroups)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("film_groups_employee_id_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.FilmGroups)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("film_groups_project_id_fkey");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genres_pkey");

            entity.ToTable("genres");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<GenresInProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genres_in_projects_pkey");

            entity.ToTable("genres_in_projects");

            entity.HasIndex(e => new { e.ProjectId, e.GenreId }, "uq_project_genre").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");

            entity.HasOne(d => d.Genre).WithMany(p => p.GenresInProjects)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("genres_in_projects_genre_id_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.GenresInProjects)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("genres_in_projects_project_id_fkey");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projects_pkey");

            entity.ToTable("projects");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Budget).HasColumnName("budget");
            entity.Property(e => e.DateEnd).HasColumnName("date_end");
            entity.Property(e => e.DateStart).HasColumnName("date_start");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TargetAudience)
                .HasMaxLength(255)
                .HasColumnName("target_audience");

            entity.HasOne(d => d.Status).WithMany(p => p.Projects)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("projects_status_id_fkey");
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rentals_pkey");

            entity.ToTable("rentals");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountCinema).HasColumnName("count_cinema");
            entity.Property(e => e.DateExit).HasColumnName("date_exit");
            entity.Property(e => e.Fees).HasColumnName("fees");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.Project).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("rentals_project_id_fkey");
        });

        modelBuilder.Entity<ShootingSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shooting_schedules_pkey");

            entity.ToTable("shooting_schedules");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AreaId).HasColumnName("area_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Scene)
                .HasMaxLength(255)
                .HasColumnName("scene");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasOne(d => d.Area).WithMany(p => p.ShootingSchedules)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("shooting_schedules_area_id_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.ShootingSchedules)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("shooting_schedules_project_id_fkey");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("specializations_pkey");

            entity.ToTable("specializations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(120)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StatusProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_projects_pkey");

            entity.ToTable("status_projects");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TypeArea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("type_areas_pkey");

            entity.ToTable("type_areas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TypeExpense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("type_expenses_pkey");

            entity.ToTable("type_expenses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
