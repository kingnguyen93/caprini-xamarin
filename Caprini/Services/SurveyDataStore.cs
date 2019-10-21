using Caprini.Extensions;
using Caprini.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Caprini.Services
{
    public class SurveyDataStore : IDataStore<Survey>
    {
        private List<Survey> items;

        private const string LocalSurveys = "surveys.json";

        public SurveyDataStore()
        {
            InitSurveys();
        }

        private void InitSurveys()
        {
            // Read Settings
            var file = AssetsExtension.GetFile(LocalSurveys);

            if (file == null)
                return;

            try
            {
                using var reader = new StreamReader(file);

                items = JsonConvert.DeserializeObject<List<Survey>>(reader.ReadToEnd());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("INIT_SURVEYS_ERROR:" + ex.GetBaseException().Message);
            }
        }

        public async Task<bool> AddItemAsync(Survey item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Survey item)
        {
            var oldItem = items.Where((Survey arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(long id)
        {
            var oldItem = items.Where((Survey arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Survey> GetItemAsync(long id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Survey>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
                return await Task.FromResult(items.DeepCopy());
            else
                return await Task.FromResult(items);
        }
    }
}