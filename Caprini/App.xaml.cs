using Caprini.Extensions;
using Caprini.Services;
using System.Reflection;
using Xamarin.Forms;

namespace Caprini
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            AssetsExtension.InitAssetsExtension("AppResources.Assets", typeof(App).GetTypeInfo().Assembly);

            DependencyService.Register<SurveyDataStore>();
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            MainPage = new AppShell();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}