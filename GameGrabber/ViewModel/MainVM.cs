using CommunityToolkit.Mvvm.ComponentModel;
using GameGrabber.View;
using System.Windows.Controls;

namespace GameGrabber.ViewModel
{
    internal class MainVM : ObservableObject
    {
        public MainVM()
        {
            CurrentPage = OverviewPage;

            OverviewPage.CardDoubleClick += (s, e) => SwitchPage();
            DetailVM.Back += (s, e) => SwitchPage();
        }

        public Page CurrentPage { get; private set; }

        public OverviewPage OverviewPage { get; } = new OverviewPage();
        public DetailPage DetailPage { get; } = new DetailPage();

        private void SwitchPage()
        {
            if (CurrentPage is OverviewPage)
            {
                CurrentPage = DetailPage;
                (DetailPage.DataContext as DetailVM).Game = (OverviewPage.DataContext as OverviewVM).SelectedGame;
            }
            else
            {
                CurrentPage = OverviewPage;
            }

            OnPropertyChanged(nameof(CurrentPage));
        }
    }
}
