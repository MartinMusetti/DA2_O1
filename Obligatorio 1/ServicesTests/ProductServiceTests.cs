using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exceptions;
using Services;
using Entities.Statuses_And_Roles;
using DataAccess;

namespace ServicesTests
{
    [TestClass]
    public class ProductServiceTests
    {
        private MyContext context;
        private MyContext getContext()
        {
            if (context == null)
                context = new MyContext();
            return context;
        }
        private ProductService getService() {
            GenericRepository<Product> repoInstance = new GenericRepository<Product>(getContext(),true);
            GenericRepository<ProductFeature> productFeatureRepoInstance = new GenericRepository<ProductFeature>(getContext());
            GenericRepository<Feature> featureRepoInstance = new GenericRepository<Feature>(getContext());
            GenericRepository<OrderProduct> orderProductRepoInstance = new GenericRepository<OrderProduct>(getContext());
            return new ProductService(repoInstance, productFeatureRepoInstance,featureRepoInstance,orderProductRepoInstance);
        }

        private CategoryService getCategoryService()
        {
            GenericRepository<Category> repoInstance = new GenericRepository<Category>(getContext());
            GenericRepository<Product> productRepo = new GenericRepository<Product>(getContext());

            return new CategoryService(repoInstance,productRepo);
        }

        private FeatureService getFeatureService()
        {
            GenericRepository<Feature> repoInstance = new GenericRepository<Feature>(getContext());
            return new FeatureService(repoInstance);
        }

        private Category getCategory()
        {
            Category c = new Category();
            c.Name = "Nombre Cat";
            c.Description = "Description Cat";
            CategoryService service = getCategoryService();
            service.Add(c);
            return c;
        }

        private Category getOtherCategory()
        {
            Category c = new Category();
            c.Name = "Nombre Cat 2";
            c.Description = "Description Cat 2";
            CategoryService service = getCategoryService();
            service.Add(c);
            return c;
        }


        [TestMethod]
        public void ProductCreateOkTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            Product savedProduct = service.Get(p.Id);

            Assert.IsNotNull(savedProduct);
        }

        [TestMethod]
        public void ProductCreateNoCategoryOkTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            service.Add(p);
            Product savedProduct = service.Get(p.Id);

