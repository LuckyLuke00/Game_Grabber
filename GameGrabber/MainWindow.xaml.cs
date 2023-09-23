using System.Windows;
using System.Windows.Input;

namespace GameGrabber
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Add event handler for Window StateChanged event
            StateChanged += MainWindow_StateChanged;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Set the mouse to a hand when the user clicks on the border
                DragMove();
            }
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Normal)
            {
                MaximizeWindow();
            }
            else
            {
                ResetWindow();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainWindow_StateChanged(object sender, System.EventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                MaximizeWindow();
            }
            else if (Application.Current.MainWindow.WindowState != WindowState.Minimized)
            {
                ResetWindow();
            }
        }

        private void MaximizeWindow()
        {
            // We need to do the window does not maximize correctly, when the style is set to None
            MaxHeight = SystemParameters.WorkArea.Height + 14;
            BorderThickness = new Thickness(6);

            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            WindowStateButton.Content = "❐";
        }

        private void ResetWindow()
        {
            // Reset the workaround for the maximized window
            BorderThickness = new Thickness(0);

            Application.Current.MainWindow.WindowState = WindowState.Normal;

            WindowStateButton.Content = "⬜";
        }
    }
}
