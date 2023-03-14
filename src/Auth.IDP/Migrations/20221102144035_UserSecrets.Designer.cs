﻿// <auto-generated />
using System;
using Auth.IDP.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Auth.IDP.Migrations
{
    [DbContext(typeof(IdentityDbContext))]
    [Migration("20221102144035_UserSecrets")]
    partial class UserSecrets
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("Auth.IDP.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SecureCodeExpirationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("SecurityCode")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Subject")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                            Active = true,
                            ConcurrencyStamp = "7afb613b-43f5-413c-b60f-7ff4d9bfcf7e",
                            Email = "david@mail.com",
                            Password = "AQAAAAEAACcQAAAAEKFzpqbJZztQiEoUlWok01l543K6ur7CFyD3hzbrg4BL1fsBuWzHW4WkoygQz6y3gQ==",
                            SecureCodeExpirationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Subject = "d860efca-22d9-47fd-8249-791ba61b07c7",
                            UserName = "David"
                        },
                        new
                        {
                            Id = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                            Active = true,
                            ConcurrencyStamp = "0e308590-a948-494b-94fa-9d32f15fb156",
                            Email = "emma@mail.com",
                            Password = "AQAAAAEAACcQAAAAEKFzpqbJZztQiEoUlWok01l543K6ur7CFyD3hzbrg4BL1fsBuWzHW4WkoygQz6y3gQ==",
                            SecureCodeExpirationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Subject = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                            UserName = "Emma"
                        });
                });

            modelBuilder.Entity("Auth.IDP.Entities.UserClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");

                    b.HasData(
                        new
                        {
                            Id = new Guid("04203832-9abc-425c-a337-9f1fe83579f9"),
                            ConcurrencyStamp = "34f4f981-5b36-497e-9066-94d539226a56",
                            Type = "given_name",
                            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                            Value = "David"
                        },
                        new
                        {
                            Id = new Guid("13e85232-3c19-416b-b544-e958a85fc048"),
                            ConcurrencyStamp = "2d29e589-2d39-4e09-b333-a4b183129d97",
                            Type = "family_name",
                            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                            Value = "Flagg"
                        },
                        new
                        {
                            Id = new Guid("b7bf8ab7-1a0c-4a69-8f08-771f962b5b47"),
                            ConcurrencyStamp = "59f7cfef-b9a8-4a51-bc74-95e91e73bcd8",
                            Type = "country",
                            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                            Value = "nl"
                        },
                        new
                        {
                            Id = new Guid("08efeca9-d36a-4eac-9bbd-7b6cd2115762"),
                            ConcurrencyStamp = "d9588d82-a278-4b36-bbe3-7669626182aa",
                            Type = "role",
                            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                            Value = "FreeUser"
                        },
                        new
                        {
                            Id = new Guid("3bab97c0-ff82-48f4-b621-13d1553118d0"),
                            ConcurrencyStamp = "aa00ad1f-36aa-436f-b4fe-001685be68de",
                            Type = "given_name",
                            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                            Value = "Emma"
                        },
                        new
                        {
                            Id = new Guid("463c6136-bca5-462d-a570-b813d098fd6d"),
                            ConcurrencyStamp = "eea02f28-d6e0-40e9-99f4-378957e7b2cd",
                            Type = "family_name",
                            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                            Value = "Flagg"
                        },
                        new
                        {
                            Id = new Guid("bdab5fcc-89fb-49fa-b082-8f8b66088806"),
                            ConcurrencyStamp = "e1f5b9fa-3616-4867-bb35-dd65c4d63276",
                            Type = "country",
                            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                            Value = "be"
                        },
                        new
                        {
                            Id = new Guid("3f4b7f8e-026c-46a7-9d8f-af7743218325"),
                            ConcurrencyStamp = "928461e1-0c03-4dae-a39e-26ebac8eb550",
                            Type = "role",
                            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                            Value = "PayingUser"
                        });
                });

            modelBuilder.Entity("Auth.IDP.Entities.UserLogin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderIdentityKey")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Auth.IDP.Entities.UserSecret", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSecrets");
                });

            modelBuilder.Entity("Auth.IDP.Entities.UserClaim", b =>
                {
                    b.HasOne("Auth.IDP.Entities.User", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Auth.IDP.Entities.UserLogin", b =>
                {
                    b.HasOne("Auth.IDP.Entities.User", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Auth.IDP.Entities.UserSecret", b =>
                {
                    b.HasOne("Auth.IDP.Entities.User", "User")
                        .WithMany("UserSecrets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Auth.IDP.Entities.User", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Logins");

                    b.Navigation("UserSecrets");
                });
#pragma warning restore 612, 618
        }
    }
}
