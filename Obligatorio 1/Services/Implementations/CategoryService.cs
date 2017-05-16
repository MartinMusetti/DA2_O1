using Entities;
using Exceptions;
using Repository;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private IGenericRepository<Category> categoryRepository;
        private IGenericRepository<Product> productRepository;

        public CategoryService(IGenericRepository<Category> repo, IGenericRepository<Product> productRepo)
        {
            categoryRepository = repo;
            productRepository = productRepo;
        }

        public void Add(Category c)
        {
            c.Validate();
            checkForExistingName(c.Name);
            categoryRepository.Add(c);
        }

        private void checkForExistingName(string name)
        {
            List<Category> allCategories = categoryRepository.GetAll();
            var existingCategory = allCategories.Find(category => category.Name == name);
            if (existingCategory != null)
            {
                throw new ExistingCategoryNameException("Ya existe una categoria con este nombre");
            }
        }

        public void Delete(System.Guid id) {
            List<Product> allProducts = productRepository.GetAll();
            List<Product> productsWithCat = allProducts.FindAll(p => p.Category != null && p.Category.Id == id);
            foreach (Product p in productsWithCat) {
                p.Category = null;
                productRepository.Update(p);
            }
            bool deleted = categoryRepository.Delete(id);
            if (!deleted) {
                throw new NotExistingCategoryException();
            } 
        }

        public List<Category> GetAll()
        {
            return categoryRepository.GetAll();
        }

        public Category Get(Guid id) {
            Category c = categoryRepository.Get(id);
            if (c != null)
                return c;
            throw new NotExistingCategoryException("No hay categoría con este id");
        }

        public void Modify(Category c, string name, string description)
        {
            if (c.Name != name) {
                checkForExistingName(name);
            }
            c.Name = name;
            c.Description = description;
            c.Validate();
            categoryRepository.Update(c);
        }
    }
}