            Assert.IsNotNull(savedProduct);
        }

        [ExpectedException(typeof(NotExistingCategoryException))]
        [TestMethod]
        public void ProductCreateWrongCategoryTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = new Category();
            service.Add(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductCreateNoNameTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductCreateNoCodeTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "nombre";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductCreateNoDescriptionTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Manufacturer = "Manu";
            p.Name = "nombre";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductCreateNoManufacturerTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Name = "nombre";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductCreateNoPriceTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "nombre";
            p.Category = getCategory();
            service.Add(p);
        }

        [ExpectedException(typeof(ProductWrongPriceException))]
        [TestMethod]
        public void ProductCreateWrongPriceTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "nombre";
            p.Price = -100;
            p.Category = getCategory();
            service.Add(p);
        }

        [ExpectedException(typeof(ProductDuplicateException))]
        [TestMethod]
        public void ProductCreateDuplicateNameTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "nombre";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            Product p2 = new Product();
            p2.Code = "3333";
            p2.Description = "Desc";
            p2.Manufacturer = "Manu";
            p2.Name = "nombre";
            p2.Price = 100;
            p2.Category = cat;
            service.Add(p2);
        }

        [ExpectedException(typeof(ProductDuplicateException))]
        [TestMethod]
        public void ProductCreateDuplicateCodeTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "3333";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "nombre";
            p.Price = 100;
            Category cat = getCategory();

            p.Category = cat;
            service.Add(p);

            Product p2 = new Product();
            p2.Code = "3333";
            p2.Description = "Desc";
            p2.Manufacturer = "Manu";
            p2.Name = "Hola";
            p2.Price = 100;
            p2.Category = cat;
            service.Add(p2);
        }

        [TestMethod]
        public void ProductModifyOkTest() {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Name = "Cambiado";
            service.Modify(p);
            Product savedProduct = service.Get(p.Id);

            Assert.IsNotNull(savedProduct);
            Assert.AreEqual("Cambiado",savedProduct.Name);
        }

        [TestMethod]
        public void ProductModifyNoCategoryOkTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Category = null;
            service.Modify(p);
            Product savedProduct = service.Get(p.Id);

            Assert.IsNotNull(savedProduct);
            Assert.IsNull(savedProduct.Category);
        }

        [ExpectedException(typeof(NotExistingCategoryException))]
        [TestMethod]
        public void ProductModifyWrongCategoryTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Category = new Category();
            service.Modify(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductModifyNoNameTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Name = "";
            service.Modify(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductModifyNoCodeTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Code = "";
            service.Modify(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductModifyNoDescriptionTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Description = "";
            service.Modify(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductModifyNoManufacturerTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Manufacturer = "";
            service.Modify(p);
        }

        [ExpectedException(typeof(ProductMissingDataException))]
        [TestMethod]
        public void ProductModifyNoPriceTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Price = 0;
            service.Modify(p);
        }

        [ExpectedException(typeof(ProductWrongPriceException))]
        [TestMethod]
        public void ProductModifyNegativePriceTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            p.Price = -10;
            service.Modify(p);
        }

        [ExpectedException(typeof(ProductDuplicateException))]
        [TestMethod]
        public void ProductModifyDuplicateNameTest()
        {
            ProductService service = getService();

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();

            p.Category = cat;
            service.Add(p);

            Product p2 = new Product();
            p2.Code = "12345";
            p2.Description = "Desc";
            p2.Manufacturer = "Manu";
            p2.Name = "Nombre";
            p2.Price = 100;
            p2.Category = cat;
            service.Add(p2);

            p2.Name = "Name";
            service.Modify(p2);
        }

        [ExpectedException(typeof(ProductDuplicateException))]
        [TestMethod]
        public void ProductModifyDuplicateCodeTest()
        {
            ProductService service = getService();

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            Product p2 = new Product();
            p2.Code = "12345";
            p2.Description = "Desc";
            p2.Manufacturer = "Manu";
            p2.Name = "Nombre";
            p2.Price = 100;
            p2.Category = cat;
            service.Add(p2);

            p2.Code = "1234";
            service.Modify(p2);
        }

        [ExpectedException(typeof(ProductNotExistingException))]
        [TestMethod]
        public void ProductModifyNotExistingTest() {
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            ProductService service = getService();
            service.Modify(p);
        }

        [ExpectedException (typeof(ProductNotExistingException))]
        [TestMethod]
        public void ProductDeleteOkTest() {//FALTA BORRAR ORDER PRODUCTS DE ORDENES WAITING FOR SHIPPING, NO LLAMO A DELETE, CAMBIO ACTIVE A 0, ESO CAMBIA LOS GET!!!!
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            service.Delete(p.Id);
            Product savedProduct = service.Get(p.Id);
         }

        [ExpectedException(typeof(ProductNotExistingException))]
        [TestMethod]
        public void ProductDeleteNotExistingTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Delete(p.Id);
        }

        //Delete con ordenes con este producto (borro order products de ordenes haciendose), cambio active a 0

        [TestMethod]
        public void ProductChangeCategoryOkTest() {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);

            Category c2 = getOtherCategory();
            service.ChangeCategory(p.Id, c2.Id);

            Product savedProduct = service.Get(p.Id);
            Assert.AreEqual(c2.Id, savedProduct.Category.Id);
        }

        [TestMethod]
        public void ProductChangeCategoryNoCategoryOkTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);

            service.ChangeCategory(p.Id, Guid.Empty);

            Product savedProduct = service.Get(p.Id);
            Assert.AreEqual(null, savedProduct.Category);
        }

        [ExpectedException (typeof(ProductChangeCategoryException))]
        [TestMethod]
        public void ProductChangeCategorySameCategoryTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);
            service.ChangeCategory(p.Id, cat.Id);
        }

        [ExpectedException(typeof(ProductChangeCategoryException))]
        [TestMethod]
        public void ProductChangeCategoryWrongCategoryTest()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);
            Category newC = new Category();
            service.ChangeCategory(p.Id,newC.Id);
        }

        [ExpectedException(typeof(ProductNotExistingException))]
        [TestMethod]
        public void ProductChangeCategoryNoProductTest() {
            ProductService service = getService();
            Product p = new Product();
            Category cat = getCategory();
            service.ChangeCategory(p.Id, cat.Id);
        }

        [TestMethod]
        public void ProductAddFeatureStringOkTest() {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.STRING;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Rojo";

            service.AddProductFeature(productFeature);

            List<ProductFeature> productFeatures = service.GetAllProductFeaturesFromProduct(p);
            Assert.AreEqual(productFeature, productFeatures.First());
             
        }

        [TestMethod]
        public void ProductAddFeatureIntOkTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.INT;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "11";

            service.AddProductFeature(productFeature);

            List<ProductFeature> productFeatures = service.GetAllProductFeaturesFromProduct(p);
            Assert.AreEqual(productFeature, productFeatures.First());

        }

        [TestMethod]
        public void ProductAddFeatureDateOkTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.DATE;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = DateTime.Now.ToString();

            service.AddProductFeature(productFeature);

            List<ProductFeature> productFeatures = service.GetAllProductFeaturesFromProduct(p);
            Assert.AreEqual(productFeature, productFeatures.First());

        }

        [ExpectedException (typeof(NoFeatureException))]
        [TestMethod]
        public void ProductAddFeatureNoFeatureTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.STRING;

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Rojo";

            service.AddProductFeature(productFeature);

        }

        [ExpectedException(typeof(ProductNotExistingException))]
        [TestMethod]
        public void ProductAddFeatureNoProductTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.STRING;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Rojo";

            service.AddProductFeature(productFeature);

            List<ProductFeature> productFeatures = service.GetAllProductFeaturesFromProduct(p);

        }

        [ExpectedException(typeof(ProductFeatureWrongValueException))]
        [TestMethod]
        public void ProductAddFeatureOtherTypeInStringTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.STRING;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "10";

            service.AddProductFeature(productFeature);
        }

        [ExpectedException(typeof(ProductFeatureWrongValueException))]
        [TestMethod]
        public void ProductAddFeatureOtherTypeInIntTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.INT;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Hola";

            service.AddProductFeature(productFeature);
        }

     

        [ExpectedException(typeof(ProductFeatureWrongValueException))]
        [TestMethod]
        public void ProductAddFeatureOtherTypeInInDateTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.DATE;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Hola";

            service.AddProductFeature(productFeature);
        }

        [ExpectedException(typeof(ProductFeatureNoValueException))]
        [TestMethod]
        public void ProductAddFeatureNoValueTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.INT;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;

            service.AddProductFeature(productFeature);           

        }
        [ExpectedException(typeof(ProductFeatureDuplicateFeature))]
        [TestMethod]
        public void ProductAddFeatureTwiceTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.STRING;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Rojo";

            ProductFeature productFeature2 = new ProductFeature();
            productFeature2.ProductId = p.Id;
            productFeature2.FeatureId = f.Id;
            productFeature2.Value = "Azul";

            service.AddProductFeature(productFeature);
            service.AddProductFeature(productFeature2);

        }

        [TestMethod]
        public void ProductModifyStringFeatureOkTest() {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.STRING;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Rojo";

            service.AddProductFeature(productFeature);

            service.ModifyProductFeatureValue(productFeature.Id, "Verde");
            List<ProductFeature> productFeatures = service.GetAllProductFeaturesFromProduct(p);
            Assert.AreEqual("Verde", productFeatures.First().Value);
        }

        [TestMethod]
        public void ProductModifyIntFeatureOkTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Peso";
            f.Type = FeatureTypes.INT;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "10";

            service.AddProductFeature(productFeature);

            service.ModifyProductFeatureValue(productFeature.Id, "20");
            List<ProductFeature> productFeatures = service.GetAllProductFeaturesFromProduct(p);
            Assert.AreEqual("20", productFeatures.First().Value);
        }


        [TestMethod]
        public void ProductModifyDateFeatureOkTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.DATE;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = DateTime.Now.ToString();

            service.AddProductFeature(productFeature);

            service.ModifyProductFeatureValue(productFeature.Id, "01/25/1993 17:00:00");
            List<ProductFeature> productFeatures = service.GetAllProductFeaturesFromProduct(p);
            Assert.AreEqual("01/25/1993 17:00:00", productFeatures.First().Value);
        }

        [ExpectedException(typeof(ProductFeatureWrongValueException))]
        [TestMethod]
        public void ProductModifyStringFeatureWrongValueTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.STRING;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Rojo";

            service.AddProductFeature(productFeature);

            service.ModifyProductFeatureValue(productFeature.Id, "10");
        }

        [ExpectedException(typeof(ProductFeatureWrongValueException))]
        [TestMethod]
        public void ProductModifyIntFeatureWrongValueTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.INT;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "10";

            service.AddProductFeature(productFeature);

            service.ModifyProductFeatureValue(productFeature.Id, "hola");
        }

        [ExpectedException(typeof(ProductFeatureWrongValueException))]
        [TestMethod]
        public void ProductModifyDateFeatureWrongValueTest()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.DATE;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = DateTime.Now.ToString();

            service.AddProductFeature(productFeature);

            service.ModifyProductFeatureValue(productFeature.Id, "10");
        }


        [TestMethod]
        public void ProductDeleteFeatureOk() {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.INT;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "11";

            service.AddProductFeature(productFeature);

            service.RemoveFeatureFromProduct(p, f);
            List<ProductFeature> productFeatures = service.GetAllProductFeaturesFromProduct(p);
            Assert.AreEqual(0,productFeatures.Count());
        }

        [ExpectedException(typeof(ProductNotExistingException))]
        [TestMethod]
        public void ProductDeleteFeatureNoProduct()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.INT;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.RemoveFeatureFromProduct(p, f);
        }

        [ExpectedException(typeof(NoFeatureException))]
        [TestMethod]
        public void ProductDeleteFeatureNoFeature()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.INT;

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            service.RemoveFeatureFromProduct(p, f);
        }

        [ExpectedException(typeof(ProductWithoutFeatureException))]
        [TestMethod]
        public void ProductDeleteFeatureNoProductFeature()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.INT;
            featureService.Add(f);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            service.RemoveFeatureFromProduct(p, f);
        }

        [TestMethod]
        public void ProductGetOk()
        {
            ProductService service = getService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            Product savedProduct = service.Get(p.Id);
            Assert.IsNotNull(savedProduct);
        }

        [TestMethod]
        public void ProductGetCheckFeaturesOk()
        {
            ProductService service = getService();
            FeatureService featureService = getFeatureService();

            Feature f = new Feature();
            f.Name = "Color";
            f.Type = FeatureTypes.STRING;
            featureService.Add(f);

            Feature f2 = new Feature();
            f2.Name = "Peso";
            f2.Type = FeatureTypes.INT;
            featureService.Add(f2);

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;
            service.Add(p);

            ProductFeature productFeature = new ProductFeature();
            productFeature.ProductId = p.Id;
            productFeature.FeatureId = f.Id;
            productFeature.Value = "Rojo";

            service.AddProductFeature(productFeature);

            ProductFeature productFeature2 = new ProductFeature();
            productFeature2.ProductId = p.Id;
            productFeature2.FeatureId = f2.Id;
            productFeature2.Value = "100";

            service.AddProductFeature(productFeature2);

            Product savedProduct = service.Get(p.Id);
            Assert.AreEqual(2, savedProduct.ProductFeatures.Count());
        }

        [ExpectedException(typeof(ProductNotExistingException))]
        [TestMethod]
        public void ProductGetNotExistingTest() {
            ProductService service = getService();

            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            Category cat = getCategory();
            p.Category = cat;

            Product savedProduct = service.Get(p.Id);

        }


        /*  [TestMethod]
          public void ProductGetAllNoUserOk() {
              ProductService service = getService();
              Product p = new Product();
              p.Code = "1111";
              p.Description = "Desc";
              p.Manufacturer = "Manu";
              p.Name = "Product 1";
              p.Price = 100;
              p.Category = getCategory();
              service.Add(p);

              Product p2 = new Product();
              p2.Code = "2222";
              p2.Description = "Desc";
              p2.Manufacturer = "Manu";
              p2.Name = "Product 2";
              p2.Price = 100;
              p2.Category = getCategory();
              service.Add(p2);

              Product p3 = new Product();
              p3.Code = "3333";
              p3.Description = "Desc";
              p3.Manufacturer = "Manu";
              p3.Name = "Product 3";
              p3.Price = 100;
              p3.Category = getCategory();
              service.Add(p3);
          }
          */

        //GET ALL 
        //Ok (no user)
        //Ok (user, with reviews)
        //Wrong User (Idem no user)


        //GET Filtered


        //GET MOST SOLD
        //ADD PICTURE
        //DELETE PICTURE





    }
}
