using Newtonsoft.Json;
using System;

namespace JereckNET.Web.UI {
    class RecaptchaResponse {
        [JsonProperty("success")]
        public bool Success {
            get;
            private set;
        }

        [JsonProperty("challenge_ts")]
        public DateTime Timestamp {
            get;
            private set;
        }

        [JsonProperty("hostname")]
        public string Hostname {
            get;
            private set;
        }

        [JsonProperty("error-codes")]
        [JsonConverter(typeof(RecaptchaErrorCodeConverter))]
        public RecaptchaErrors Errors {
            get;
            private set;
        }

        private RecaptchaResponse() {

        }

        public override string ToString() {
            return Success.ToString();
        }
    }
}

