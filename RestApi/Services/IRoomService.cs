using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Services
{
    public interface IRoomService
    {
        Task<Room> GetRoomAsync(Guid id);
        Task<PagedResults<Room>> GetRoomsAsync(PagingOptions pagingOptions, SortOptions<Room, RoomEntity> sortOptions);
    }
}
