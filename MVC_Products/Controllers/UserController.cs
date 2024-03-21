using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC_Product.Models;
using Newtonsoft.Json;

namespace MVC_Product.Controllers
{
    /// <summary>
    /// Controller pro získání informací o uživatelích z API.
    /// </summary>
    public class UserController : Controller
    {
        // GET: UserController
        /// <summary>
        /// Akce pro získání seznamu uživatelů
        /// </summary>
        /// <returns>Pohled uživatelů</returns>
        public async Task<IActionResult> Index()
        {
            // Pouziti HttpClient pro komunikaci se vzdalenym API
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Zaslání GET požadavku na API
                    using (var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/users"))
                    {
                        // Zkontrolování, zda je odpověď úspěšná
                        response.EnsureSuccessStatusCode();

                        // Deserializace odpovědi na seznam uživatelů
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<User>>(jsonString);

                        // Předání seznamu uživatelů do pohledu pro zobrazení
                        return View(users);
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Zachycení výjimky při chybě při komunikaci se serverem
                    ViewBag.ErrorMessage = $"Chyba při komunikaci se serverem: {ex.Message}";
                    return View(new List<User>());
                }
            }
        }
    }
}
