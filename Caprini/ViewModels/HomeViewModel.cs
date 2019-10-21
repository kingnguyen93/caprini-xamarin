using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Caprini.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = "Chào mừng";
        }

        public ICommand StartSurveyCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync("//SurveyPage").ConfigureAwait(false);
        });
    }
}