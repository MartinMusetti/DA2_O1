using Entities;
using Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
namespace ServicesTests
{
    [TestClass]
    public class CategoryServiceTests
    {
        private MyContext context;
        private MyContext getContext()
        {
            if (context == null)
                context = new MyContext();
            return context;
        }
        private CategoryService getService()
        {
            GenericRepository<Category> repo = new GenericRepository<Category>(getContext(),true);
            GenericRepository<Product> pRepo = new GenericRepository<Product>(getContext());
            return new CategoryService(repo, pRepo);
        }

        [TestMethod]
        public void CreateCategoryOkTest()
        {
            Category c = new Category();
            c.Name = "Cat1";
            c.Description = "Desc.";
            CategoryService service = getService();
            service.Add(c);
            Assert.AreNotEqual(Guid.Empty, c.Id);
        }

        [ExpectedException(typeof(MissingCategoryDataException))]
        [TestMethod]
        public void MissingCategoryNameTest()
        {
            Category c = new Category();
            c.Description = "Desc";
            CategoryService service = getService();
            service.Add(c);
        }

        [ExpectedException(typeof(MissingCategoryDataException))]
        [TestMethod]
        public void MissingCategoryDescriptionTest()
        {
            Category c = new Category();
            c.Name = "Cat";
            CategoryService service = getService();
            service.Add(c);
        }

        [ExpectedException(typeof(ExistingCategoryNameException))]
        [TestMethod]
        public void ExistingCategoryNameTest()
        {
            Category c = new Category();
            c.Name = "Category";
            c.Description = "Desc.";

            Category c2 = new Category();
            c2.Name = "Category";
            c2.Description = "Description1";

            CategoryService service = getService();
            service.Add(c);
            service.Add(c2);
        }

        [TestMethod]
        public void DeleteCategoryOkTest()
        {
            Category c1 = new Category();
            c1.Name = "Category";
            c1.Description = "Desc.";
            Category c2 = new Category();
            c2.Name = "Category 2";
            c2.Description = "Desc. Cat. 2";

            CategoryService service = getService();

            service.Add(c1);
            service.Add(c2);
            service.Delete(c1.Id);

            List<Category> categories = service.GetAll();
            Assert.AreEqual(categories.Count, 1);
            Assert.AreEqual(categories[0], c2);

            //FALTA SETEARLE LA CATEGORIA A UN PRODUCTO Y CHEQUEAR QUE CUANDO LA BORRO SE LA SAQUE A TODOS LOS PRODUCTOS
        }

        [ExpectedException(typeof(NotExistingCategoryException))]
        [TestMethod]
        public void DeleteNotExistingCategoryTest()
        {
            Category c1 = new Category();
            c1.Name = "Category";
            c1.Description = "Desc.";
            Category c2 = new Category();
            c2.Name = "Category 2";
            c2.Description = "Desc. Cat. 2";

            CategoryService service = getService();

            service.Add(c1);
            service.Add(c2);

            byte[] bytes = new byte[16];
            int originalIdToDelete = 3;
            BitConverter.GetBytes(originalIdToDelete).CopyTo(bytes, 0);
            Guid idToDelete = new Guid(bytes);
            service.Delete(idToDelete);
        }

        [TestMethod]
        public void ModifyCategoryOkTest()
        {
            Category c = new Category();
            c.Name = "Category";
            c.Description = "Desc.";

            CategoryService service = getService();
            service.Add(c);

            service.Modify(c, "Category Mod", c.Description);
            Category modifiedCat = service.Get(c.Id);
            Assert.AreEqual(modifiedCat.Name, "Category Mod");
        }

        [ExpectedException(typeof(MissingCategoryDataException))]
        [TestMethod]
        public void ModifyMissingCategoryNameTest()
        {
            Category c = new Category();
            c.Name = "Cat1";
            c.Description = "Desc";
            CategoryService service = getService();
            service.Add(c);
            string modifiedName = "";
            service.Modify(c, modifiedName, c.Description);
        }

      /*  [ExpectedException(typeof(MissingCategoryDataException))]
        [TestMethod]
        public void ModifyMissingCategoryDescriptionTest()
        {
            Category c = new Category();
            c.Name = "Cat1";
            c.Description = "Desc";
            CategoryService service = getService();
            service.Add(c);
            string modifiedDescription = "";
            service.Modify(c, c.Name, modifiedDescription);
        }
        */
        [ExpectedException(typeof(ExistingCategoryNameException))]
        [TestMethod]
        public void ModifyExistingCategoryNameTest()
        {
            Category c = new Category();
            c.Name = "Category1";
            c.Description = "Desc.";

            Category c2 = new Category();
            c2.Name = "Category2";
            c2.Description = "Desc.";

            CategoryService service = getService();
            service.Add(c);
            service.Add(c2);
            string modifiedName = "Category2";
            service.Modify(c, modifiedName, c.Description);
        }


    }
}
