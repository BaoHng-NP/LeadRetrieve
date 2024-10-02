using Newtonsoft.Json;
namespace LeadRetrieve.Models
{
    public class LeadResponse
    {
        [JsonProperty("data")]
        public List<LeadData> Data { get; set; }

        [JsonProperty("paging")]
        public PagingInfo Paging { get; set; }
    }

    public class LeadData
    {
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("field_data")]
        public List<FieldData> FieldData { get; set; }
    }

    public class FieldData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }
    }

    public class PagingInfo
    {
        [JsonProperty("cursors")]
        public Cursors Cursors { get; set; }
    }

    public class Cursors
    {
        [JsonProperty("before")]
        public string Before { get; set; }

        [JsonProperty("after")]
        public string After { get; set; }
    }
}
