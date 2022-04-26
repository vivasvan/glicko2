using Newtonsoft.Json;

namespace Surabhi.Models
{
    public partial class ClientSentimentsResponse
    {
        [JsonProperty("clientSentiments")]
        public ClientSentiment[] ClientSentiments { get; set; }
    }

    public partial class ClientSentiment
    {
        [JsonProperty("marketId")]
        public string MarketId { get; set; }

        [JsonProperty("longPositionPercentage")]
        public long LongPositionPercentage { get; set; }

        [JsonProperty("shortPositionPercentage")]
        public long ShortPositionPercentage { get; set; }
    }
}