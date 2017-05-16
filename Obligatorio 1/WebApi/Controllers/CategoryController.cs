using Entities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entities.Statuses_And_Roles;
using Exceptions;

namespace WebApi.Controllers
{
    public class CategoryController : ApiController
    {
        private ICategoryService _categoryService;
        private IUserService _userService;

        public CategoryController(ICategoryService service, IUserService userService)
        {
            _categoryService = service;
            _userService = userService;
        }

        public IHttpActionResult Post([FromBody] Category category)
        {
            try
            {
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("Token"))
                {
                    string token = headers.GetValues("Token").First();
                    User loggedUser = _userService.GetFromToken(token);
                    if (loggedUser.Role == UserRoles.ADMIN || loggedUser.Role == UserRoles.SUPERADMIN) {
                        _categoryService.Add(category);
                        return CreatedAtRoute("DefaultApi", new { id = category.Id }, category);

                    }
                    return BadRequest("Solo los administradores pueden agregar categorías");
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MissingCategoryDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ExistingCategoryNameException ex)
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
                        _categoryService.Delete(id);
                        return Ok("Se quitó la categoría " + id);

                    }
                    return BadRequest("Solo los administradores pueden eliminar categorías");
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotExistingCategoryException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IHttpActionResult Put(Guid id, [FromBody]Category category)
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
                        Category existing = _categoryService.Get(id);
                        _categoryService.Modify(existing, category.Name, category.Description);
                        return CreatedAtRoute("DefaultApi", new { id = existing.Id }, existing);

                    }
                    return BadRequest("Solo los administradores pueden modificar categorías");
                }
                return BadRequest("Debes mandar el Token de sesión en los headers");
            }
            catch (NoUserWithTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ExistingCategoryNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MissingCategoryDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotExistingCategoryException ex) {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
