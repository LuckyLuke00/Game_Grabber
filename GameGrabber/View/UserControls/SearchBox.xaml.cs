using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameGrabber.View.UserControls
{
    public partial class SearchBox : UserControl
    {
        // Define the search event
        public static event EventHandler<string> Search;

        private bool _disableInstantSearch = false;
        private double _searchDelayMs = 150;
        private System.Timers.Timer _searchDelayTimer;

        public SearchBox()
        {
            InitializeComponent();

            txtInput.KeyUp += TxtInput_KeyUp;
        }

        private void txtInput_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (txtInput.IsKeyboardFocused)
            {
                tbPlaceholder.Visibility = Visibility.Hidden;
                txtInput.SelectAll();
            }
            else
            {
                if (!string.IsNullOrEmpty(txtInput.Text)) return;

                tbPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void txtInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                tbPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void TxtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtInput.IsKeyboardFocused)
            {
                // Raise the search event with the search query as an argument
                OnSearch(txtInput.Text);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Raise the search event with the search query as an argument
            OnSearch(txtInput.Text);
        }

        private void OnSearch(string searchQuery)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                tbPlaceholder.Visibility = Visibility.Visible;
                return;
            }

            Search?.Invoke(null, searchQuery);
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_disableInstantSearch)
            {
                if (string.IsNullOrEmpty(txtInput.Text))
                {
                    Search?.Invoke(null, "");
                }

                return;
            }

            if (_searchDelayTimer != null)
            {
                // If there's an existing timer, stop it
                _searchDelayTimer.Stop();
            }
            else
            {
                // Otherwise, create a new timer
                _searchDelayTimer = new System.Timers.Timer(_searchDelayMs);
                _searchDelayTimer.Elapsed += OnSearchDelayTimerElapsed;
                _searchDelayTimer.AutoReset = false;
            }

            // Start the timer to wait for the specified delay
            _searchDelayTimer.Start();
        }

        private void OnSearchDelayTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Invoke the search with the current text after the delay
            Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrEmpty(txtInput.Text))
                {
                    Search?.Invoke(null, "");
                }
                else
                {
                    Search?.Invoke(null, txtInput.Text);
                }
            });

            // Stop and dispose the timer
            _searchDelayTimer.Stop();
            _searchDelayTimer.Dispose();
            _searchDelayTimer = null;
        }
    }
}
