using System.Collections.Generic;

namespace SEG.Core.Communication
{
    public class ResponseResult
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public ResponseErrorMessages Errors { get; set; }
        private object ResponseObject { get; set; }


        public ResponseResult()
        {
            Errors = new ResponseErrorMessages();
        }  
        
        public void AtribuirResponseObject<T>(T responseObjetc)
        {
            ResponseObject = responseObjetc;
        }

        public T ObterResponseObject<T> ()
        {
            return (T)ResponseObject;
        }
    }

    public class ResponseErrorMessages
    {
        public List<string> Mensagens { get; set; }

        public ResponseErrorMessages()
        {
            Mensagens = new List<string>();
        }        
    }
}