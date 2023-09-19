using System;

namespace ReMIND.Server.Helpers
{
    public class ReMindException: Exception 
    {
        public string message;

        public ReMindException() {

        }

        public ReMindException(string message): base(message) {
            this.message = message;
        }

        public ReMindException(string message, Exception inner): base(message, inner) {
            
        }
        
    }
}