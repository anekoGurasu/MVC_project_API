using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using MVC_Product.DataContext;
using MVC_Product.Models;
using MVC_Product.Services;
using Newtonsoft.Json;

namespace MVC_Product.Controllers
{
    /// <summary>
    /// Controller pro manipulaci s produkty.
    /// </summary>
    public class ProductsController : Controller
    {
        //databazovy kontext
        private readonly AppDbContext _context;
        //cesta k souboru
        private static string filePath = "data/products.txt";
        //list produktu
        private readonly List<Product> fileProducts;
        // Instance tridy ProductService pro manipulaci se souborem s produkty.
        private readonly ProductService _productService = new ProductService(filePath);

        /// <summary>
        /// Inicializuje novou instanci třídy ProductsController s daným kontextem databáze.
        /// </summary>
        /// <param name="context">Kontext databáze</param>
        public ProductsController(AppDbContext context)
        {
            _context = context;
            // Produkty nactene ze souboru
            fileProducts = _productService.ReadProductsFromFileAsync().Result;

            // Synchronní volání pro počkání na dokončení inicializace
            InitializeProductsAsync().Wait();
        }

        /// <summary>
        /// Asynchronně inicializuje produkty.
        /// </summary>
        private async Task InitializeProductsAsync()
        {
            try
            {
                // Pokud v databazi nejsou zadne produkty, pridejte produkty z textového souboru.
               if (!_context.Products.Any())
                {
                    foreach (var fileProduct in fileProducts)
                    {
                        _context.Products.Add(fileProduct);
                    }

                    // Uloženi zmen do databaze.
                    await _context.SaveChangesAsync();
                }

                // Aktualizace databáze
                await UpdateDatabase(fileProducts);
            }
            catch (Exception ex)
            {
                // Zachycení vyjimky
                Console.WriteLine($"Chyba při inicializaci produktů: {ex.Message}");
                
            }
        }


        // GET: Products
        /// <summary>
        /// Získává a zobrazuje seznam všech produktů.
        /// </summary>
        /// <returns>Pohled s načtenými produkty z databáze</returns>
        public async Task<IActionResult> Index() {
            
            try{ 
                if (!_context.Database.CanConnect())
                {
                    // Pokud není možné připojit se k databázi, můžete vrátit odpovídající pohled s chybovou zprávou.
                    ViewData["ErrorMessage"] = "Database not found or not accessible.\r\nCheck user details of user \"tester\" and try again.";
                    return View("Error");
                }

                // Nactení produktu z databaze
                List<Product> dbProducts = await _context.Products.ToListAsync();

                // Vrati pohled s nactenymi produkty z databaze
                return View(dbProducts);
            }
            catch (Exception ex)
    {
                // Zachycení výjimky a předání do pohledu.
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }


        // GET: Products/Details/5
        /// <summary>
        /// Zobrazuje detaily konkrétního produktu.
        /// </summary>
        /// <param name="id">ID produktu</param>
        public async Task<IActionResult> Details(int? id)
        {
            // Overeni, zda bylo zadano platne ID produktu.
            if (id == null)
            {
                return NotFound();
            }

            // Ziskani produktu z databaze podle ID
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Vrati pohled s nactenymi produkty z databaze
            return View(product);
        }

        // GET: Products/Create
        /// <summary>
        /// Zobrazuje formulář pro vytvoření nového produktu.
        /// </summary>
        /// <returns>Pohled</returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        /// <summary>
        /// Ukládá nově vytvořený produkt do databáze.
        /// </summary>
        /// <param name="product">Nově vytvořený produkt.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price")] Product product)
        {
            // Zjistíme maximální hodnotu ID v databázi
    var maxId = await _context.Products.MaxAsync(p => (int?)p.Id) ?? 0;
    
    // Přiřadíme novému produktu nové ID
    product.Id = maxId + 1;

            // Overeni, zda cena produktu neni zaporna
            if (product.Price < 0)
            {
                // Pokud je cena produktu zaporna, pridame do ModelState chybovou zprávu
                ModelState.AddModelError("Price", "Price can not be negative.");
            }

            // Kontrola, zda je stav modelu platny.
            if (ModelState.IsValid)
            {
                // Pridani noveho produktu do databaze.
                _context.Add(product);
                await _context.SaveChangesAsync();

                // Aktualizovani textoveho souboru
                await _productService.WriteProductsToFileAsync(await _context.Products.ToListAsync());

                // Presmerovani na akci Index pro zobrazeni aktualizovaneho seznamu produktu.
                return RedirectToAction(nameof(Index));
            }
            // Vrati pohled s nactenymi produkty z databaze
            return View(product);
        }

        // GET: Products/Edit/5
        /// <summary>
        /// Zobrazuje formulář pro úpravu produktu s daným ID.
        /// </summary>
        /// <param name="id">ID produktu.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            // Overeni, zda bylo zadano platne ID produktu.
            if (id == null)
            {
                return NotFound();
            }

            // Ziskani produktu z databaze podle ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Vraceni pohledu s detaily daneho produktu
            return View(product);
        }

