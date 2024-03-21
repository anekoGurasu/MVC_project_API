namespace MVC_Product.Models
{
    /// <summary>
    /// Třída reprezentující adresu
    /// </summary>
    public class Address
    {
        public string Street { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public Geo Geo { get; set; }
    }

    /// <summary>
    /// Třída reprezentující geografické údaje
    /// </summary>
    public class Geo
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }

    /// <summary>
    /// Třída reprezentující společnost
    /// </summary>
    public class Company
    {
        public string Name { get; set; }
        public string CatchPhrase { get; set; }
        public string Bs { get; set; }
    }

    /// <summary>
    /// Třída reprezentující uživatele
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public Company Company { get; set; }
    }
}
