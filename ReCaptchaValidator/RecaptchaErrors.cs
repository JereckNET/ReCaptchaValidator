using System;
using Newtonsoft.Json;

namespace JereckNET.Web.UI {
    /// <summary>
    /// Specifies the <see cref="ReCaptchaValidator.Errors"/> returned by Google's reCAPTCHA API.
    /// </summary>
    [Flags]
    [JsonConverter(typeof(RecaptchaErrorCodeConverter))]
    public enum RecaptchaErrors {
        /// <summary>
        /// No errors
        /// </summary>
        None                    = 0b0000_0000,
        /// <summary>
        /// The secret parameter is missing.
        /// </summary>
        MissingInputSecret      = 0b0000_0001,
        /// <summary>
        /// The secret parameter is invalid or malformed.
        /// </summary>
        InvalidInputSecret      = 0b0000_0100,
        /// <summary>
        /// The response parameter is missing.
        /// </summary>
        MissingInputResponse    = 0b0000_1000,
        /// <summary>
        /// The response parameter is invalid or malformed.
        /// </summary>
        InvalidInputResponse    = 0b0001_0000,
        /// <summary>
        /// The request is invalid or malformed.
        /// </summary>
        BadRequest              = 0b0010_0000,
        /// <summary>
        /// The response is no longer valid: either is too old or has been used previously.        
        /// </summary>
        TimeoutOrDuplicate      = 0b0100_0000,
        /// <summary>
        /// Self-explanatory
        /// </summary>
        UnknownError            = 0b1000_0000
    }
}

