using Caprini.Models;
using Caprini.Services;
using Caprini.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Caprini.ViewModels
{
    public class SurveyViewModel : BaseViewModel
    {
        public IDataStore<Survey> DataStore => DependencyService.Get<IDataStore<Survey>>();

        public ObservableCollection<Survey> Surveys { get; set; } = new ObservableCollection<Survey>();

        private Survey currentSurvey;
        public Survey CurrentSurvey { get => currentSurvey; set => SetProperty(ref currentSurvey, value); }

        private int position;
        public int Position { get => position; set => SetProperty(ref position, value, onChanged: () => OnPropertyChanged(nameof(PreviousVisible), nameof(NextVisible), nameof(NextText))); }

        public bool PreviousVisible => Position > 0;

        public bool NextVisible => Position < Surveys.Count;

        public string NextText => Position == Surveys.Count - 1 ? "Kết quả" : "Tiếp theo";

        public ICommand LoadItemsCommand { get; set; }

        public SurveyViewModel()
        {
            Title = "Khảo sát";

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Unsubscribe<SurveyResultViewModel>(this, "StartOver");
            MessagingCenter.Subscribe<SurveyResultViewModel>(this, "StartOver", (s) => LoadItemsCommand.Execute(null));

            LoadItemsCommand.Execute(null);
        }

        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Surveys.Clear();
                Position = 0;

                foreach (var item in await DataStore.GetItemsAsync(true))
                {
                    Surveys.Add(item);

                    if (!item.IsMulti)
                    {
                        foreach (var q in item.Questions)
                        {
                            q.PropertyChanged -= Q_PropertyChanged;
                            q.PropertyChanged += Q_PropertyChanged;
                        }
                    }
                }

                CurrentSurvey = Surveys.FirstOrDefault();

                OnPropertyChanged(nameof(PreviousVisible), nameof(NextVisible));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Q_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                if (sender is Question question && question.Selected && Surveys.FirstOrDefault(s => s.Id == question.SurveyId) is Survey survey)
                {
                    foreach (var q in survey.Questions.FindAll(q => q.Content != question.Content))
                    {
                        q.PropertyChanged -= Q_PropertyChanged;

                        q.Selected = false;

                        q.PropertyChanged += Q_PropertyChanged;
                    }
                }
            }
        }

        public ICommand PreviousCommand => new Command(() =>
        {
            if (Position > 0)
            {
                Position--;
                CurrentSurvey = Surveys[Position];
            }
        });

        public ICommand NextCommand => new Command(() =>
        {
            if (Position < Surveys.Count - 1)
            {
                Position++;
                CurrentSurvey = Surveys[Position];
            }
            else
            {
                CalculateResult();
            }
        });

        private async void CalculateResult()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                var totalCaprini = Surveys.Sum(s => s.Questions.Where(q => q.Selected).Sum(q => q.Caprini));
                var totalImprove = Surveys.Sum(s => s.Questions.Where(q => q.Selected).Sum(q => q.Improve));
                var totalLung = Surveys.Sum(s => s.Questions.Where(q => q.Selected).Sum(q => q.Lung));

                string capriniResult;
                string capriniRate;
                string lungRate;
                string improveRate;
                string plan;

                if (totalCaprini == 0)
                {
                    capriniResult = "Rất thấp";
                    capriniRate = "< 10%";
                    plan = "Đi lại sớm trong thời gian nằm viện";
                }
                else if (totalCaprini < 2)
                {
                    capriniResult = "Thấp";
                    capriniRate = "10-20%";
                    plan = "Đi tất áp lực hoặc chống đông dự phòng trong thời gian nằm viện";
                }
                else if (totalCaprini < 4)
                {
                    capriniResult = "Trung bình";
                    capriniRate = "20-40%";
                    plan = "Đi tất áp lực và chống đông dự phòng trong thời gian nằm viện";
                }
                else if (totalCaprini < 8)
                {
                    capriniResult = "Cao";
                    capriniRate = "40-80%";
                    plan = "Đi tất áp lực và chống đông dự phòng 7-10 ngày";
                }
                else
                {
                    capriniResult = "Rất cao";
                    capriniRate = "40-80%";
                    plan = "Đi tất áp lực và chống đông dự phòng 30 ngày";
                }

                if (totalImprove < 7)
                {
                    improveRate = "Không có nguy cơ chảy máu nặng, hoặc chảy máu có ý nghĩa lâm sàng";
                }
                else
                {
                    improveRate = "Có nguy cơ chảy máu nặng, hoặc chảy máu có ý nghĩa lâm sàng";
                }

                if (totalLung < 3)
                {
                    lungRate = "7-9%";
                }
                else if (totalLung < 10)
                {
                    lungRate = "20-30%";
                }
                else
                {
                    lungRate = "> 60%";
                }

                var page = new SurveyResultPage
                {
                    BindingContext = new SurveyResultViewModel()
                    {
                        TotalCaprini = totalCaprini,
                        CapriniResult = capriniResult,
                        CapriniRate = capriniRate,
                        ImproveRate = improveRate,
                        LungeRate = lungRate,
                        Plan = plan
                    }
                };

                await Shell.Current.Navigation.PushModalAsync(new NavigationPage(page));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}