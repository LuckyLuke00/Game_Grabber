using System;
using System.Windows.Controls;

namespace GameGrabber.View
{
    public partial class OverviewPage : Page
    {
        public static event EventHandler CardDoubleClick;

        public OverviewPage()
        {
            InitializeComponent();
        }

        private void GameCard_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CardDoubleClick?.Invoke(null, EventArgs.Empty);
        }
    }
}
