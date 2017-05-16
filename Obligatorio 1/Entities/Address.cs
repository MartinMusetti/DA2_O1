using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exceptions;

namespace Entities
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }

        public String Street { get; set; }
        public String StreetNumber { get; set; }
        public String PhoneNumber { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Address()
        {
            this.Id = Guid.NewGuid();
            this.Users = new List<User>();
        }

        public void Validate() {
            validateAddress();
            
        }

        public void CheckForExisting() {
            
        }

        private void validateAddress()
        {
            if (Street == null || Street.Trim() == "")
            {
                throw new MissingAddressDataException("No se puede dejar la calle vacía");
            }
            if (StreetNumber == null || StreetNumber.Trim() == "")
            {
                throw new MissingAddressDataException("No se puede dejar el número de puerta vacío");
            }
            if (PhoneNumber == null || PhoneNumber.Trim() == "")
            {
                throw new MissingAddressDataException("No se puede dejar el teléfono vacío");
            }
        }

       /* public override bool Equals(object obj)
        {
            var type = obj.GetType();
            if (obj.GetType() == typeof(Address))
            {
                Address a = (Address)obj;
                return a.Street == this.Street && a.StreetNumber == this.StreetNumber && a.PhoneNumber == this.PhoneNumber;
            }
            return false;
        }
        */

    }
}
