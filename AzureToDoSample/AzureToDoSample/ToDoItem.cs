using Newtonsoft.Json;

namespace AzureToDoSample{
    class ToDoItem{
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }

        [JsonProperty(PropertyName = "dmy")]
        public bool Dmy { get; set; }
    }
}
