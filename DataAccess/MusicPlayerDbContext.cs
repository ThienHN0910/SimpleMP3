using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SimpleMP3.Models;

namespace SimpleMP3.DataAccess;

public partial class MusicPlayerDbContext : DbContext
{
    public MusicPlayerDbContext()
    {
    }

    public MusicPlayerDbContext(DbContextOptions<MusicPlayerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<PlayHistory> PlayHistories { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Artists__3214EC07EEE3887B");

            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<PlayHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlayHist__3214EC0756A4AFE2");

            entity.ToTable("PlayHistory");

            entity.Property(e => e.PlayedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Track).WithMany(p => p.PlayHistories)
                .HasForeignKey(d => d.TrackId)
                .HasConstraintName("FK__PlayHisto__Track__3A81B327");

            entity.HasOne(d => d.User).WithMany(p => p.PlayHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__PlayHisto__UserI__398D8EEE");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Playlist__3214EC0718D81803");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Playlists__UserI__31EC6D26");

            entity.HasMany(d => d.Tracks).WithMany(p => p.Playlists)
                .UsingEntity<Dictionary<string, object>>(
                    "PlaylistTrack",
                    r => r.HasOne<Track>().WithMany()
                        .HasForeignKey("TrackId")
                        .HasConstraintName("FK__PlaylistT__Track__35BCFE0A"),
                    l => l.HasOne<Playlist>().WithMany()
                        .HasForeignKey("PlaylistId")
                        .HasConstraintName("FK__PlaylistT__Playl__34C8D9D1"),
                    j =>
                    {
                        j.HasKey("PlaylistId", "TrackId").HasName("PK__Playlist__A4A6282E7AC592C2");
                        j.ToTable("PlaylistTracks");
                    });
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tracks__3214EC0748080EE4");

            entity.Property(e => e.Album).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.YouTubeId).HasMaxLength(20);

            entity.HasOne(d => d.Artist).WithMany(p => p.Tracks)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Tracks__ArtistId__2D27B809");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07C99FB432");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E47523E6B7").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105349930AEEC").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
