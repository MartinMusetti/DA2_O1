using DataAccess;
using Entities;
using Entities.Statuses_And_Roles;
using Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Services;
using Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesTests
{
    [TestClass]
    public class ReviewServiceTests
    {
        private MyContext context;
        private MyContext getContext()
        {
            if (context == null)
                context = new MyContext();
            return context;
        }
        private ReviewService getService()
        {
            GenericRepository<Review> reviewRepoInstance = new GenericRepository<Review>(getContext(), true);
            GenericRepository<Product> productRepoInstance = new GenericRepository<Product>(getContext());
            GenericRepository<User> userRepoInstance = new GenericRepository<User>(getContext());
            GenericRepository<Order> orderRepoInstance = new GenericRepository<Order>(getContext());
            GenericRepository<OrderProduct> orderProductRepoInstance = new GenericRepository<OrderProduct>(getContext());
            return new ReviewService(reviewRepoInstance, productRepoInstance, userRepoInstance, orderRepoInstance, orderProductRepoInstance);
        }

        private CategoryService getCategoryService()
        {
            GenericRepository<Category> repoInstance = new GenericRepository<Category>(getContext());
            GenericRepository<Product> pRepoInstance = new GenericRepository<Product>(getContext());

            return new CategoryService(repoInstance, pRepoInstance);
        }

        private OrderService getOrderService()
        {
            GenericRepository<Order> orderRepo = new GenericRepository<Order>(getContext(), true);
            GenericRepository<OrderProduct> orderProductRepo = new GenericRepository<OrderProduct>(getContext());
            GenericRepository<User> userRepo = new GenericRepository<User>(getContext());
            GenericRepository<Product> productRepo = new GenericRepository<Product>(getContext());
            return new OrderService(orderRepo, orderProductRepo, userRepo, productRepo);
        }

        private UserService getUserService()
        {
            GenericRepository<User> repo = new GenericRepository<User>(getContext());
            GenericRepository<Order> orderRepo = new GenericRepository<Order>(getContext());
            GenericRepository<Address> addressRepo = new GenericRepository<Address>(getContext());
            return new UserService(repo, orderRepo, addressRepo);
        }

        private ProductService getProductService()
        {
            GenericRepository<Product> repoInstance = new GenericRepository<Product>(getContext());
            GenericRepository<ProductFeature> productFeatureRepoInstance = new GenericRepository<ProductFeature>(getContext());
            GenericRepository<Feature> featureRepoInstance = new GenericRepository<Feature>(getContext());
            GenericRepository<OrderProduct> orderProductRepoInstance = new GenericRepository<OrderProduct>(getContext());
            return new ProductService(repoInstance, productFeatureRepoInstance, featureRepoInstance, orderProductRepoInstance);
        }

        private Product generateProduct()
        {
            ProductService service = getProductService();
            Product p = new Product();
            p.Code = "1234";
            p.Description = "Desc";
            p.Manufacturer = "Manu";
            p.Name = "Name";
            p.Price = 100;
            p.Category = getCategory();
            service.Add(p);
            return p;
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

        private User registerUser()
        {
            User u = new User();
            u.FirstName = "Matias";
            u.LastName = "Grunwaldt";
            u.PhoneNumber = "+59894606123";
            u.Password = "prueba1234";
            u.Email = "matigru@gmail.com";
            u.Username = "Mati";
            Address a = new Address();
            a.Street = "Carlos Butler";
            a.StreetNumber = "1921";
            a.PhoneNumber = "26007263";
            UserService userService = getUserService();
            userService.Register(u, a);
            return u;
        }


        [TestMethod]
        public void ReviewOkTest() {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            User u = registerUser();
            Product p = generateProduct();
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            orderService.Pay(orderId);
            string reviewText = "Muy bueno";
            reviewService.Evaluate(u, p.Id, orderId, reviewText);
            Review myReview = reviewService.Get(p.Id, orderId);
            Assert.IsNotNull(myReview);
        }

        [ExpectedException(typeof(NotExistingUserException))]
        [TestMethod]
        public void ReviewNoUserTest()
        {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            User u = registerUser();
            Product p = generateProduct();
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            orderService.Pay(orderId);
            string reviewText = "Muy bueno";
            User u2 = new User();
            u2.Id = Guid.NewGuid();
            reviewService.Evaluate(u2, p.Id, orderId, reviewText);
        }

        [ExpectedException(typeof(NotExistingProductException))]
        [TestMethod]
        public void ReviewNoProductTest()
        {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            User u = registerUser();
            Product p = generateProduct();
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            orderService.Pay(orderId);
            string reviewText = "Muy bueno";
            reviewService.Evaluate(u, Guid.NewGuid(), orderId, reviewText);
        }

        [ExpectedException(typeof(NoTextForReviewException))]
        [TestMethod]
        public void ReviewNoTextTest()
        {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            User u = registerUser();
            Product p = generateProduct();
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            orderService.Pay(orderId);
            string reviewText = "";
            reviewService.Evaluate(u, p.Id, orderId, reviewText);
        }

        [ExpectedException(typeof(NotExistingOrderException))]
        [TestMethod]
        public void ReviewNoOrderTest()
        {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            User u = registerUser();
            Product p = generateProduct();
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            orderService.Pay(orderId);
            string reviewText = "Muy bueno";
            reviewService.Evaluate(u, p.Id, Guid.NewGuid(), reviewText);
        }

        [ExpectedException(typeof(IncorrectOrderStatusException))]
        [TestMethod]
        public void ReviewIncorrectOrderStatusTest()
        {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            User u = registerUser();
            Product p = generateProduct();
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            string reviewText = "Muy bueno";
            reviewService.Evaluate(u, p.Id, orderId, reviewText);
        }

        [ExpectedException(typeof(NotExistingProductInOrderException))]
        [TestMethod]
        public void ReviewNoProductInOrderTest()
        {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            ProductService productService = getProductService();
            User u = registerUser();
            Product p = generateProduct();
            Product p2 = new Product();
            p2.Code = "1235";
            p2.Description = "Desc";
            p2.Manufacturer = "Manu";
            p2.Name = "Name2";
            p2.Price = 120;
            p2.Category = p.Category;
            productService.Add(p2);
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            string reviewText = "Muy bueno";
            reviewService.Evaluate(u, p2.Id, orderId, reviewText);
        }

        [TestMethod]
        public void ReviewLastProductInOrderTest()
        {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            User u = registerUser();
            Product p = generateProduct();
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            orderService.Pay(orderId);
            string reviewText = "Muy bueno";
            reviewService.Evaluate(u, p.Id, orderId, reviewText);
            Order o = orderService.Get(orderId);
            Assert.AreEqual(o.Status, OrderStatuses.FINALIZED);
        }

        [ExpectedException(typeof(ProductAlreadyEvaluatedException))]
        [TestMethod]
        public void ReviewAlreadyReviewedProductTest()
        {
            ReviewService reviewService = getService();
            OrderService orderService = getOrderService();
            ProductService productService = getProductService();
            User u = registerUser();
            Product p = generateProduct();
            Product p2 = new Product();
            p2.Code = "1235";
            p2.Description = "Desc";
            p2.Manufacturer = "Manu";
            p2.Name = "Name2";
            p2.Price = 120;
            p2.Category = p.Category;
            productService.Add(p2);
            Guid orderId = orderService.GetActiveOrderFromUser(u).Id;
            orderService.AddProduct(u, p);
            orderService.AddProduct(u, p2);
            orderService.SetAddress(u, u.Address.Id);
            orderService.Ship(orderId);
            orderService.Pay(orderId);
            string reviewText = "Muy bueno";
            reviewService.Evaluate(u, p.Id, orderId, reviewText);
            reviewService.Evaluate(u, p.Id, orderId, reviewText);
        }

        //produxt id order id texto
        //EVALUATE PRODUCT
        //OK ---------------------
        //No user ------------------
        //NO Product ID ------------------
        //No Text -----------------
        //No order Id --------------
        //Order status != Payed ----------------
        //No product in order ------------------
        //If last product in order, change order status --------------

        // prueba para esto:
        // cuando hago review me fijo que no puede haber un review con ese order id y ese prod id


    }
}
