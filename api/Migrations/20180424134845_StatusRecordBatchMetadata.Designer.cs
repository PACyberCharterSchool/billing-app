﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using api.Models;

namespace api.Migrations
{
    [DbContext(typeof(PacBillContext))]
    [Migration("20180424134845_StatusRecordBatchMetadata")]
    partial class StatusRecordBatchMetadata
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-preview1-28290")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("api.Models.SchoolDistrict", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("AlternateRate");

                    b.Property<int>("Aun");

                    b.Property<string>("Name");

                    b.Property<string>("PaymentType")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("ACH");

                    b.Property<decimal>("Rate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0m);

                    b.HasKey("Id");

                    b.ToTable("SchoolDistricts");
                });

            modelBuilder.Entity("api.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<DateTime?>("CurrentIep");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("FirstName");

                    b.Property<DateTime?>("FormerIep");

                    b.Property<string>("Grade");

                    b.Property<bool>("IsSpecialEducation");

                    b.Property<string>("LastName");

                    b.Property<string>("MiddleInitial");

                    b.Property<DateTime?>("NOREP");

                    b.Property<int>("PACyberId");

                    b.Property<ulong?>("PASecuredId");

                    b.Property<int?>("SchoolDistrictId");

                    b.Property<string>("State");

                    b.Property<string>("Street1");

                    b.Property<string>("Street2");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.HasIndex("SchoolDistrictId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("api.Models.StudentStatusRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivitySchoolYear");

                    b.Property<string>("BatchFilename");

                    b.Property<string>("BatchHash");

                    b.Property<DateTime>("BatchTime");

                    b.Property<int>("SchoolDistrictId");

                    b.Property<string>("SchoolDistrictName");

                    b.Property<string>("StudentCity");

                    b.Property<DateTime?>("StudentCurrentIep");

                    b.Property<DateTime>("StudentDateOfBirth");

                    b.Property<DateTime>("StudentEnrollmentDate");

                    b.Property<string>("StudentFirstName");

                    b.Property<DateTime?>("StudentFormerIep");

                    b.Property<string>("StudentGradeLevel");

                    b.Property<int>("StudentId");

                    b.Property<bool>("StudentIsSpecialEducation");

                    b.Property<string>("StudentLastName");

                    b.Property<string>("StudentMiddleInitial");

                    b.Property<DateTime?>("StudentNorep");

                    b.Property<ulong?>("StudentPaSecuredId");

                    b.Property<string>("StudentState");

                    b.Property<string>("StudentStreet1");

                    b.Property<string>("StudentStreet2");

                    b.Property<DateTime?>("StudentWithdrawalDate");

                    b.Property<string>("StudentZipCode");

                    b.HasKey("Id");

                    b.ToTable("PendingStudentStatusRecords");
                });

            modelBuilder.Entity("api.Models.Student", b =>
                {
                    b.HasOne("api.Models.SchoolDistrict", "SchoolDistrict")
                        .WithMany("Students")
                        .HasForeignKey("SchoolDistrictId");
                });
#pragma warning restore 612, 618
        }
    }
}
