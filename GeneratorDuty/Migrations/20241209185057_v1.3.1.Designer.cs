﻿// <auto-generated />
using System;
using GeneratorDuty.Repository.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GeneratorDuty.Migrations
{
    [DbContext(typeof(DutyContext))]
    [Migration("20241209185057_v1.3.1")]
    partial class v131
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("GeneratorDuty.Models.Duty.LogDutyMember", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("LogDutyMembers");
                });

            modelBuilder.Entity("GeneratorDuty.Models.Duty.LogDutyMemberPriority", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("LogDutyMemberPriorities");
                });

            modelBuilder.Entity("GeneratorDuty.Models.Duty.MemberDuty", b =>
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

            modelBuilder.Entity("GeneratorDuty.Models.Properties.ScheduleProp", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Fails")
                        .HasColumnType("INTEGER");

                    b.Property<long>("IdPeer")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAutoExport")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAutoSend")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMigrated")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRequiredAdminRights")
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

            modelBuilder.Entity("GeneratorDuty.Models.Schedule.MessageWidget", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MessageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("MessageWidgets");
                });

            modelBuilder.Entity("GeneratorDuty.Models.Schedule.ScheduleCustomRules", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CallType")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ShowImportantLesson")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowRussianHorizont")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ScheduleCustomRules");
                });

            modelBuilder.Entity("GeneratorDuty.Models.Schedule.ScheduleHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<long?>("IdPeer")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPinned")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MessageId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("History");
                });

            modelBuilder.Entity("GeneratorDuty.Models.Duty.LogDutyMember", b =>
                {
                    b.HasOne("GeneratorDuty.Models.Duty.MemberDuty", "Duty")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Duty");
                });

            modelBuilder.Entity("GeneratorDuty.Models.Duty.LogDutyMemberPriority", b =>
                {
                    b.HasOne("GeneratorDuty.Models.Duty.MemberDuty", "Duty")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Duty");
                });
#pragma warning restore 612, 618
        }
    }
}