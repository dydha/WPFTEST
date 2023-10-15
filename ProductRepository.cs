using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WPFTEST
{
    public class ProductRepository
    {

        private List<Product> products = new()
        {
            new Product { Id = 1, Name = "Computer", Barcode = "9890378449164", Price = 959.99m },
            new Product { Id = 2, Name = "Smartphone", Barcode = "7392345596861", Price = 499.99m },
            new Product { Id = 3, Name = "Tablet", Barcode = "6228845671856", Price = 299.99m },
            new Product { Id = 4, Name = "Laptop", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 799.99m },
            new Product { Id = 5, Name = "Television", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 799.99m },
            new Product { Id = 6, Name = "Coffee Maker", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 59.99m },
            new Product { Id = 7, Name = "Bluetooth Speaker", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 69.99m },
            new Product { Id = 8, Name = "Headphones", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 49.99m },
            new Product { Id = 9, Name = "Digital Camera", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 299.99m },
            new Product { Id = 10, Name = "Smart Watch", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 129.99m },
            new Product { Id = 11, Name = "Gaming Console", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 399.99m },
            new Product { Id = 12, Name = "Vacuum Cleaner", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 99.99m },
            new Product { Id = 13, Name = "Microwave Oven", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 129.99m },
            new Product { Id = 14, Name = "Toaster", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 39.99m },
            new Product { Id = 15, Name = "Blender", Barcode = EAN13Generator.Instance.GenerateRandomEAN13(), Price = 49.99m }
        };
        private static ProductRepository? instance;
        private ProductRepository() { }

        public static ProductRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProductRepository();
                }
                return instance;
            }
        }
        public Product? GetProduct(string barcode)
        {
            var product = products.Where(p => p.Barcode == barcode).FirstOrDefault();
            return product;
        }
        public List<Product> GetAllProducts()
        {
            return products;
        }
    }
}
