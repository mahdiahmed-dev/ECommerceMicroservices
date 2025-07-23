using ProductService.Models;
using ProductService.Data;  // Assuming your DbContext is here
using Microsoft.EntityFrameworkCore;

namespace ProductService.Services
{
    public class ProductManager
    {
        private readonly ProductDbContext _dbContext;

        public ProductManager(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> GetAll() => _dbContext.Products.ToList();

        public Product? GetById(Guid id) => _dbContext.Products.Find(id);

        public void Add(Product product)
        {
            if (product.Id == Guid.Empty)
                product.Id = Guid.NewGuid();

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
        }

        public void Update(Product updatedProduct)
        {
            var existingProduct = _dbContext.Products.Find(updatedProduct.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.Price = updatedProduct.Price;

                _dbContext.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var product = _dbContext.Products.Find(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                _dbContext.SaveChanges();
            }
        }

        public bool AreAllProductIdsValid(IEnumerable<Guid> productIds)
        {
            var existingIds = _dbContext.Products
            .Select(p => p.Id)
            .ToHashSet();

            return productIds.All(id => existingIds.Contains(id));
        }
    }
}
