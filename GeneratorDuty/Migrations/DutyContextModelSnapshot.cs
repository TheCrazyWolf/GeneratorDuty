﻿// <auto-generated />
using GeneratorDuty.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GeneratorDuty.Migrations
{
    [DbContext(typeof(DutyContext))]
    partial class DutyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("GeneratorDuty.Models.MemberDuty", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("IdPeer")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MemberNameDuty")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MemberDuties");
                });

            modelBuilder.Entity("GeneratorDuty.Models.ScheduleProp", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("IdPeer")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAutoSend")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastResult")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SearchType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ScheduleProps");
                });
#pragma warning restore 612, 618
        }
    }
}
