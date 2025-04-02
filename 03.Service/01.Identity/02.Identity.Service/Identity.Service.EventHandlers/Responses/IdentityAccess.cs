using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service.EventHandlers.Responses
{
    public class IdentityAccess
    {
        public bool Succeded { get; set; }
        public string AccessToken { get; set; }
    }
}
