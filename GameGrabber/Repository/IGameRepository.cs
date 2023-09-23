using GameGrabber.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameGrabber.Repository
{
    internal interface IGameRepository
    {
        Task<List<Game>> GetGamesAsync();
    }
}
