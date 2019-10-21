using System.Windows.Input;
using Xamarin.Forms;

namespace Caprini.ViewModels
{
    public class SurveyResultViewModel : BaseViewModel
    {
        public float TotalCaprini { get; set; }

        public string CapriniResult { get; set; }

        public string CapriniRate { get; set; }

        public string LungeRate { get; set; }

        public string ImproveRate { get; set; }

        public string Plan { get; set; }

        public SurveyResultViewModel()
        {
            Title = "Kết quả khảo sát";
        }

        public ICommand PreviousCommand => new Command(async () =>
        {
            await Shell.Current.Navigation.PopModalAsync();
        });

        public ICommand NewSurveyCommand => new Command(async () =>
        {
            MessagingCenter.Send(this, "StartOver");
            await Shell.Current.Navigation.PopModalAsync();
        });
    }
}