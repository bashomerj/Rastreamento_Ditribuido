using Cliente.API.Models.Interfaces;
using Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cliente.API.Models.Entities
{
    public class Cliente : Entity, ICliente, IAggregateRoot
    {

        public Guid id { get; private set; }
        public string nome { get; private set; }
        public string numeroCpf { get; private set; }
        public DateTime dataNascimento { get; private set; }
        public string sexo { get; private set; }


        protected Cliente(){}

       
    }
}
