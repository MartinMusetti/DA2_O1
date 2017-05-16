using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Entities;
using Moq;
using Services.Interfaces;
using WebApi.Controllers;
using System.Web.Http;
using System.Web.Http.Results;
using Exceptions;
using Newtonsoft.Json.Linq;

using System.Web.Http.Controllers;
using System.Net.Http;
using Entities.Statuses_And_Roles;
namespace WebApiTests
{
    [TestClass]
    public class UserControllerTests
    {
        private User getFakeUser() {
            User u = new User();
            u.FirstName = "Luis";
            u.LastName = "Suarez";
            u.Password = "contra1234";
            u.PhoneNumber = "094555000";
            u.Username = "luchoTricolor";
            u.Email = "suarez@barcelona.com";
            return u;
        }

        private Address getFakeAddress() {
            Address a = new Address();
            a.Street = "Rambla";
            a.StreetNumber = "1256";
            a.PhoneNumber = "2555666777";
            return a;
        }


        [TestMethod]
        public void RegisterOkTest()
        {
            User fakeUser = getFakeUser();
            fakeUser.Address = getFakeAddress();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Register(fakeUser,fakeUser.Address));

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Register(fakeUser);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Register", createdResult.RouteName);
            Assert.AreEqual(fakeUser.Id, createdResult.RouteValues["id"]);
            Assert.AreEqual(fakeUser, createdResult.Content);
        }

