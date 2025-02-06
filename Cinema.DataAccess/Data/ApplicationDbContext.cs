using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    MovieID = 1,
                    Title = "Inception",
                    Genre = "Sci-Fi",
                    Description = "A thief who enters the dreams of others to steal secrets.",
                    TrailerLink = "https://example.com/inception",
                    Duration = "148 min",
                    ReleaseDate = "2010-07-16",
                    MovieImage = ""
                },
                new Movie
                {
                    MovieID = 2,
                    Title = "The Dark Knight",
                    Genre = "Action",
                    Description = "Batman faces the Joker, a criminal mastermind.",
                    TrailerLink = "https://example.com/darkknight",
                    Duration = "152 min",
                    ReleaseDate = "2008-07-18",
                    MovieImage = ""
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductID = 1,
                    Name = "Popcorn",
                    Description = "A large bucket of buttered popcorn.",
                    ProductType = "Snack",
                    Price = 5.99,
                    ProductImage = ""
                },
                new Product
                {
                    ProductID = 2,
                    Name = "Soda",
                    Description = "Refreshing cold soda, 500ml.",
                    ProductType = "Drink",
                    Price = 2.99,
                    ProductImage = ""
                }
            );
        }


    }
}
