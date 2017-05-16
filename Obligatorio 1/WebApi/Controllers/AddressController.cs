using Entities;
using Exceptions;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class AddressController : ApiController
    {
        private IAddressService _addressService;
        private IUserService _userService;

        public AddressController(IAddressService service, IUserService userService)
        {
            _addressService = service;
            _userService = userService;
        }
        public IHttpActionResult Post([FromBody] Address address)
        {
            try
            {
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("Token"))
                {
                    string token = headers.GetValues("Token").First();
                    User loggedUser = _userService.GetFromToken(token);
                    _addressService.AddAddress(address, loggedUser);
                    return Ok("Se agregó la dirección con Id " + address.Id + " al usuario con id " + loggedUser.Id);
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MissingAddressDataException ex) {
                return BadRequest(ex.Message);
            }
            catch (UserAlreadyHasAddressException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest("Debes enviar todos los datos") ;
            }
        }


        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("Token"))
                {
                    string token = headers.GetValues("Token").First();
                    User loggedUser = _userService.GetFromToken(token);
                    _addressService.RemoveAddress(id, loggedUser);
                    return Ok("Se le quitó la dirección " + id + " al usuario " + loggedUser.Id);
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AddressDeleteNoAddressException ex) {
                return BadRequest(ex.Message);
            }
            catch (AddressDeleteDefaultNoReplacementException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AddressDeleteUserDoesntHaveException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IHttpActionResult Get()
        {
            try
            {
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("Token"))
                {
                    string token = headers.GetValues("Token").First();
                    User loggedUser = _userService.GetFromToken(token);
                    List<Address> allAddresses = _addressService.GetAllAddresses(loggedUser);
                    List<Address> addressesToShow = new List<Address>();
                    foreach (Address add in allAddresses) {
                        Address a = new Address();
                        a.Id = add.Id;
                        a.Street = add.Street;
                        a.StreetNumber = add.StreetNumber;
                        a.PhoneNumber = add.PhoneNumber;
                        addressesToShow.Add(a);
                    }
                    return Ok(addressesToShow);
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /*
         public IHttpActionResult Get()
        {
            IEnumerable<Breed> breeds = breedsBusinessLogic.GetAllBreeds();
            if (breeds == null)
            {
                return NotFound();
            }
            return Ok(breeds);
        }
         */
    }
}
