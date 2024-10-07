using Newtonsoft.Json;

namespace LeadRetrieve.Models
{
    public class JsonDataModel
    {
        [JsonProperty("entry")]
        public List<Entry> Entry { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }
    }

    public class Entry
    {
        [JsonProperty("changes")]
        public List<Change> Changes { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }
    }

    public class Change
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("value")]
        public Value Value { get; set; }
    }

    public class Value
    {
        [JsonProperty("ad_id")]
        public string ad_id { get; set; }

        [JsonProperty("form_id")]
        public string form_id { get; set; }

        [JsonProperty("leadgen_id")]
        public string leadgen_id { get; set; }

        [JsonProperty("created_time")]
        public int created_time { get; set; }

        [JsonProperty("page_id")]
        public string page_id { get; set; }

        [JsonProperty("adgroup_id")]
        public string adgroup_id { get; set; }
    }


}
