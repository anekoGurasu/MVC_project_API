using Microsoft.EntityFrameworkCore;
using MVC_Product.Models;
using System;

namespace MVC_Product.DataContext
{
    /// <summary>
    /// Třída AppDbContext reprezentuje kontext databáze pro aplikaci.
    /// </summary>
    public class AppDbContext:DbContext
    {
        /// <summary>
        /// Konstruktor třídy, který inicializuje novou instanci třídy AppDbContext.
        /// </summary>
        public AppDbContext()
        {
        }

        /// <summary>
        /// Konstruktor třídy, který inicializuje novou instanci třídy AppDbContext s danými možnostmi konfigurace.
        /// </summary>
        /// <param name="options">Možnosti konfigurace databázového kontextu.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Vlastnost Products reprezentuje databázovou sadu pro produkty.
        /// </summary>
        public DbSet<Product> Products { get; set; }
    }
        

    
}
