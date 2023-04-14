using FluentValidation.Results;

namespace Core.Messages.Integration
{
    public class ResponseMessage : Message
    {
        public ValidationResult ValidationResult { get; set; }
        private object ResponseObject { get; set; }
        //public object Result { get; set; }

        public ResponseMessage(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
               

        public void AtribuirResponseObject(object responseObjetc)
        {
            ResponseObject = responseObjetc;
        }

        public object ObterResponseObject()
        {
            return ResponseObject;
        }
    }

}