using Microsoft.EntityFrameworkCore;
using MVC_Product.DataContext;
using MVC_Product.Migrations;
using MVC_Product.Models;
using Newtonsoft.Json;
using Product = MVC_Product.Models.Product;


namespace MVC_Product.Services
{
    /// <summary>
    /// Třída ProductService poskytuje metody pro čtení a zápis produktů do/z textového souboru.
    /// </summary>
    public class ProductService
    {
        //Cesta k souboru
        private string filePath;

        /// <summary>
        /// Konstruktor třídy, který inicializuje cestu k souboru.
        /// </summary>
        /// <param name="filePath">Cesta k souboru </param>
        public ProductService(string filePath)
        {
            // Inicializace cesty k souboru v konstruktoru tridy.
            this.filePath = filePath;
        }

        /// <summary>
        /// Asynchronní metoda pro čtení produktů ze souboru
        /// </summary>
        /// <returns>Seznam produktů</returns>
        public async Task<List<Product>> ReadProductsFromFileAsync()
        {
            try
            {
                string json;

                //otevreni souboru pro cteni
                using (StreamReader reader = File.OpenText(filePath))
                {
                    //cteni obsahu souboru
                    json = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(json))
                {
                    // Informace, pokud je soubor prazdny
                    Console.WriteLine("The file is empty or contains only whitespace.");
                    // Vrati prazdny seznam, pokud je soubor prazdny
                    return new List<Product>(); 
                }

                // Deserializace JSON obsahu souboru do seznamu produktu
                List<Product> products = JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>(); // Vrátí deserializovaný seznam nebo prázdný seznam, pokud deserializace selže

                // Vrati seznam produktu
                return products;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");

                // Vrati prazdny seznam, pokud je soubor prazdny
                return new List<Product>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");

                // Vrati prazdny seznam, pokud je soubor prazdny
                return new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");

                // Vrati prazdny seznam, pokud je soubor prazdny
                return new List<Product>();
            }
        }

        /// <summary>
        /// Asynchronní metoda pro zápis produktů do souboru
        /// </summary>
        /// <param name="products">Seznam produktů</param>
        public async Task WriteProductsToFileAsync(List<Product> products)
        {
            try
            {
                // Serializace seznamu produktu do formatu JSON
                string json = JsonConvert.SerializeObject(products);

                // Otevreni souboru pro zapis
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    // Zapis serializovanych dat do souboru
                    await writer.WriteAsync(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }
        
    }
}
