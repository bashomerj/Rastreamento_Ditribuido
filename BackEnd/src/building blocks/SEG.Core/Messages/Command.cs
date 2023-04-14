using System;
using FluentValidation.Results;
using MediatR;

namespace SEG.Core.Messages
{
    public abstract class Command : Message, IRequest<ValidationResult>
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }

        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class CommandResponse : Message, IRequest<ResponseDateValidation>
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }
  
        protected CommandResponse()
        {
            Timestamp = DateTime.Now;
        }

        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }
    }

    public class ResponseDateValidation
    {
        public ValidationResult ValidationResult { get; set; }
        public object DataObject { get; set; }
        public Guid Id { get; set; }
        public int Cdpes;

        public ResponseDateValidation()
        {
        }

        public ResponseDateValidation(ValidationResult validationResult, object dataObject)
        {
            ValidationResult = validationResult;
            DataObject = dataObject;
        }
    }
}