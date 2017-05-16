using Entities;
using Entities.Statuses_And_Roles;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Exceptions;
namespace WebApi.Controllers
{
    public class FeatureController : ApiController
    {
        private IFeatureService _featureService;
        private IUserService _userService;

        public FeatureController(IFeatureService service, IUserService userService)
        {
            _featureService = service;
            _userService = userService;
        }

        public IHttpActionResult Post([FromBody] Feature feature)
        {
            try
            {
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("Token"))
                {
                    string token = headers.GetValues("Token").First();
                    User loggedUser = _userService.GetFromToken(token);
                    if (loggedUser.Role == UserRoles.ADMIN || loggedUser.Role == UserRoles.SUPERADMIN)
                    {
                        _featureService.Add(feature);
                        return CreatedAtRoute("DefaultApi", new { id = feature.Id }, feature);

                    }
                    return BadRequest("Solo los administradores pueden agregar características");
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FeatureNoNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FeatureWithoutTypeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FeatureWrongTypeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FeatureExistingCombinationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest("Debes enviar todos los datos");
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
                    if (loggedUser.Role == UserRoles.ADMIN || loggedUser.Role == UserRoles.SUPERADMIN)
                    {
                        List<Feature> allFeatures = _featureService.GetAll();
                        return Ok(allFeatures);
                    }
                    return BadRequest("Solo los administradores pueden ver las características");
                  /*  List<Address> allAddresses = _addressService.GetAllAddresses(loggedUser);
                    List<Address> addressesToShow = new List<Address>();
                    foreach (Address add in allAddresses)
                    {
                        Address a = new Address();
                        a.Id = add.Id;
                        a.Street = add.Street;
                        a.StreetNumber = add.StreetNumber;
                        a.PhoneNumber = add.PhoneNumber;
                        addressesToShow.Add(a);
                    }*/
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
