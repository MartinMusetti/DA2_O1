using DataAccess;
using Entities;
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
    public class AddresServiceTests
    {
        private GenericRepository<Address> addressRepo;
        private GenericRepository<User> userRepo;

        private MyContext context;
        private MyContext getContext() {
            if (context == null)
                context = new MyContext();
            return context;
        }

        private GenericRepository<User> getUserRepo() {
            if (userRepo == null) {
                GenericRepository<User> newUserRepo = new GenericRepository<User>(getContext());
                userRepo = newUserRepo;
                return newUserRepo;
            }
            return userRepo;
        }

        private GenericRepository<Address> getAddressRepo()
        {
            if (addressRepo == null)
            {
                GenericRepository<Address> newAddressRepo = new GenericRepository<Address>(getContext(),true);
                addressRepo = newAddressRepo;
                return newAddressRepo;
            }
            return addressRepo;
        }

        private AddressService getService()
        {

            return new AddressService(getAddressRepo(), getUserRepo());


        }
        private UserService getUserService()
        {
            GenericRepository<Order> repo = new GenericRepository<Order>(getContext());
            return new UserService(getUserRepo(),repo,getAddressRepo());
        }

        private User getUser() {
            User u = new User();
            u.FirstName = "Matias";
            u.LastName = "Grunwaldt";
            u.PhoneNumber = "+59894606123";
            u.Password = "prueba1234";
            u.Email = "matigru@gmail.com";
            u.Username = "Mati";
            Address ad = new Address();
            ad.Street = "Carlos Butler";
            ad.StreetNumber = "1921";
            ad.PhoneNumber = "26007263";
            UserService service = getUserService();
            service.Register(u, ad);
            return u;
        }

        private User getOtherUser()
        {
            User u = new User();
            u.FirstName = "Juan";
            u.LastName = "Grunwaldt";
            u.PhoneNumber = "+59894606123";
            u.Password = "prueba1234";
            u.Email = "juan@gmail.com";
            u.Username = "Juan";
            Address a = new Address();
            a.Street = "Jamaica";
            a.StreetNumber = "1921";
            a.PhoneNumber = "26007263";
            UserService service = getUserService();
            service.Register(u, a);
            return u;
        }

        [TestMethod]
        public void AddressCreateOkTest() {
            AddressService service = getService();
            User u = getUser();
            Address a1 = new Address();
            a1.Street = "Copacabana";
            a1.StreetNumber = "3345";
            a1.PhoneNumber = "26001564";

            service.AddAddress(a1, u);
            Assert.AreNotEqual(Guid.Empty, a1.Id);

            GenericRepository<User> ur = getUserRepo();
            User savedUser = ur.Get(u.Id);
            List<Address> userAddresses = savedUser.Addresses.ToList();
            bool hasAddress = userAddresses.Exists(address => address.Id == a1.Id);
            Assert.IsTrue(hasAddress);
        }

        [ExpectedException(typeof(NoUserForAddressException))]
        [TestMethod]
        public void AddressCreateNoUserTest()
        {
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";
            AddressService service = getService();
            User u = new User();
            service.AddAddress(a, u);
        }

        [ExpectedException(typeof(MissingAddressDataException))]
        [TestMethod]
        public void AddressCreateNoStreetTest()
        {
            Address a = new Address();
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";
            AddressService service = getService();
            User u = getUser();
            service.AddAddress(a, u);
        }

        [ExpectedException(typeof(MissingAddressDataException))]
        [TestMethod]
        public void AddressCreateNoStreetNumberTest()
        {
            Address a = new Address();
            a.Street = "Cartagena";
            a.PhoneNumber = "26003564";
            AddressService service = getService();
            User u = getUser();
            service.AddAddress(a, u);
        }

        [ExpectedException(typeof(MissingAddressDataException))]
        [TestMethod]
        public void AddressCreateNoPhoneNumberTest()
        {
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            AddressService service = getService();
            User u = getUser();
            service.AddAddress(a, u);
        }

        [TestMethod]
        public void AddExistingAddressToDifferentUserTest()
        {
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";

            Address a2 = new Address();
            a2.Street = "Cartagena";
            a2.StreetNumber = "1582";
            a2.PhoneNumber = "26003564";
            AddressService service = getService();
            User u = getUser();
            User u2 = getOtherUser();
            service.AddAddress(a, u);
            service.AddAddress(a2, u2);

            GenericRepository<User> ur = getUserRepo();
            User savedUserOne = ur.Get(u.Id);
            User savedUserTwo = ur.Get(u2.Id);


            List<Address> userOneAddresses = savedUserOne.Addresses.ToList();
            List<Address> userTwoAddresses = savedUserTwo.Addresses.ToList();


            bool userOneHasAddress = userOneAddresses.Exists(address => address.Id == a.Id);
            bool userTwoHasAddress = userTwoAddresses.Exists(address => address.Id == a.Id);

            Assert.IsTrue(userOneHasAddress);
            Assert.IsTrue(userTwoHasAddress);
        }
                
        [TestMethod]
        public void AddressAddMultipleToOneUserTest()
        {
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";

            Address a2 = new Address();
            a2.Street = "Miami";
            a2.StreetNumber = "2222";
            a2.PhoneNumber = "26203564";

            Address a3 = new Address();
            a3.Street = "Cooper";
            a3.StreetNumber = "1111";
            a3.PhoneNumber = "26204564";


            AddressService service = getService();
            User u = getUser();
            service.AddAddress(a, u);
            service.AddAddress(a2, u);
            service.AddAddress(a3, u);

            GenericRepository<User> ur = getUserRepo();
            User savedUser = ur.Get(u.Id);

            List<Address> userOneAddresses = savedUser.Addresses.ToList();
            bool userHasAddressOne = userOneAddresses.Exists(address => address.Id == a.Id);
            bool userHasAddressTwo = userOneAddresses.Exists(address => address.Id == a2.Id);
            bool userHasAddressThree = userOneAddresses.Exists(address => address.Id == a3.Id);


            Assert.IsTrue(userHasAddressOne);
            Assert.IsTrue(userHasAddressTwo);
            Assert.IsTrue(userHasAddressThree);
        }

        [ExpectedException(typeof(UserAlreadyHasAddressException))]
        [TestMethod]
        public void AddressAddSameToUserTest() {
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";

            Address a2 = new Address();
            a2.Street = "Cartagena";
            a2.StreetNumber = "1582";
            a2.PhoneNumber = "26003564";

            AddressService service = getService();
            User u = getUser();

            service.AddAddress(a, u);
            service.AddAddress(a2, u);

        }

        [TestMethod]
        public void AddressDeleteListAddressOkTest() {

            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";

            Address a2 = new Address();
            a2.Street = "Maximo Tajes";
            a2.StreetNumber = "2221";
            a2.PhoneNumber = "26000000";

            AddressService service = getService();
            User u = getUser();

            service.AddAddress(a, u);
            service.AddAddress(a2, u);

            service.RemoveAddress(a2.Id, u);
            List<Address> allAddresses = service.GetAllAddresses(u);
            Address deleted = allAddresses.Find(address => address.Id == a2.Id);
            Assert.IsNull(deleted);

        }

        [TestMethod]
        public void AddressDeleteDefaultOk() {
            AddressService service = getService();
            User u = getUser();

            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";

            service.AddAddress(a, u);
            service.RemoveAddress(u.Address.Id, u);

            Assert.AreEqual(u.Address.Id, a.Id);
        }

        [ExpectedException(typeof(AddressDeleteDefaultNoReplacementException))]
        [TestMethod]
        public void AddressDeleteDefaultNoReplacementTest()
        {
            AddressService service = getService();
            User u = getUser();
            service.RemoveAddress(u.Address.Id, u);
        }
        [ExpectedException(typeof(NoUserForAddressException))]
        [TestMethod]
        public void AddressDeleteNoUserTest()
        {
            AddressService service = getService();
            User u = new User();
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";
            service.RemoveAddress(a.Id, u);
        }

        [ExpectedException (typeof(AddressDeleteNoAddressException))]
        [TestMethod]
        public void AddressDeleteNoAddressTest() {
            AddressService service = getService();
            User u = getUser();
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";
            service.RemoveAddress(a.Id, u);
        }

        [ExpectedException (typeof(AddressDeleteUserDoesntHaveException))]
        [TestMethod]
        public void AddressDeleteUserDoesntHaveTest()
        {
            AddressService service = getService();
            User u = getUser();
            User u2 = getOtherUser();
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";
            service.AddAddress(a, u);
            service.RemoveAddress(a.Id, u2);
        }

        [TestMethod]
        public void GetAllFromUser() {
            AddressService service = getService();
            User u = getUser();
            Address a = new Address();
            a.Street = "Cartagena";
            a.StreetNumber = "1582";
            a.PhoneNumber = "26003564";

            Address a2 = new Address();
            a2.Street = "Maximo Tajes";
            a2.StreetNumber = "2221";
            a2.PhoneNumber = "26000000";
            service.AddAddress(a, u);
            service.AddAddress(a2, u);
            List<Address> allAddresses = service.GetAllAddresses(u);
            Assert.AreEqual(3, allAddresses.Count);
        }




    }
}