        // POST: Products/Edit/5
        /// <summary>
        /// Ukládá upravený produkt do databáze.
        /// </summary>
        /// <param name="id">ID produktu.</param>
        /// <param name="product">Upravený produkt.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] Product product)
        {
            // Overeni, zda bylo zadano platne ID produktu.
            if (id != product.Id)
            {
                return NotFound();
            }

            // Kontrola, zda je stav modelu platny.
            if (ModelState.IsValid)
            {
                try
                {
                    // Získání existujícího produktu z databáze
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Aktualizace vlastností existujícího produktu na základě předaných hodnot z formuláře
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    await _context.SaveChangesAsync();

                    // Aktualizace textoveho souboru s produkty
                    await _productService.WriteProductsToFileAsync(await _context.Products.ToListAsync());
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Pokud produkt jiz neexistuje v databazi, vrati se NotFound.
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Presmerovani na akci Index pro zobrazeni aktualizovaneho seznamu produktu.
                return RedirectToAction(nameof(Index));
            }
            // Vraceni pohledu s detaily daneho produktu
            return View(product);
        }

        // GET: Products/Delete/5
        /// <summary>
        /// Zobrazuje potvrzovací stránku pro smazání produktu s daným ID.
        /// </summary>
        /// <param name="id">ID produktu</param>
        public async Task<IActionResult> Delete(int? id)
        {
            // Overeni, zda bylo zadano platne ID produktu.
            if (id == null)
            {
                return NotFound();
            }

            // Ziskani produktu z databaze podle ID
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Aktualizace textoveho souboru s produkty
            await _productService.WriteProductsToFileAsync(await _context.Products.ToListAsync());
            
            // Vraceni pohledu s detaily daneho produktu.
            return View(product);
        }

        // POST: Products/Delete/5
        /// <summary>
        /// Odebírá produkt s daným ID z databáze.
        /// </summary>
        /// <param name="id">ID produktu</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ziskani produktu z databaze podle ID
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Odstraneni produktu z databaze
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // Aktualizace textoveho souboru s produkty
                await _productService.WriteProductsToFileAsync(await _context.Products.ToListAsync());
            }

            // Presmerovani na akci Index pro zobrazeni aktualizovaneho seznamu produktu.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Metoda pro ověření existence produktu v databázi podle zadaného ID.
        /// </summary>
        /// <param name="id">ID produktu</param>
        /// <returns>existence produktu v databázi (True/False)</returns>
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        /// <summary>
        /// Asynchronní metoda pro aktualizaci databáze na základě seznamu produktů
        /// </summary>
        /// <param name="products">Seznam produktů</param>
        private async Task UpdateDatabase(List<Product> products)
        {
            try
            {
                // Procházejte produkty a aktualizujte databázi
                foreach (var product in products)
                {
                    // Získává existující produkt z databáze podle ID.
                    var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
                    if (existingProduct != null)
                    {
                        // Pokud produkt existuje v databázi, aktualizujte ho
                        existingProduct.Name = product.Name;
                        if(product.Price>=0)
                        {
                            existingProduct.Price = product.Price;
                        }
                        else
                        {
                            existingProduct.Price = 0;
                        }
                    }
                    else
                    {
                        // Pokud produkt neexistuje v databázi, přidejte ho
                        _context.Products.Add(product);
                    }
                }

                // Odstranění položek, které nejsou v seznamu
                var productsIds = products.Select(p => p.Id).ToList();
                var itemsToDelete = await _context.Products.Where(p => !productsIds.Contains(p.Id)).ToListAsync();
                _context.Products.RemoveRange(itemsToDelete);

                // Uložit změny do databáze
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating database: {ex.Message}");
                throw; // Vyhození výjimky dále
            }
        }
    }
}
