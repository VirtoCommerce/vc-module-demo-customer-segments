﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Repositories;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Migrations
{
    [DbContext(typeof(DemoCustomerSegmentDbContext))]
    [Migration("20201203122551_AddUserGroupFieldToCustomerSegments")]
    partial class AddUserGroupFieldToCustomerSegments
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.DemoCustomerSegmentsModule.Data.Models.DemoCustomerSegmentEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(1024)")
                        .HasMaxLength(1024);

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExpressionTreeSerialized")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserGroup")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DemoCustomerSegments");
                });

            modelBuilder.Entity("VirtoCommerce.DemoCustomerSegmentsModule.Data.Models.DemoCustomerSegmentStoreEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("CustomerSegmentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("StoreId")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("CustomerSegmentId");

                    b.HasIndex("StoreId");

                    b.ToTable("DemoCustomerSegmentStore");
                });

            modelBuilder.Entity("VirtoCommerce.DemoCustomerSegmentsModule.Data.Models.DemoCustomerSegmentStoreEntity", b =>
                {
                    b.HasOne("VirtoCommerce.DemoCustomerSegmentsModule.Data.Models.DemoCustomerSegmentEntity", "CustomerSegment")
                        .WithMany("Stores")
                        .HasForeignKey("CustomerSegmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
