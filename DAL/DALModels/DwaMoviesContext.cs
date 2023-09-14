using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.DALModels;

public partial class DwaMoviesContext : DbContext
{
    public DwaMoviesContext()
    {
    }

    public DwaMoviesContext(DbContextOptions<DwaMoviesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    public virtual DbSet<VideoTag> VideoTags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("server=.;Database=DwaMovies;User Id=sas;Password=SQL;TrustServerCertificate=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.Code).IsFixedLength();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasOne(d => d.CountryOfResidence).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Country");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Genre).WithMany(p => p.Videos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Video_Genre");

            entity.HasOne(d => d.Image).WithMany(p => p.Videos).HasConstraintName("FK_Video_Images");
        });

        modelBuilder.Entity<VideoTag>(entity =>
        {
            entity.HasOne(d => d.Tag).WithMany(p => p.VideoTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VideoTag_Tag");

            entity.HasOne(d => d.Video).WithMany(p => p.VideoTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VideoTag_Video");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
