using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class Room:Resource
    {
        public string Name { get; set; }
        public Decimal Rate { get; set; }

    }
}
