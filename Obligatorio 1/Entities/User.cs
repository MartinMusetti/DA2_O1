﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Exceptions;
namespace Entities
{

    public class User
    {
        public User()
        {
            this.Id = Guid.NewGuid();
            this.Addresses = new List<Address>();
        }

        public string Email { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public virtual Address Address {get;set;}
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public int Role { get; set; }

        [Key]
        public Guid Id { get; set; }

        public void Validate()
        {
            ValidateEmail();
            ValidateName();
            ValidatePassword();
            ValidatePhoneNumber();
            ValidateUsername();
        }

        private void ValidateUsername()
        {
            if (Username == null || Username.Trim() == "")
            {
                throw new MissingUserDataException("No se puede dejar el nombre de usuario vacío");
            }
        }

       

        private void ValidatePhoneNumber()
        {
            if (PhoneNumber == null || PhoneNumber.Trim() == "" )
            {
                throw new MissingUserDataException("No se puede dejar el teléfono vacío");
            }
        }

        private void ValidatePassword()
        {
            if (Password == null || Password.Trim() == "")
            {
                throw new MissingUserDataException("No se puede dejar la contraseña vacía");
            }
        }

        private void ValidateName()
        {
            if (FirstName == null || FirstName.Trim() == "" )
            {
                throw new MissingUserDataException("No se puede dejar el nombre vacío");
            }
            if (LastName == null || LastName.Trim() == ""  )
            {
                throw new MissingUserDataException("No se puede dejar el apellido vacío");
            }
        }

        private void ValidateEmail()
        {
            if (Email == null || Email.Trim() == "")
            {
                throw new MissingUserDataException("No se puede dejar el email vacío");
            }
        }

        
        public virtual ICollection<Address> Addresses { get; set; }
    }
}