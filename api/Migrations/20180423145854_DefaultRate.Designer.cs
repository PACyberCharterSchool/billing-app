﻿// <auto-generated />
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace api.Migrations
{
    [DbContext(typeof(PacBillContext))]
    [Migration("20180423145854_DefaultRate")]
    partial class DefaultRate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("api.Models.SchoolDistrict", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("AlternateRate");

                    b.Property<int>("Aun");

                    b.Property<string>("Name");

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

                    b.Property<DateTime>("CurrentIep");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("FirstName");

                    b.Property<DateTime>("FormerIep");

                    b.Property<int>("Grade");

                    b.Property<bool>("IsSpecialEducation");

                    b.Property<string>("LastName");

                    b.Property<string>("MiddleInitial");

                    b.Property<int>("PACyberId");

                    b.Property<int>("PASecuredId");

                    b.Property<int?>("SchoolDistrictId");

                    b.Property<string>("State");

                    b.Property<string>("Street1");

                    b.Property<string>("Street2");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.HasIndex("SchoolDistrictId");

                    b.ToTable("Students");
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
