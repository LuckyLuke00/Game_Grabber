using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameGrabber.Model;
using GameGrabber.Repository;
using GameGrabber.View.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GameGrabber.ViewModel
{
    internal class OverviewVM : ObservableObject
    {
        // Initial sorting order is by value descending
        private bool _isNameSortedDescending = false; // Keep track of the sorting order

        private bool _isValueSortedDescending = true; // Keep track of the sorting order

        private readonly string _sortSymbolAscending = "▲";
        private readonly string _sortSymbolDescending = "▼";

        private Game _selectedGame;
        private IGameRepository _gameRepository = null;
        private List<Game> _allGames;
        private ObservableCollection<Game> _games;

        public RelayCommand SortValueCommand { get; private set; }
        public RelayCommand SortNameCommand { get; private set; }

        private string _sortByValueText = "Value";
        private string _sortByNameText = "Name";

        public string SortByValueText
        {
            get { return _sortByValueText; }
            private set
            {
                _sortByValueText = value;
                OnPropertyChanged(nameof(SortByValueText));
            }
        }

        public string SortByNameText
        {
            get { return _sortByNameText; }
            private set
            {
                _sortByNameText = value;
                OnPropertyChanged(nameof(SortByNameText));
            }
        }

        public Game SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedGame));
            }
        }

        public bool UseLocalRepository
        {
            get { return _gameRepository is GameRepositoryLocal; }
            set
            {
                if (value)
                {
                    _gameRepository = new GameRepositoryLocal();
                }
                else
                {
                    _gameRepository = new GameRepositoryOnline();
                }
                _ = GetGamesAsync();
            }
        }

        public ObservableCollection<Game> Games
        {
            get { return _games; }
            set
            {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        public OverviewVM()
        {
            UseLocalRepository = false;

            // Subscribe to the Search event of the SearchBox UserControl
            SearchBox.Search += SearchBox_Search;

            SortValueCommand = new RelayCommand(async () => await SortGamesByValueAsync());
            SortNameCommand = new RelayCommand(async () => await SortGamesByNameAsync());

            // Replace the last character of the button content of the correct symbol
            SortByValueText += $" {(_isValueSortedDescending ? _sortSymbolDescending : _sortSymbolAscending)}";
            SortByNameText += $" {(_isNameSortedDescending ? _sortSymbolDescending : _sortSymbolAscending)}";
        }

        private async Task SortGamesByValueAsync()
        {
            // We always do the opposite, because the games are initially already sorted by value descending
            if (_isValueSortedDescending)
            {
                var sortedGames = await Task.Run(() => Games.OrderBy(game => game.PriceValue).ThenBy(game => game.Title)).ConfigureAwait(false);
                Games = await Task.Run(() => new ObservableCollection<Game>(sortedGames)).ConfigureAwait(false);
            }
            else
            {
                var sortedGames = await Task.Run(() => Games.OrderByDescending(game => game.PriceValue).ThenBy(game => game.Title)).ConfigureAwait(false);
                Games = await Task.Run(() => new ObservableCollection<Game>(sortedGames)).ConfigureAwait(false);
            }

            _isValueSortedDescending = !_isValueSortedDescending;
            SortByValueText = SortByValueText.Substring(0, SortByValueText.Length - 1) + $"{(_isValueSortedDescending ? _sortSymbolDescending : _sortSymbolAscending)}";
        }

        private async Task SortGamesByNameAsync()
        {
            if (_isNameSortedDescending)
            {
                var sortedGames = await Task.Run(() => Games.OrderBy(game => game.Title).ThenBy(game => game.PriceValue)).ConfigureAwait(false);
                Games = await Task.Run(() => new ObservableCollection<Game>(sortedGames)).ConfigureAwait(false);
            }
            else
            {
                var sortedGames = await Task.Run(() => Games.OrderByDescending(game => game.Title).ThenBy(game => game.PriceValue)).ConfigureAwait(false);
                Games = await Task.Run(() => new ObservableCollection<Game>(sortedGames)).ConfigureAwait(false);
            }

            _isNameSortedDescending = !_isNameSortedDescending;
            SortByNameText = SortByNameText.Substring(0, SortByNameText.Length - 1) + $"{(_isNameSortedDescending ? _sortSymbolDescending : _sortSymbolAscending)}";
        }

        public async Task GetGamesAsync()
        {
            _allGames = await _gameRepository.GetGamesAsync();
            UpdateGamesCollection();
        }

        private void UpdateGamesCollection()
        {
            Games = new ObservableCollection<Game>(_allGames);
        }

        private void SearchBox_Search(object sender, string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                ShowAllGames();
                return;
            }
            _ = SearchGames(searchQuery.ToString());
        }

        private async Task SearchGames(string searchQuery)
        {
            List<Game> filteredGames = await Task.Run(() =>
            {
                return _allGames.Where(game => game.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }).ConfigureAwait(false);

            Games = await Task.Run(() => new ObservableCollection<Game>(filteredGames)).ConfigureAwait(false); ;
        }

        public void ShowAllGames()
        {
            Games = new ObservableCollection<Game>(_allGames);
        }
    }
}
