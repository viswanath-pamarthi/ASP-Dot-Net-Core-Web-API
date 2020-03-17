using RestApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class Opening
    {
        [Sortable(EntityProperty=nameof(OpeningEntity.Roomid))]
        public Link Room { get; set; }
        [Sortable]
        public DateTimeOffset StartAt { get; set; }
        [Sortable]
        public DateTimeOffset EndAt { get; set; }
        
        [Sortable]
        public decimal Rate { get; set; }
    }
}
