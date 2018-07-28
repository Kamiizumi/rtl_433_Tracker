﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rtl433Tracker.Data;

namespace Rtl433Tracker.Data.Migrations
{
    [DbContext(typeof(Rtl433TrackerContext))]
    partial class Rtl433TrackerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("Rtl433Tracker.Data.Models.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DriverId");

                    b.Property<string>("DriverModel")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Rtl433Tracker.Data.Models.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("DeviceId");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Rtl433Tracker.Data.Models.EventData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("EventId");

                    b.Property<string>("Property")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("EventData");
                });

            modelBuilder.Entity("Rtl433Tracker.Data.Models.Event", b =>
                {
                    b.HasOne("Rtl433Tracker.Data.Models.Device", "Device")
                        .WithMany("Events")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Rtl433Tracker.Data.Models.EventData", b =>
                {
                    b.HasOne("Rtl433Tracker.Data.Models.Event", "Event")
                        .WithMany("Data")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
