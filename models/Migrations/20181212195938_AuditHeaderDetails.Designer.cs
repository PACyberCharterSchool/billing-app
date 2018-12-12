﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using models;

namespace models.Migrations
{
    [DbContext(typeof(PacBillContext))]
    [Migration("20181212195938_AuditHeaderDetails")]
    partial class AuditHeaderDetails
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rc1-32029")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("models.AuditDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Field")
                        .IsRequired();

                    b.Property<int?>("HeaderId");

                    b.Property<string>("Next");

                    b.Property<string>("Previous");

                    b.HasKey("Id");

                    b.HasIndex("HeaderId");

                    b.ToTable("AuditDetails");
                });

            modelBuilder.Entity("models.AuditHeader", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Activity")
                        .IsRequired();

                    b.Property<string>("Identifier")
                        .IsRequired();

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("AuditHeaders");
                });

            modelBuilder.Entity("models.AuditRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Activity")
                        .IsRequired();

                    b.Property<string>("Field");

                    b.Property<string>("Identifier");

                    b.Property<string>("Next");

                    b.Property<string>("Previous");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("AuditRecords");
                });

            modelBuilder.Entity("models.Calendar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("SchoolYear");

                    b.HasKey("Id");

                    b.HasIndex("SchoolYear")
                        .IsUnique()
                        .HasFilter("[SchoolYear] IS NOT NULL");

                    b.ToTable("Calendars");
                });

            modelBuilder.Entity("models.CalendarDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CalendarId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("DayOfWeek");

                    b.Property<byte>("Membership");

                    b.Property<byte>("SchoolDay");

                    b.HasKey("Id");

                    b.HasIndex("CalendarId");

                    b.ToTable("CalendarDays");
                });

            modelBuilder.Entity("models.DigitalSignature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("FileName");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.Property<byte[]>("imgData");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("DigitalSignatures");
                });

            modelBuilder.Entity("models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("ExternalId");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("PaymentId");

                    b.Property<int?>("SchoolDistrictId");

                    b.Property<string>("SchoolYear");

                    b.Property<int>("Split");

                    b.Property<string>("Type")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("Check");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("SchoolDistrictId");

                    b.HasIndex("PaymentId", "Split")
                        .IsUnique()
                        .HasFilter("[PaymentId] IS NOT NULL");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("models.Refund", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount");

                    b.Property<string>("CheckNumber");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<int?>("SchoolDistrictId");

                    b.Property<string>("SchoolYear");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("SchoolDistrictId");

                    b.ToTable("Refunds");
                });

            modelBuilder.Entity("models.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Approved");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Data");

                    b.Property<string>("Name");

                    b.Property<byte[]>("Pdf");

                    b.Property<string>("SchoolYear");

                    b.Property<string>("Scope");

                    b.Property<string>("Type");

                    b.Property<byte[]>("Xlsx");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("models.SchoolDistrict", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double?>("AlternateRate");

                    b.Property<double?>("AlternateSpecialEducationRate");

                    b.Property<int>("Aun");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("Name");

                    b.Property<string>("PaymentType")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("UniPay");

                    b.Property<double>("Rate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0.0);

                    b.Property<double>("SpecialEducationRate");

                    b.HasKey("Id");

                    b.HasIndex("Aun")
                        .IsUnique();

                    b.ToTable("SchoolDistricts");
                });

            modelBuilder.Entity("models.StudentRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActivitySchoolYear");

                    b.Property<int?>("HeaderId");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<int>("SchoolDistrictId");

                    b.Property<string>("SchoolDistrictName");

                    b.Property<string>("StudentCity");

                    b.Property<DateTime?>("StudentCurrentIep");

                    b.Property<DateTime>("StudentDateOfBirth");

                    b.Property<DateTime>("StudentEnrollmentDate");

                    b.Property<string>("StudentFirstName");

                    b.Property<DateTime?>("StudentFormerIep");

                    b.Property<string>("StudentGradeLevel");

                    b.Property<string>("StudentId");

                    b.Property<bool>("StudentIsSpecialEducation");

                    b.Property<string>("StudentLastName");

                    b.Property<string>("StudentMiddleInitial");

                    b.Property<DateTime?>("StudentNorep");

                    b.Property<decimal?>("StudentPaSecuredId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("StudentState");

                    b.Property<string>("StudentStreet1");

                    b.Property<string>("StudentStreet2");

                    b.Property<DateTime?>("StudentWithdrawalDate");

                    b.Property<string>("StudentZipCode");

                    b.HasKey("Id");

                    b.HasIndex("HeaderId");

                    b.ToTable("StudentRecords");
                });

            modelBuilder.Entity("models.StudentRecordsHeader", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Filename");

                    b.Property<bool>("Locked");

                    b.Property<string>("Scope");

                    b.HasKey("Id");

                    b.HasIndex("Scope")
                        .IsUnique()
                        .HasFilter("[Scope] IS NOT NULL");

                    b.ToTable("StudentRecordsHeaders");
                });

            modelBuilder.Entity("models.Template", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Content");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("Name");

                    b.Property<string>("ReportType");

                    b.Property<string>("SchoolYear");

                    b.HasKey("Id");

                    b.HasIndex("ReportType", "SchoolYear")
                        .IsUnique()
                        .HasFilter("[ReportType] IS NOT NULL AND [SchoolYear] IS NOT NULL");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("models.AuditDetail", b =>
                {
                    b.HasOne("models.AuditHeader", "Header")
                        .WithMany("Details")
                        .HasForeignKey("HeaderId");
                });

            modelBuilder.Entity("models.CalendarDay", b =>
                {
                    b.HasOne("models.Calendar", "Calendar")
                        .WithMany("Days")
                        .HasForeignKey("CalendarId");
                });

            modelBuilder.Entity("models.Payment", b =>
                {
                    b.HasOne("models.SchoolDistrict", "SchoolDistrict")
                        .WithMany()
                        .HasForeignKey("SchoolDistrictId");
                });

            modelBuilder.Entity("models.Refund", b =>
                {
                    b.HasOne("models.SchoolDistrict", "SchoolDistrict")
                        .WithMany()
                        .HasForeignKey("SchoolDistrictId");
                });

            modelBuilder.Entity("models.StudentRecord", b =>
                {
                    b.HasOne("models.StudentRecordsHeader", "Header")
                        .WithMany("Records")
                        .HasForeignKey("HeaderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
