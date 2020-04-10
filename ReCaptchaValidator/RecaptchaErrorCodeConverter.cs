using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace JereckNET.Web.UI {
    class RecaptchaErrorCodeConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(string[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            try {
                if (reader.TokenType != JsonToken.StartArray) return RecaptchaErrors.UnknownError;

                RecaptchaErrors errorReceived = RecaptchaErrors.None;

                while (reader.Read()) {
                    if (reader.TokenType == JsonToken.EndArray) break;
                    if (reader.TokenType == JsonToken.Comment) continue;

                    string enumString = (string)reader.Value;

                    switch (enumString) {
                        case "missing-input-secret":
                            errorReceived |= RecaptchaErrors.MissingInputSecret;
                            break;
                        case "invalid-input-secret":
                            errorReceived |= RecaptchaErrors.InvalidInputSecret;
                            break;
                        case "missing-input-response":
                            errorReceived |= RecaptchaErrors.MissingInputResponse;
                            break;
                        case "invalid-input-response":
                            errorReceived |= RecaptchaErrors.InvalidInputResponse;
                            break;
                        case "bad-request":
                            errorReceived |= RecaptchaErrors.BadRequest;
                            break;
                        case "timeout-or-duplicate":
                            errorReceived |= RecaptchaErrors.TimeoutOrDuplicate;
                            break;
                        default:
                            Debug.Assert(false, "Should not have been returned from reCAPTCHA API");
                            errorReceived |= RecaptchaErrors.UnknownError;
                            break;
                    }
                }

                return errorReceived;
            } catch (Exception) {
                return RecaptchaErrors.UnknownError;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            RecaptchaErrors errorToSend = (RecaptchaErrors)value;

            switch (errorToSend) {
                case RecaptchaErrors.MissingInputSecret:
                    writer.WriteValue("missing-input-secret");
                    break;
                case RecaptchaErrors.InvalidInputSecret:
                    writer.WriteValue("invalid-input-secret");
                    break;
                case RecaptchaErrors.MissingInputResponse:
                    writer.WriteValue("missing-input-response");
                    break;
                case RecaptchaErrors.InvalidInputResponse:
                    writer.WriteValue("invalid-input-response");
                    break;
                case RecaptchaErrors.BadRequest:
                    writer.WriteValue("bad-request");
                    break;
                case RecaptchaErrors.TimeoutOrDuplicate:
                    writer.WriteValue("timeout-or-duplicate");
                    break;
                case RecaptchaErrors.UnknownError:
                default:
                    Debug.Assert(false, "This type should not be sent");
                    break;
            }
        }
    }
}