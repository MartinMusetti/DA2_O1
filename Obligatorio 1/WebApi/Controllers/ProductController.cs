using Entities;
using Entities.Statuses_And_Roles;
using Exceptions;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class ProductController : ApiController
    {
        private IProductService _productService;
        private IUserService _userService;

        public ProductController(IProductService service, IUserService userService)
        {
            _productService = service;
            _userService = userService;
        }

        public IHttpActionResult Post([FromBody] Product product)
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
                        _productService.Add(product);
                        return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);

                    }
                    return BadRequest("Solo los administradores pueden agregar productos");
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductMissingDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductWrongPriceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductDuplicateException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotExistingCategoryException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest("Debes enviar todos los datos");
            }
        }

        public IHttpActionResult Put(Guid id, [FromBody]Product product)
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
                        Product existing = _productService.Get(id);
                        existing.Category = product.Category;
                        existing.Code = product.Code;
                        existing.Description = product.Description;
                        existing.Manufacturer = product.Manufacturer;
                        existing.Name = product.Name;
                        existing.Price = product.Price;
                        _productService.Modify(existing);
                        return CreatedAtRoute("DefaultApi", new { id = existing.Id }, existing);

                    }
                    return BadRequest("Solo los administradores pueden modificar productos");
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductNotExistingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductMissingDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductWrongPriceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductDuplicateException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotExistingCategoryException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest("Debes enviar todos los datos");
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
                    if (loggedUser.Role == UserRoles.ADMIN || loggedUser.Role == UserRoles.SUPERADMIN)
                    {
                        _productService.Delete(id);
                        return Ok("Se quitó el producto " + id);

                    }
                    return BadRequest("Solo los administradores pueden eliminar productos");
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductNotExistingException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("api/Product/ChangeCategory", Name = "ChangeCategory")]
        public IHttpActionResult ChangeCategory(JObject parameters)
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
                        dynamic json = parameters;
                        string productIdString = json.ProductId;
                        if (productIdString != null) {
                            string categoryIdString = json.CategoryId;
                            Guid productId = new Guid(productIdString);
                            if (categoryIdString != null)
                            {
                                Guid categoryId = new Guid(categoryIdString);
                                _productService.ChangeCategory(productId, categoryId);
                                return Ok("Al producto " + productIdString + " se le asignó la categoría " + categoryIdString);
                            }
                            else {
                                _productService.ChangeCategory(productId, Guid.Empty);
                                return Ok("Al producto " + productIdString + " se le sacó la categoría");
                            }

                        }
                        return BadRequest("Debes enviar un Id de Producto");
                    }
                    return BadRequest("Solo los administradores pueden cambiar categorías de productos");
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductNotExistingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductChangeCategoryException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest("Por favor verifique que los ids tienen 32 caracteres");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest("Debes enviar todos los datos");
            }
        }
                    /*
                    [HttpPost]
                    public IHttpActionResult ChangePassword(JObject parameters)
                    {
                        try
                        {
                            var re = Request;
                            var headers = re.Headers;

                            if (headers.Contains("Token"))
                            {
                                string token = headers.GetValues("Token").First();
                                User loggedUser = _userService.GetFromToken(token);
                                dynamic json = parameters;
                                string oldPassword = json.OldPassword;
                                string newPassword = json.NewPassword;
                     */


                }
}
