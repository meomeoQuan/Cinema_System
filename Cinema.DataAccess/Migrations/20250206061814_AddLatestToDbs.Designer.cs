﻿// <auto-generated />
using System;
using Cinema.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cinema.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250206061814_AddLatestToDbs")]
    partial class AddLatestToDbs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Cinema.Models.Movie", b =>
                {
                    b.Property<int>("movieID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("movieID"));

                    b.Property<string>("ageLimit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("duration")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("genre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("movieImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("releaseDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("trailerLink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("updatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("movieID");

                    b.ToTable("Movies");

                    b.HasData(
                        new
                        {
                            movieID = 1,
                            ageLimit = "10+",
                            description = "A thief who enters the dreams of others to steal secrets.",
                            duration = "148 min",
                            genre = "Sci-Fi",
                            movieImage = "",
                            releaseDate = "2010-07-16",
                            title = "Inception",
                            trailerLink = "https://example.com/inception"
                        },
                        new
                        {
                            movieID = 2,
                            ageLimit = "18+",
                            description = "Batman faces the Joker, a criminal mastermind.",
                            duration = "152 min",
                            genre = "Action",
                            movieImage = "",
                            releaseDate = "2008-07-18",
                            title = "The Dark Knight",
                            trailerLink = "https://example.com/darkknight"
                        });
                });

            modelBuilder.Entity("Cinema.Models.Product", b =>
                {
                    b.Property<int>("productID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("productID"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nameProduct")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("price")
                        .HasColumnType("float");

                    b.Property<string>("productImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("productType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("productID");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            productID = 1,
                            description = "A large bucket of buttered popcorn.",
                            nameProduct = "Popcorn",
                            price = 5.9900000000000002,
                            productImage = "",
                            productType = "Snack"
                        },
                        new
                        {
                            productID = 2,
                            description = "Refreshing cold soda, 500ml.",
                            nameProduct = "Soda",
                            price = 2.9900000000000002,
                            productImage = "",
                            productType = "Drink"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
