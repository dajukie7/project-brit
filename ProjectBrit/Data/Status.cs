using System;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ProjectBrit.Data
{
    public class Status
    {
        public Status()
        {
        }

        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
    }    
}