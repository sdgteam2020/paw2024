﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using swas.DAL;

#nullable disable

namespace swas.DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230731110252_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("swas.DAL.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("IsActive")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("tbl_Types");
                });

            modelBuilder.Entity("swas.DAL.Models.tbl_AttHistory", b =>
                {
                    b.Property<int>("AttId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttId"), 1L, 1);

                    b.Property<int>("ActionId")
                        .HasColumnType("int");

                    b.Property<string>("AttPath")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("DateTimeOfUpdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EditDeleteBy")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("EditDeleteDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("PsmId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.HasKey("AttId");

                    b.ToTable("AttHistory");
                });

            modelBuilder.Entity("swas.DAL.Models.tbl_Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"), 1L, 1);

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<DateTime?>("DateTimeOfUpdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EditDeleteBy")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("EditDeleteDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("PsmId")
                        .HasColumnType("int");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.HasKey("CommentId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("swas.DAL.Models.tbl_mActions", b =>
                {
                    b.Property<int>("ActionsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ActionsId"), 1L, 1);

                    b.Property<string>("Actions")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("DateTimeOfUpdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EditDeleteBy")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("EditDeleteDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.HasKey("ActionsId");

                    b.ToTable("mActions");
                });

            modelBuilder.Entity("swas.DAL.Models.tbl_mStages", b =>
                {
                    b.Property<int>("StagesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StagesId"), 1L, 1);

                    b.Property<DateTime?>("DateTimeOfUpdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EditDeleteBy")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("EditDeleteDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Stages")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.HasKey("StagesId");

                    b.ToTable("mStages");
                });

            modelBuilder.Entity("swas.DAL.Models.tbl_mStakeHolder", b =>
                {
                    b.Property<int>("StakeHolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StakeHolderId"), 1L, 1);

                    b.Property<DateTime?>("DateTimeOfUpdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EditDeleteBy")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("EditDeleteDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("StakeHolder")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.HasKey("StakeHolderId");

                    b.ToTable("mStakeHolder");
                });

            modelBuilder.Entity("swas.DAL.Models.tbl_mStatus", b =>
                {
                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StatusId"), 1L, 1);

                    b.Property<DateTime?>("DateTimeOfUpdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EditDeleteBy")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("EditDeleteDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("StageId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.HasKey("StatusId");

                    b.ToTable("mStatus");
                });

            modelBuilder.Entity("swas.DAL.Models.tbl_Projects", b =>
                {
                    b.Property<int>("ProjId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjId"), 1L, 1);

                    b.Property<DateTime?>("CompletionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CurrentPslmId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateTimeOfUpdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EditDeleteBy")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("EditDeleteDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("InitiatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("IsWhitelisted")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("InitialRemark")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("ProjName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("StakeHolderId")
                        .HasColumnType("int");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.HasKey("ProjId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("swas.DAL.Models.tbl_ProjStakeHolderMov", b =>
                {
                    b.Property<int>("PsmId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PsmId"), 1L, 1);

                    b.Property<int>("ActionId")
                        .HasColumnType("int");

                    b.Property<string>("AddRemarks")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("CommentId")
                        .HasColumnType("int");

                    b.Property<int>("CurrentStakeHolderId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateTimeOfUpdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EditDeleteBy")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("EditDeleteDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("FromStakeHolderId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("ProjId")
                        .HasColumnType("int");

                    b.Property<int>("StageId")
                        .HasColumnType("int");

                    b.Property<int>("StakeHolderId")
                        .HasColumnType("int");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("ToStakeHolderId")
                        .HasColumnType("int");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.HasKey("PsmId");

                    b.ToTable("ProjStakeHolderMov");
                });

            modelBuilder.Entity("swas.DAL.Models.UnitDtl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("unitid");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Command")
                        .HasColumnType("int")
                        .HasColumnName("comdid");

                    b.Property<int>("CorpsId")
                        .HasColumnType("int");

                    b.Property<string>("Loc")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("area_loc");

                    b.Property<bool>("Status")
                        .HasColumnType("bit")
                        .HasColumnName("status");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.Property<string>("UnitName")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("unitname");

                    b.Property<string>("UnitSusNo")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("unitSusNo");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("int")
                        .HasColumnName("updatedby");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("updateddt");

                    b.HasKey("Id");

                    b.ToTable("tbl_mUnitBranch");
                });

            modelBuilder.Entity("swas.DAL.Models.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CmdId")
                        .HasColumnType("int")
                        .HasColumnName("CmdId");

                    b.Property<int>("CorpsId")
                        .HasColumnType("int")
                        .HasColumnName("CorpsId");

                    b.Property<int>("IsActive")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleId");

                    b.Property<int>("UnitId")
                        .HasColumnType("int")
                        .HasColumnName("UnitId");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("updatedBy");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("updateddt");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasColumnName("UserName");

                    b.HasKey("Id");

                    b.ToTable("tbl_users");
                });

            modelBuilder.Entity("swas.DAL.SoftwareType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("TypeOfSoftware")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SoftwareTypes");
                });

            modelBuilder.Entity("ASPNetCoreIdentityCustomFields.Data.ApplicationUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName_IAM")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description_iam")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("domain_iam")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("unit_id")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("ApplicationUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
