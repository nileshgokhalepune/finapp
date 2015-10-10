using System.Collections.Generic;
using Newtonsoft.Json;

namespace FinApp.Models
{
    public class IndustryModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("company")]
        public List<CompanyModel> Company { get; set; }
        
    }
}
