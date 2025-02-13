using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    movieID = 1,
                    title = "Inception",
                    genre = "Sci-Fi",
                    ageLimit = "10+",
                    description = "A thief who enters the dreams of others to steal secrets.",
                    trailerLink = "https://example.com/inception",
                    duration = "148 min",
                    releaseDate = "2010-07-16",
                    movieImage = ""
                },
                new Movie
                {
                    movieID = 2,
                    title = "The Dark Knight",
                    ageLimit= "18+",
                    genre = "Action",
                    description = "Batman faces the Joker, a criminal mastermind.",
                    trailerLink = "https://example.com/darkknight",
                    duration = "152 min",
                    releaseDate = "2008-07-18",
                    movieImage = ""
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    productID = 1,
                    nameProduct = "Popcorn",
                    description = "A large bucket of buttered popcorn.",
                    productType = "Snack",
                    price = 5.99,
                    productImage = ""
                },
                new Product
                {
                    productID = 2,
                    nameProduct = "Soda",
                    description = "Refreshing cold soda, 500ml.",
                    productType = "Drink",
                    price = 2.99,
                    productImage = ""
                }
            );
        }


    }
}
