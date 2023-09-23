using GameGrabber.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace GameGrabber.Repository
{
    internal class GameRepositoryOnline : IGameRepository
    {
        private const string RequestUri = "https://www.gamerpower.com/api/giveaways?sort-by=value";
        private List<Game> _games = null;

        // Static event to signal when online repository fails to load games
        public static event EventHandler OnlineRepositoryFailed;

        private async Task LoadGames()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(RequestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _games = JsonConvert.DeserializeObject<List<Game>>(content);
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Handle HttpRequestException (e.g., no internet connection)
                    ShowErrorMessage($"Failed to load games online, switching to local repo. Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    ShowErrorMessage($"Failed to load games online, switching to local repo. Error: {ex.Message}");
                }
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

        private void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            OnlineRepositoryFailed?.Invoke(null, EventArgs.Empty); // Invoke event with error message
        }
    }
}
