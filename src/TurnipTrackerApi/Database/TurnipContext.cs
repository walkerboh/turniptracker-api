using System;
using Microsoft.EntityFrameworkCore;
using TurnipTallyApi.Database.Entities;

namespace TurnipTallyApi.Database
{
    public class TurnipContext : DbContext
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<BoardUser> BoardUsers { get; set; }
        public DbSet<Week> Weeks { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<RegisteredUser> RegisteredUsers { get; set; }

        public TurnipContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Board>(board =>
            {
                board.HasKey(b => b.Id);
                board.HasMany(b => b.Users).WithOne(u => u.Board).HasForeignKey(u=>u.BoardId).IsRequired();
                board.HasOne(b => b.Owner).WithMany(u => u.OwnedBoards).HasForeignKey(b => b.OwnerId).IsRequired();
            });

            modelBuilder.Entity<BoardUser>(user =>
            {
                user.HasKey(c => c.Id);
                user.Property(c => c.Id).ValueGeneratedOnAdd();
                user.HasOne(bu => bu.RegisteredUser).WithMany(ru => ru.BoardUsers)
                    .HasForeignKey(bu => bu.RegisteredUserId);
            });

            modelBuilder.Entity<Week>(week =>
            {
                week.HasKey(w => new
                {
                    w.UserId, w.WeekDate
                });
                week.HasMany(w => w.Records).WithOne(r => r.Week).HasForeignKey(r => new {r.UserId, r.WeekDate}).IsRequired();
            });

            modelBuilder.Entity<Record>(record => 
            { 
                record.HasKey(r => new {r.UserId, r.WeekDate, r.Day, r.Period});
                record.Property(r => r.Day)
                    .HasConversion(d => d.ToString(), d => (DayOfWeek) Enum.Parse(typeof(DayOfWeek), d));
                record.Property(r => r.Period)
                    .HasConversion(p => p.ToString(), p => (Period) Enum.Parse(typeof(Period), p));
            });

            modelBuilder.Entity<RegisteredUser>(user =>
            {
                user.HasKey(u => u.Id);
                user.Property(u => u.Id).ValueGeneratedOnAdd();
                user.HasMany(u => u.Weeks).WithOne(w => w.User).HasForeignKey(w => w.UserId).IsRequired();
            });
        }
    }
}