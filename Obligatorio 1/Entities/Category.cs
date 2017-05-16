using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using Exceptions;
namespace Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public Category() {
            this.Id = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Category))
            {
                Category c = (Category)obj;
                return c.Id == this.Id && c.Name == this.Name && c.Description == this.Description;
            }
            return false;
        }

        public void Validate()
        {
            ValidateName();
            ValidateDescription();
        }

        private void ValidateName()
        {
            if (Name == null || Name.Trim() == "")
            {
                throw new MissingCategoryDataException("No se puede dejar el nombre vacío");
            }
        }

        private void ValidateDescription()
        {
            if (Description == null || Description.Trim() == "")
            {
                throw new MissingCategoryDataException("No se puede dejar la descripción vacía");
            }
        }

    }
}