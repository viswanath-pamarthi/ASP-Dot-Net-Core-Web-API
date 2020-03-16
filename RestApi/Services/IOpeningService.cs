using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Services
{
    public interface IOpeningService
    {
        Task<PagedResults<Opening>> GetOpeningsAsync(PagingOptions pagingOptions);

        Task<IEnumerable<BookingRange>> GetConflictingSlots(
            Guid roomid,
            DateTimeOffset start,
            DateTimeOffset end
            );

    }
}
