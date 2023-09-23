using GameGrabber.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GameGrabber.Repository
{
    internal class GameRepositoryLocal : IGameRepository
    {
        private List<Game> _games = null;

        private async Task LoadGames()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = "GameGrabber.Resources.DataFiles.Games.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = await reader.ReadToEndAsync();
                _games = JsonConvert.DeserializeObject<List<Game>>(json);
            }
        }

        public async Task<List<Game>> GetGamesAsync()
        {
            if (_games == null)
            {
                await LoadGames();
            }

            return _games;
        }
    }
}
