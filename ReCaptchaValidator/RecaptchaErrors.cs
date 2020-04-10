using System;
using Newtonsoft.Json;

namespace JereckNET.Web.UI {
    [Flags]
    [JsonConverter(typeof(RecaptchaErrorCodeConverter))]
    public enum RecaptchaErrors {
        None                    = 0b0000_0000,
        MissingInputSecret      = 0b0000_0001,  //  missing-input-secret	The secret parameter is missing.
        InvalidInputSecret      = 0b0000_0100,  //  invalid-input-secret	The secret parameter is invalid or malformed.
        MissingInputResponse    = 0b0000_1000,  //  missing-input-response	The response parameter is missing.
        InvalidInputResponse    = 0b0001_0000,  //  invalid-input-response	The response parameter is invalid or malformed.
        BadRequest              = 0b0010_0000,  //  bad-request	            The request is invalid or malformed.
        TimeoutOrDuplicate      = 0b0100_0000,  //  timeout-or-duplicate	The response is no longer valid: either is too old or has been used previously.        
        UnknownError            = 0b1000_0000
    }
}

