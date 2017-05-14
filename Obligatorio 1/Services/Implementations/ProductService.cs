﻿using System;
using Entities;
using Repository;
using Services.Interfaces;
using DataAccess;
using System.Linq;
using Exceptions;
using System.Collections.Generic;

namespace Services
{
    public class ProductService:IProductService
    {
        private GenericRepository<Product> repo;


        public ProductService(GenericRepository<Product> repoInstance)
        {
            this.repo = repoInstance;
        }

        public void Add(Product p)
        {
            p.Validate();
            checkForExistingProduct(p);
            setCategory(p);
            repo.Add(p);
        }

        private void checkForExistingProduct(Product p)
        {
            List<Product> allProducts = repo.GetAll();
            Product existing = allProducts.Find(prod => prod.Equals(p) && prod.Id != p.Id && prod.IsActive == true);
            if (existing != null) {
                throw new ProductDuplicateException("Ya existe un producto con este nombre y/o código");
            }
        }

        private void setCategory(Product p)
        {
            if (p.Category != null)
            {
                MyContext context = repo.GetContext();
                Category c = context.Categories.Where(ca => ca.Id == p.Category.Id).FirstOrDefault();
                if (c == null)
                    throw new NotExistingCategoryException("La categoría asignada a este producto no existe");
                p.Category = c;
            }
        }

        public void Modify(Product p)
        {
            checkIfProductExists(p);
            p.Validate();
            checkForExistingProduct(p);
            setCategory(p);
            repo.Update(p);
        }

        private void checkIfProductExists(Product p)
        {
            List<Product> allProducts = repo.GetAll();
            Product existing = allProducts.Find(prod => prod.Id == p.Id && prod.IsActive == true);
            if (existing == null)
            {
                throw new ProductModifyNotExistingException("No se puede modificar un producto que no está en el sistema");
            }
        }

        public Product Get(Guid id)
        {
            List<Product> allProducts = repo.GetAll();
            Product existing = allProducts.Find(prod => prod.Id == id && prod.IsActive == true);
            if (existing == null) {
                throw new ProductNotExistingException("No hay producto activo con este id");
            }
            return existing;
        }

        public void Delete(Product p)
        {
            Get(p.Id);
            p.IsActive = false;
            repo.Update(p);
        }

        public void ChangeCategory(Guid id, Category c2)
        {
            Product p = Get(id);
            
            MyContext context = repo.GetContext();
            if (c2 != null)
            {
                Category c = context.Categories.Where(ca => ca.Id == c2.Id).FirstOrDefault();
                if (c == null)
                {
                    throw new ProductChangeCategoryException("La categoría nueva no existe");
                }
                if (c2.Equals(p.Category))
                    throw new ProductChangeCategoryException("El producto ya tiene esta categoría");
                p.Category = c;
                
            }
            else p.Category = null;
            

            repo.Update(p);
        }
    }
}