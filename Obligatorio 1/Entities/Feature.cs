using System;
using System.ComponentModel.DataAnnotations;
using Exceptions;
using Entities.Statuses_And_Roles;

namespace Entities
{
    public class Feature
    {
        public Feature() {
            this.Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        public void Validate()
        {
            validateName();
            validateType();
        }
        private void validateName() {
            if (Name == null || Name.Trim() == "")
                throw new FeatureNoNameException("Un atributo debe tener nombre");
        }
        private void validateType()
        {
            if (Type == 0)
                throw new FeatureWithoutTypeException("Un atributo debe tener un tipo numérico");
            if (Type != FeatureTypes.INT && Type != FeatureTypes.STRING && Type != FeatureTypes.DATE)
                throw new FeatureWrongTypeException("Los tipos de atributo pueden ser: '1' para Texto '2' para un número y '3' para Fecha");
        }
    }
}