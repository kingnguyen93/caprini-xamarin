using Newtonsoft.Json;
using System.Collections.Generic;

namespace Caprini.Models
{
    public class Survey : BaseModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("is_multi")]
        public bool IsMulti { get; set; }

        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }

    public class Question : BaseModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("survey_id")]
        public long SurveyId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("caprini")]
        public float Caprini { get; set; }

        [JsonProperty("improve")]
        public float Improve { get; set; }

        [JsonProperty("lung")]
        public float Lung { get; set; }

        private bool seleced;
        [JsonIgnore]
        public bool Selected { get => seleced; set => SetProperty(ref seleced, value); }
    }
}