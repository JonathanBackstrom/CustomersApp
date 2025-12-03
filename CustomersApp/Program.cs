using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CustomersApp;

internal class Program
{
    static void Main(string[] args)
    {
        var context = new ApplicationContext();

        //context.Orders.Add(new Order { CustomerId = 1, PriceTotal = 1000 });
        //context.Orders.Add(new Order { CustomerId = 1, PriceTotal = 1100 });
        //context.Orders.Add(new Order { CustomerId = 3, PriceTotal = 1200 });
        //context.SaveChanges();

        var q = context.Customers
            .Select(c => new { c.CompanyName, c.Id });

        foreach (var c in q)
        {
            Console.WriteLine($"Kundnamn: {c.CompanyName} ID: {c.Id}");
        }

        int input = int.Parse(Console.ReadLine());
        var customer = context.Customers.Find(input);
        if (customer is null)
            Console.WriteLine("Not found");
        else
        {
            var k = context.Orders
                .Where(o => o.CustomerId == input)
                .Select(o => new { o.Id, o.PriceTotal });

            foreach (var o in k)
            {
                Console.WriteLine($"OrderID: {o.Id} Pricetotal: {o.PriceTotal}");
            }
        }
    }

        //if (input < 1 || input > context.Customers.Count())
        //{
        //    Console.WriteLine("Ogiltigt val");
        //}
        //else



}

public class Customer
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string City { get; set; } = null!;

    // Navigation property (One-To-Many)
    public List<Order> Orders { get; set; } = null!;
}
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal PriceTotal { get; set; }
    public DateTime Created { get; set; }

    // Navigation property (Many-To-One)
    public Customer Customer { get; set; } = null!;
}
public class ApplicationContext : DbContext
{
    // Exponerar våra entiteter som DbSet
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("AppSettings.json")
            .Build();

        // Läs vår connection-string från konfigurations-filen
        var connStr = config.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connStr);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Specificerar vilken collation databasen ska använda
        modelBuilder.UseCollation("Finnish_Swedish_CI_AS");

        // Specificerar vilken datatyp databasen ska använda för en specifik kolumn
        modelBuilder.Entity<Order>()
            .Property(o => o.PriceTotal)
            .HasColumnType(SqlDbType.Money.ToString());

        // Specificerar data som en specifik tabell ska för-populeras med
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, CompanyName = "Company 1", City = "Stockholm" },
            new Customer { Id = 2, CompanyName = "Company 2", City = "Stockholm" },
            new Customer { Id = 3, CompanyName = "Company 3", City = "Göteborg" }
        );
    }
}
