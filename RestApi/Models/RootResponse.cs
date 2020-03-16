using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class RootResponse:Resource
    {
        //public Link href { get; set; }
        public Link rooms { get; set; }
        public Link info { get; set; }

    }
}
