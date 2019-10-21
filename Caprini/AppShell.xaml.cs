using Caprini.Views;
using Xamarin.Forms;

namespace Caprini
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("SurveyResultPage", typeof(SurveyResultPage));
        }
    }
}