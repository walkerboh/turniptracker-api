﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TurnipTallyApi.Database;

namespace TurnipTallyApi.Migrations
{
    [DbContext(typeof(TurnipContext))]
    [Migration("20200712191515_passwordResetUpdate")]
    partial class passwordResetUpdate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.Board", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<string>("EditKey")
                        .HasColumnType("text");

                    b.Property<long>("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<bool>("Private")
                        .HasColumnType("boolean");

                    b.Property<string>("UrlName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.BoardUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("BoardId")
                        .HasColumnType("bigint");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("RegisteredUserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.HasIndex("RegisteredUserId");

                    b.ToTable("BoardUsers");
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.PasswordReset", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("RegisteredUserId")
                        .HasColumnType("bigint");

                    b.HasKey("Key");

                    b.ToTable("PasswordResets");
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.Record", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("WeekDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Day")
                        .HasColumnType("text");

                    b.Property<string>("Period")
                        .HasColumnType("text");

                    b.Property<int?>("SellPrice")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "WeekDate", "Day", "Period");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.RegisteredUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("bytea");

                    b.Property<string>("TimezoneId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RegisteredUsers");
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.Timezone", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Timezones");
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.Week", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("WeekDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("BuyPrice")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "WeekDate");

                    b.ToTable("Weeks");
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.Board", b =>
                {
                    b.HasOne("TurnipTallyApi.Database.Entities.RegisteredUser", "Owner")
                        .WithMany("OwnedBoards")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.BoardUser", b =>
                {
                    b.HasOne("TurnipTallyApi.Database.Entities.Board", "Board")
                        .WithMany("Users")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TurnipTallyApi.Database.Entities.RegisteredUser", "RegisteredUser")
                        .WithMany("BoardUsers")
                        .HasForeignKey("RegisteredUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.Record", b =>
                {
                    b.HasOne("TurnipTallyApi.Database.Entities.Week", "Week")
                        .WithMany("Records")
                        .HasForeignKey("UserId", "WeekDate")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TurnipTallyApi.Database.Entities.Week", b =>
                {
                    b.HasOne("TurnipTallyApi.Database.Entities.RegisteredUser", "User")
                        .WithMany("Weeks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
