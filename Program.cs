
class Program
{
    

    //Second Task Main
    public class Customer
    {
        public string Name { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
    }

    public class Sale
    {
        public string Region { get; set; }
        public decimal Amount { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Category { get; set; }
    }

    public class Order_Group
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    static void Main(string[] args)
    {

        // Question 1: Nested Projection with SelectMany

        List<Customer> customers = new List<Customer>
        {
            new Customer { Name = "Alice", Orders = new List<Order> { new Order { OrderId = 1, Total = 100 }, new Order { OrderId = 2, Total = 150 } } },
            new Customer { Name = "Bob", Orders = new List<Order> { new Order { OrderId = 3, Total = 200 } } }
        };

        var flattenedOrders = customers
            .SelectMany(c => c.Orders, (c, o) => new { CustomerName = c.Name, o.Total });

        foreach (var item in flattenedOrders)
        {
            Console.WriteLine($"Customer: {item.CustomerName}, Order Total: {item.Total}");
        }

        Console.WriteLine("----------------------------------------");
        // Question 2: Conditional Group Aggregation

        List<Sale> sales = new List<Sale>
        {
            new Sale { Region = "North", Amount = 1200 },
            new Sale { Region = "North", Amount = 800 },
            new Sale { Region = "South", Amount = 1500 },
            new Sale { Region = "South", Amount = 700 },
            new Sale { Region = "East", Amount = 2000 },
            new Sale { Region = "East", Amount = 500 }
        };

        var regionStats = sales
            .GroupBy(s => s.Region)
            .Select(g => new
            {
                Region = g.Key,
                TotalSales = g.Sum(s => s.Amount),
                AverageSales = g.Average(s => s.Amount),
                HighValueSalesCount = g.Count(s => s.Amount > 1000)
            });

        foreach (var stat in regionStats)
        {
            Console.WriteLine($"Region: {stat.Region}, Total: {stat.TotalSales}, Average: {stat.AverageSales}, High-Value Sales: {stat.HighValueSalesCount}");
        }

        Console.WriteLine("----------------------------------------");
        // Question 3: Join + GroupBy + Aggregation

        List<Order_Group> orders = new List<Order_Group>
        {
            new Order_Group { ProductId = 1, Quantity = 10 },
            new Order_Group { ProductId = 2, Quantity = 5 },
            new Order_Group { ProductId = 3, Quantity = 8 },
            new Order_Group { ProductId = 1, Quantity = 7 },
            new Order_Group { ProductId = 2, Quantity = 3 }
        };

        List<Product> products = new List<Product>
        {
            new Product { Id = 1, Category = "Electronics" },
            new Product { Id = 2, Category = "Books" },
            new Product { Id = 3, Category = "Electronics" }
        };

        var categorySales = orders
            .Join(products,
                o => o.ProductId,
                p => p.Id,
                (o, p) => new { p.Category, o.Quantity })
            .GroupBy(x => x.Category)
            .Select(g => new
            {
                Category = g.Key,
                TotalQuantitySold = g.Sum(x => x.Quantity)
            });

        foreach (var item in categorySales)
        {
            Console.WriteLine($"Category: {item.Category}, Total Quantity Sold: {item.TotalQuantitySold}");
        }
    }

}