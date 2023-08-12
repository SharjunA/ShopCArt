using System;
using System.Collections.Generic;

internal class Program
{
    internal class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; } // Added quantity property
    }

    internal class Discount
    {
        public int DiscountId { get; set; }
        public List<int> ProductIds { get; set; }
        public string Name { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
    }

    private static List<Product> products = new List<Product>
    {
        new Product { ProductId = 1, Name = "Banana", Unit = "Kg", Price = 100.00 },
        new Product { ProductId = 2, Name = "Orange", Unit = "Kg", Price = 230.00 },
        new Product { ProductId = 3, Name = "Apple", Unit = "Kg", Price = 330.00 },
        new Product { ProductId = 4, Name = "Grapes", Unit = "Kg", Price = 230.00 }
    };

    private static List<Discount> discounts = new List<Discount>
    {
        new Discount
        {
            DiscountId = 1,
            ProductIds = new List<int> { 1 },
            Name = "Buy 1 Get 1 Free",
            EffectiveStartDate = new DateTime(2023, 8, 2),
            EffectiveEndDate = new DateTime(2023, 8, 10)
        },
        new Discount
        {
            DiscountId = 2,
            ProductIds = new List<int> { 2, 3 },
            Name = "Buy 2 Get 1 Free",
            EffectiveStartDate = new DateTime(2023, 8, 2),
            EffectiveEndDate = null
        }
    };

    private static List<Product> cart = new List<Product>();

    private static void Main(string[] args)
    {
        ShowMenu();
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Welcome to the Online Shopping System!");
        while (true)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. View Shopping Cart");
            Console.WriteLine("3. Add Product to Cart");
            Console.WriteLine("4. Exit");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        ViewProducts();
                        break;
                    case 2:
                        ViewCart();
                        break;
                    case 3:
                        AddToCart();
                        break;
                    case 4:
                        Console.WriteLine("Thank you for shopping with us!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose a valid option.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
    }

    private static void ViewProducts()
    {
        Console.WriteLine("\nList of Products:");
        foreach (var product in products)
        {
            Console.WriteLine($"\nProduct ID: {product.ProductId}");
            Console.WriteLine($"Name: {product.Name}");
            Console.WriteLine($"Unit: {product.Unit}");
            Console.WriteLine($"Price: {product.Price:F2}");
        }

        Console.WriteLine("\nDiscounts:");
        foreach (var discount in discounts)
        {
            Console.WriteLine($"\nDiscount ID: {discount.DiscountId}");
            Console.WriteLine($"Name: {discount.Name}");
            Console.WriteLine($"Effective Start Date: {discount.EffectiveStartDate:yyyy-MM-dd}");
            Console.WriteLine($"Effective End Date: {(discount.EffectiveEndDate.HasValue ? discount.EffectiveEndDate.Value.ToString("yyyy-MM-dd") : "N/A")}");
            Console.Write("Product IDs: [");
            foreach (var productId in discount.ProductIds)
            {
                Console.Write($"{productId} ");
            }
            Console.WriteLine("]");
        }
    }

    private static void ViewCart()
    {
        Console.WriteLine("\nShopping Cart:");
        if (cart.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
        }
        else
        {
            double totalPrice = 0;
            Console.WriteLine($"\nProduct ID\tName\tUnit\tPrice\tQuantity");
            foreach (var product in cart)
            {
                Console.WriteLine($"\n{product.ProductId}\t\t{product.Name}\t{product.Unit}\t{product.Price:F2}\t{product.Quantity}");
                totalPrice += product.Price * product.Quantity;
            }
            Console.WriteLine("\nTotal Price: " + totalPrice.ToString("F2"));
        }
    }

    private static void AddToCart()
    {
        Console.WriteLine("\nAdd products to cart");

        while (true)
        {
            Console.WriteLine("\nEnter the Product ID of the Item you wish to add (or 0 to go back):");
            int productId;
            if (int.TryParse(Console.ReadLine(), out productId))
            {
                if (productId == 0)
                {
                    break;
                }

                var product = products.Find(p => p.ProductId == productId);
                if (product != null)
                {
                    Console.WriteLine($"\nAdding {product.Name} to cart...");
                    Console.WriteLine($"Enter the quantity of {product.Name} in kg:");
                    int quantity;
                    if (int.TryParse(Console.ReadLine(), out quantity))
                    {
                        if (quantity > 0)
                        {
                            var existingCartItem = cart.Find(item => item.ProductId == productId);
                            if (existingCartItem != null)
                            {
                                existingCartItem.Quantity += quantity; // Accumulate quantity
                                existingCartItem.Price += ApplyDiscount(product.ProductId, product.Price, quantity);
                            }
                            else
                            {
                                product.Quantity = quantity;
                                product.Price = ApplyDiscount(product.ProductId, product.Price, quantity);
                                cart.Add(product);
                            }

                            Console.WriteLine($"\n{product.Name} added to the cart.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid quantity. Please enter a positive value.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid quantity.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Product ID. Please enter a valid Product ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
    }



    private static double ApplyDiscount(int productId, double price, int quantity)
    {
        double totalDiscount = 0.0;

        foreach (var discount in discounts)
        {
            if (discount.ProductIds.Contains(productId) && IsDiscountApplicable(discount))
            {
                if (discount.DiscountId == 1)
                {
                    int eligibleQuantity = quantity / 2;
                    totalDiscount += (eligibleQuantity * price);
                }
                else if (discount.DiscountId == 2)
                {
                    int eligibleQuantity = quantity / 3;
                    totalDiscount += (eligibleQuantity * price);
                }
            }
        }

        return price - totalDiscount;
    }


    private static bool IsDiscountApplicable(Discount discount)
    {
        return discount.EffectiveStartDate <= DateTime.Today && (!discount.EffectiveEndDate.HasValue || discount.EffectiveEndDate >= DateTime.Today);
    }
}