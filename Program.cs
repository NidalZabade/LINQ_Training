
class Program
{

    class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
    }

    class Order
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<OrderSecond> Orders { get; set; }
    }
    public class OrderSecond
    {
        public int Id { get; set; }
        public List<OrderItem> Items { get; set; }
    }
    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

    }

    public class ProductSecond
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }

    public class Sale
    {
        public string Region { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }

    public class Transaction
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }

    static void Main(string[] args)
    {

        // Question 1: Multi-Level Join with Grouping and Conditional Aggregation
        List<Order> orders = new List<Order>
    {
        new Order { ProductId = 1, Quantity = 60 },
        new Order { ProductId = 2, Quantity = 50 },
        new Order { ProductId = 3, Quantity = 30 },
        new Order { ProductId = 4, Quantity = 80 },
        new Order { ProductId = 2, Quantity = 60 },
        new Order { ProductId = 3, Quantity = 90 }
    };

        List<Product> products = new List<Product>
    {
        new Product { Id = 1, CategoryId = 1 },
        new Product { Id = 2, CategoryId = 2 },
        new Product { Id = 3, CategoryId = 1 },
        new Product { Id = 4, CategoryId = 3 }
    };

        List<Category> categories = new List<Category>
    {
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Books" },
        new Category { Id = 3, Name = "Clothing" }
    };

        var categoryTotals = orders
            .Join(products, o => o.ProductId, p => p.Id, (o, p) => new { o.Quantity, p.CategoryId })
            .Join(categories, op => op.CategoryId, c => c.Id, (op, c) => new { c.Name, op.Quantity })
            .GroupBy(x => x.Name)
            .Select(g => new { Category = g.Key, TotalQuantity = g.Sum(x => x.Quantity) })
            .Where(x => x.TotalQuantity > 100);

        foreach (var item in categoryTotals)
        {
            Console.WriteLine($"Category: {item.Category}, Total Quantity Sold: {item.TotalQuantity}");
        }

        Console.WriteLine("----------------------------------------");


        // Question 2: Multi-Level Nested Join with Filtering
        List<ProductSecond> products2 = new List<ProductSecond>
        {
            new ProductSecond { Id = 1, Category = "Electronics", Price = 100 },
            new ProductSecond { Id = 2, Category = "Books", Price = 20 },
            new ProductSecond { Id = 3, Category = "Electronics", Price = 150 },
            new ProductSecond { Id = 4, Category = "Clothing", Price = 50 }
        };

        List<Customer> customers = new List<Customer>
        {
            new Customer
            {
                Id = 1,
                Name = "Alice",
                Orders = new List<OrderSecond>
                {
                    new OrderSecond
                    {
                        Id = 1,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = 1, Quantity = 2 },
                            new OrderItem { ProductId = 2, Quantity = 1 }
                        }
                    },
                    new OrderSecond
                    {
                        Id = 2,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = 3, Quantity = 1 }
                        }
                    }
                }
            },
            new Customer
            {
                Id = 2,
                Name = "Bob",
                Orders = new List<OrderSecond>
                {
                    new OrderSecond
                    {
                        Id = 3,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = 3, Quantity = 2 },
                            new OrderItem { ProductId = 4, Quantity = 1 }
                        }
                    }
                }
            }
        };

        var electronicsSpend = customers
            .Select(c => new
            {
                Customer = c.Name,
                TotalSpend = c.Orders
                    .SelectMany(o => o.Items)
                    .Join(products2.Where(p => p.Category == "Electronics"),
                          oi => oi.ProductId,
                          p => p.Id,
                          (oi, p) => oi.Quantity * p.Price)
                    .Sum()
            })
            .Where(x => x.TotalSpend > 0)
            .OrderByDescending(x => x.TotalSpend);

        foreach (var item in electronicsSpend)
        {
            Console.WriteLine($"Customer: {item.Customer}, Total Electronics Spend: {item.TotalSpend}");
        }
        Console.WriteLine("----------------------------------------");

        // Question 3: Dynamic Grouping with Sub-Aggregates

        List<Sale> sales = new List<Sale>
        {
            new Sale { Region = "North", CustomerId = 1, Amount = 1200 },
            new Sale { Region = "North", CustomerId = 2, Amount = 800 },
            new Sale { Region = "South", CustomerId = 3, Amount = 1500 },
            new Sale { Region = "South", CustomerId = 3, Amount = 700 },
            new Sale { Region = "East", CustomerId = 4, Amount = 950 },
            new Sale { Region = "East", CustomerId = 5, Amount = 1100 },
            new Sale { Region = "North", CustomerId = 1, Amount = 500 }
        };

        var regionStats = sales
            .GroupBy(s => s.Region)
            .Select(g => new
            {
                Region = g.Key,
                TotalSales = g.Sum(s => s.Amount),
                DistinctCustomers = g.Select(s => s.CustomerId).Distinct().Count(),
                MaxTransaction = g.Max(s => s.Amount),
                HighSales = g.Where(s => s.Amount > 1000).Sum(s => s.Amount),
                LowSales = g.Where(s => s.Amount <= 1000).Sum(s => s.Amount)
            });

        foreach (var item in regionStats)
        {
            Console.WriteLine($"Region: {item.Region}");
            Console.WriteLine($"  Total Sales: {item.TotalSales}");
            Console.WriteLine($"  Distinct Customers: {item.DistinctCustomers}");
            Console.WriteLine($"  Max Single Transaction: {item.MaxTransaction}");
            Console.WriteLine($"  High Sales (>1000): {item.HighSales}");
            Console.WriteLine($"  Low Sales (<=1000): {item.LowSales}");
        }
        Console.WriteLine("----------------------------------------");
        // Question 4: Time-Series Aggregation by Month and Category

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Date = new DateTime(2024, 1, 10), Category = "Food", Amount = 50 },
            new Transaction { Date = new DateTime(2024, 1, 15), Category = "Transport", Amount = 20 },
            new Transaction { Date = new DateTime(2024, 2, 5), Category = "Food", Amount = 30 },
            new Transaction { Date = new DateTime(2024, 2, 20), Category = "Transport", Amount = 25 },
            new Transaction { Date = new DateTime(2024, 1, 22), Category = "Food", Amount = 40 },
            new Transaction { Date = new DateTime(2024, 2, 18), Category = "Food", Amount = 60 }
        };

        var monthlyCategoryTotals = transactions
            .GroupBy(t => new { Month = t.Date.Month, t.Category })
            .Select(g => new
            {
                Month = g.Key.Month,
                Category = g.Key.Category,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .OrderBy(x => x.Month)
            .ThenBy(x => x.Category);

        foreach (var item in monthlyCategoryTotals)
        {
            Console.WriteLine($"Month: {item.Month}, Category: {item.Category}, Total Amount: {item.TotalAmount}");
        }
    }

}