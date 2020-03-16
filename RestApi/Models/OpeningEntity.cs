using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class OpeningEntity:BookingRange
    {
        public Guid Roomid { get; set; }
        public int Rate { get; set; }
    }
}