        [TestMethod]
        public void RegisterMissingDataTest()
        {
            User fakeUser = new User();
            fakeUser.Address = getFakeAddress();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Register(fakeUser, fakeUser.Address)).Throws(new MissingUserDataException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Register(fakeUser);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void RegisterWrongEmailTest()
        {
            User fakeUser = new User();
            fakeUser.Address = getFakeAddress();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Register(fakeUser, fakeUser.Address)).Throws(new WrongEmailFormatException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Register(fakeUser);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void RegisterExistingUserTest()
        {
            User fakeUser = new User();
            fakeUser.Address = getFakeAddress();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Register(fakeUser, fakeUser.Address)).Throws(new ExistingUserException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Register(fakeUser);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void RegisterWrongPasswordTest()
        {
            User fakeUser = new User();
            fakeUser.Address = getFakeAddress();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Register(fakeUser, fakeUser.Address)).Throws(new WrongPasswordException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Register(fakeUser);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void RegisterWrongNumberFormatTest()
        {
            User fakeUser = new User();
            fakeUser.Address = getFakeAddress();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Register(fakeUser, fakeUser.Address)).Throws(new WrongNumberFormatException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Register(fakeUser);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void RegisterWrongAddressTest()
        {
            User fakeUser = new User();
            fakeUser.Address = new Address();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Register(fakeUser, fakeUser.Address)).Throws(new MissingAddressDataException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Register(fakeUser);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void RegisterNullTest()
        {
            User fakeUser = new User();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Register(fakeUser, fakeUser.Address)).Throws(new NullReferenceException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Register(fakeUser);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }


        [TestMethod]
        public void LoginEmailOkTest()
        {
            dynamic parameters = new JObject();
            parameters.Email = "matigru@gmail.com";
            parameters.Password = "ASDF";
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Login("matigru@gmail.com", "ASDF")).Returns("tokenNuevo");

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Login(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Loggueado con éxito, el token de seguridad es tokenNuevo", createdResult.Content);
        }

        [TestMethod]
        public void LoginUsernameOkTest()
        {
            dynamic parameters = new JObject();
            parameters.Username = "matigru";
            parameters.Password = "ASDF";
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Login("matigru", "ASDF")).Returns("tokenNuevo");

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Login(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Loggueado con éxito, el token de seguridad es tokenNuevo", createdResult.Content);
        }

        [TestMethod]
        public void LoginWrongPasswordTest()
        {
            dynamic parameters = new JObject();
            parameters.Username = "matigru";
            parameters.Password = "ASDF";
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Login("matigru", "ASDF")).Throws(new NoLoginDataMatchException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Login(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void LoginNoUserTest()
        {
            dynamic parameters = new JObject();
            parameters.Username = "matigru";
            parameters.Password = "ASDF";
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Login("matigru", "ASDF")).Throws(new NotExistingUserException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Login(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void LoginNullTest()
        {
            var mockUserService = new Mock<IUserService>();

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Login(null);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void LoginNullInfoTest()
        {
            var mockUserService = new Mock<IUserService>();
            dynamic parameters = new JObject();
            parameters.Username = null;
            parameters.Password = null;
            mockUserService.Setup(service => service.Login(null, null)).Throws(new NullReferenceException());

            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult obtainedResult = controller.Login(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void LogoutTest() {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.Logout(u.Id));

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            IHttpActionResult obtainedResult = controller.Logout();
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Desloggueado con éxito", createdResult.Content);

        }

        [TestMethod]
        public void LogoutTestNoToken()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            IHttpActionResult obtainedResult = controller.Logout();
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void LogoutTestWrongToken()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Throws(new NoUserWithTokenException());

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            IHttpActionResult obtainedResult = controller.Logout();
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangeUserRoleTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.ChangeUserRole(u.Id,UserRoles.ADMIN));

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;


            dynamic parameters = new JObject();
            parameters.UserRole = "2";
            parameters.Id = u.Id.ToString();


            IHttpActionResult obtainedResult = controller.ChangeUserRole(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Rol cambiado con éxito", createdResult.Content);

        }


        [TestMethod]
        public void ChangeUserRoleNoUserLoggedInTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Throws(new NoUserWithTokenException());
            
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;


            dynamic parameters = new JObject();
            parameters.UserRole = "2";
            parameters.Id = u.Id.ToString();


            IHttpActionResult obtainedResult = controller.ChangeUserRole(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangeUserRoleNotSuperadminTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.ADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;


            dynamic parameters = new JObject();
            parameters.UserRole = "2";
            parameters.Id = u.Id.ToString();


            IHttpActionResult obtainedResult = controller.ChangeUserRole(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangeUserRoleNoUserTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.ChangeUserRole(u.Id, UserRoles.ADMIN)).Throws(new NotExistingUserException());

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;


            dynamic parameters = new JObject();
            parameters.UserRole = "2";
            parameters.Id = u.Id.ToString();


            IHttpActionResult obtainedResult = controller.ChangeUserRole(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangeUserRoleNoDataTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            IHttpActionResult obtainedResult = controller.ChangeUserRole(null);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangeUserRoleWrongDataTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            dynamic parameters = new JObject();
            parameters.UserRole = null;
            parameters.Id = null;


            IHttpActionResult obtainedResult = controller.ChangeUserRole(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangeUserRoleWrongIdTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            dynamic parameters = new JObject();
            parameters.UserRole = UserRoles.ADMIN;
            parameters.Id = "a";


            IHttpActionResult obtainedResult = controller.ChangeUserRole(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangeUserRoleStringRoleTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;


            dynamic parameters = new JObject();
            parameters.UserRole = "admin";
            parameters.Id = u.Id.ToString();


            IHttpActionResult obtainedResult = controller.ChangeUserRole(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));


        }

        [TestMethod]
        public void ChangeUserRoleNotExistingRoleTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.ChangeUserRole(u.Id, 5)).Throws(new NotExistingUserRoleException());

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;


            dynamic parameters = new JObject();
            parameters.UserRole = "5";
            parameters.Id = u.Id.ToString();


            IHttpActionResult obtainedResult = controller.ChangeUserRole(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ModifyUserOkTest() {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            u.FirstName = "Hola";
            mockUserService.Setup(service => service.Modify(u));

            IHttpActionResult obtainedResult = controller.Put(u.Id,u);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(u.Id, createdResult.RouteValues["id"]);
            Assert.AreEqual(u, createdResult.Content);

        }

        [TestMethod]
        public void ModifyUserNoTokenTest() {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            IHttpActionResult obtainedResult = controller.Put(u.Id,u);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ModifyUserDifferentUserThanTokenTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            IHttpActionResult obtainedResult = controller.Put(Guid.NewGuid(), u);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ModifyUserNoUserWithTokenTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Throws(new NoUserWithTokenException());

            IHttpActionResult obtainedResult = controller.Put(Guid.NewGuid(), u);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }


        [TestMethod]
        public void ModifyUserMissingDataTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            u.FirstName = null;
            mockUserService.Setup(service => service.Modify(u)).Throws(new MissingUserDataException());

            IHttpActionResult obtainedResult = controller.Put(u.Id, u);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ModifyUserExistingUserTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.Modify(u)).Throws(new ExistingUserException());

            IHttpActionResult obtainedResult = controller.Put(u.Id, u);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ModifyUserWrongNumberFormatTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.Modify(u)).Throws(new WrongNumberFormatException());

            IHttpActionResult obtainedResult = controller.Put(u.Id, u);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ModifyUserWrongEmailFormatTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.Modify(u)).Throws(new WrongEmailFormatException());

            IHttpActionResult obtainedResult = controller.Put(u.Id, u);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ModifyUserNullUserTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            IHttpActionResult obtainedResult = controller.Put(u.Id, null);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void DeleteUserOkTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.Delete(u.Id));

            IHttpActionResult obtainedResult = controller.Delete(u.Id);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Se eliminó al usuario con éxito", createdResult.Content);

        }

        [TestMethod]
        public void DeleteUserNoTokenTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            IHttpActionResult obtainedResult = controller.Delete(u.Id);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void DeleteOtherUserTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            IHttpActionResult obtainedResult = controller.Delete(Guid.NewGuid());
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void DeleteUserNoUserWithTokenTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Throws(new NoUserWithTokenException());

            IHttpActionResult obtainedResult = controller.Delete(Guid.NewGuid());
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangePasswordOk() {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.ChangePassword(u.Id, "old","new"));

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;


            dynamic parameters = new JObject();
            parameters.OldPassword = "old";
            parameters.NewPassword = "new";


            IHttpActionResult obtainedResult = controller.ChangePassword(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Se cambió la contraseña con éxito", createdResult.Content);
        }

        [TestMethod]
        public void ChangePasswordNoUserWithTokenTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Throws(new NoUserWithTokenException());

            dynamic parameters = new JObject();
            parameters.OldPassword = "old";
            parameters.NewPassword = "new";

            IHttpActionResult obtainedResult = controller.ChangePassword(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }
        [TestMethod]
        public void ChangePasswordNoTokenTest()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            dynamic parameters = new JObject();
            parameters.OldPassword = "old";
            parameters.NewPassword = "new";

            IHttpActionResult obtainedResult = controller.ChangePassword(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ChangePasswordWrongPassword()
        {
            var mockUserService = new Mock<IUserService>();
            User u = getFakeUser();
            u.Role = UserRoles.SUPERADMIN;
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.ChangePassword(u.Id, "old", "new")).Throws(new WrongPasswordException());

            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;


            dynamic parameters = new JObject();
            parameters.OldPassword = "old";
            parameters.NewPassword = "new";


            IHttpActionResult obtainedResult = controller.ChangePassword(parameters);
            var createdResult = obtainedResult as OkNegotiatedContentResult<String>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));

        }

        [TestMethod]
        public void ChangePasswordNullDataTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);

            IHttpActionResult obtainedResult = controller.ChangePassword(null);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ChangePasswordNullPasswordTest()
        {
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add("Token", "aheup9obyd8xnu3xsd1lnxljgx8j7vt1");
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            User u = getFakeUser();
            mockUserService.Setup(service => service.GetFromToken("aheup9obyd8xnu3xsd1lnxljgx8j7vt1")).Returns(u);
            mockUserService.Setup(service => service.ChangePassword(u.Id, null, "new")).Throws(new ArgumentNullException());
            dynamic parameters = new JObject();
            parameters.OldPassword = null;
            parameters.NewPassword = "new";

            IHttpActionResult obtainedResult = controller.ChangePassword(parameters);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<User>;

            mockUserService.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }



    }
}
