﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JereckNET.Web.UI {
    [DefaultProperty("ErrorMessage")]
    [ToolboxData("<{0}:ReCaptchaValidator runat=\"server\" SiteKey=\"your_site_key\" SecretKey=\"your_secret_key\" ErrorMessage=\"ReCaptchaValidator\"></{0}:ReCaptchaValidator>")]
    public class ReCaptchaValidator : WebControl, IValidator {
        #region Constants
        private const string RECAPTCHA_SCRIPT_SOURCE = "https://www.recaptcha.net/recaptcha/api.js?onload=recaptcha_loaded_{0}&render=explicit";
        private const string RECAPTCHA_SITE_VERIFY = "https://www.recaptcha.net/recaptcha/api/siteverify?secret={0}&response={1}";
        private const string TEST_MODE_SITE_KEY = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI";
        private const string TEST_MODE_SECRET_KEY = "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe";
        private const string SCRIPT_RESOURCE_NAME = "JereckNET.Web.ReCaptcha.ReCaptcha.js";
        #endregion

        #region Designer Properties
        #region reCAPTCHA Configuration
        [Category("reCAPTCHA Configuration")]
        [Description("This is your site public key.")]
        public string SiteKey { get; set; }
        [Category("reCAPTCHA Configuration")]
        [Description("This is your site private key.")]
        public string SecretKey { get; set; }
        [Category("reCAPTCHA Configuration")]
        [DefaultValue(false)]
        [Description("This enables the reCAPTCHA validation to pass without check (for test purposes only).")]
        public bool TestMode { get; set; }
        #endregion
        #region Appearance
        [Category("Appearance")]
        [DefaultValue("Normal")]
        [Description("The size of the widget.")]
        public ReCaptchaSize Size { get; set; }
        [Category("Appearance")]
        [DefaultValue("Light")]
        [Description("The color theme of the widget.")]
        public ReCaptchaTheme Theme { get; set; }
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Is the widget compatible with browser's prefers-color-scheme media query.")]
        public bool PrefersColorSchemeAware { get; set; }

        [DefaultValue("reCAPTCHA Vaidation failed")]
        [Category("Appearance")]
        [Description("Message to display in a ValidationSummary when the captcha validation fails.")]
        public string ErrorMessage {
            get {
                object o = ViewState["ErrorMessage"];
                return ((o == null) ? String.Empty : (string)o);
            }
            set {
                ViewState["ErrorMessage"] = value;
            }
        }
        #endregion
        #region Client-side callbacks
        [Category("Client-side callbacks")]
        [Description("The name of your callback function, executed when the user submits a successful response.\nThe g-recaptcha-response token is passed to your callback.")]
        public string Callback { get; set; }
        [Category("Client-side callbacks")]
        [Description("The name of your callback function, executed when the reCAPTCHA control is created.")]
        public string LoadedCallback { get; set; }
        [Category("Client-side callbacks")]
        [Description("The name of your callback function, executed when the reCAPTCHA response expires and the user needs to re-verify.")]
        public string ExpiredCallback { get; set; }
        [Category("Client-side callbacks")]
        [Description("The name of your callback function, executed when reCAPTCHA encounters an error.")]
        public string ErrorCallback { get; set; }
        #endregion
        #endregion

        #region Non-Designer Properties
        [Browsable(false)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsValid {
            get;
            set;
        }

        [Browsable(false)]
        [Category("Error handling")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RecaptchaErrors Errors {
            get;
            private set;
        }
        #endregion

        #region Overrides
        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;
        public override bool Enabled {
            get {
                return base.Enabled;
            }
            set {
                base.Enabled = value;
                // When disabling a validator, it would almost always be intended for that validator
                // to not make the page invalid for that round-trip.
                if (!value) {
                    IsValid = true;
                }
            }
        }
        #endregion

        #region Control Life-cycle Event handlers
        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);

            if (!PropertiesValid) {
                // In practice the call to the property PropertiesValid would throw if bad things happen.
                Debug.Assert(false, "Exception should have been thrown if properties are invalid");
            }

            Page.ClientScript.RegisterClientScriptResource(typeof(ReCaptchaValidator), SCRIPT_RESOURCE_NAME);
        }
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);

            Page.Validators.Add(this);
        }
        protected override void OnUnload(EventArgs e) {
            if (Page != null) {
                Page.Validators.Remove(this);
            }
            base.OnUnload(e);
        }
        #endregion

        #region Control rendering
        protected bool PropertiesValid {
            get {
                if (string.IsNullOrEmpty(SiteKey) || SiteKey.Equals("your_site_key"))
                    throw new HttpException("Public site key not provided");

                if (string.IsNullOrEmpty(SecretKey) || SecretKey.Equals("your_secret_key"))
                    throw new HttpException("Secret site key not provided");

                return true;
            }
        }

        public override void RenderControl(HtmlTextWriter writer) {
            bool shouldBeVisible = Enabled && !IsValid;

            if (!PropertiesValid) {
                return;
            }

            if (Page != null) {
                Page.VerifyRenderingInServerForm(this);
            }

            if (Site != null && Site.DesignMode) {
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write("<b>[Google reCAPTCHA]</b>");
                writer.WriteBreak();
                writer.Write($"Site key : <em>{SiteKey}</em>");
                writer.WriteBreak();
                writer.Write($"Secret key : <em>{SecretKey}</em>");
                writer.RenderEndTag();

            } else {
                render(writer);
            }
        }
        private void render(HtmlTextWriter writer) {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "recaptcha-container-" + ClientID);

            RenderBeginTag(writer);
            RenderEndTag(writer);

            // Control-specific script
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.Write(Script($@"
                var recaptcha_{ClientID} = new JereckNET.Web.Recaptcha('{ClientID}', {PrefersColorSchemeAware:L}, {{
                    'sitekey':'{(TestMode ? TEST_MODE_SITE_KEY : SiteKey)}',
                    'theme':'{Theme:L}',
                    'size':'{Size:L}',
                    'tabindex':'{TabIndex}',
                    'callback':'{Callback}',
                    'expired-callback':'{ExpiredCallback}',
                    'error-callback':'{ErrorCallback}'
                }});
                function recaptcha_loaded_{ClientID}(){{
                    recaptcha_{ClientID}.loaded();
                }}
                "));

            if (!string.IsNullOrEmpty(LoadedCallback)) {
                writer.Write($"document.addEventListener('recaptcha:Loaded:{ClientID}', function (e) {{ {LoadedCallback}(); }}, false);");
            }

            writer.RenderEndTag();

            // Googe reCAPTCHA API
            writer.AddAttribute(HtmlTextWriterAttribute.Src, string.Format(RECAPTCHA_SCRIPT_SOURCE, ClientID));
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.AddAttribute("async", null);
            writer.AddAttribute("defer", null);
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();

        }
        #endregion

        #region IValidator 
        private async Task<string> getAPIResponse(string clientResponse) {
            HttpClient verifyRequestClient = new HttpClient();

            HttpResponseMessage verifyResponse = await verifyRequestClient.GetAsync(
                string.Format(RECAPTCHA_SITE_VERIFY, TestMode ? TEST_MODE_SECRET_KEY : SecretKey, clientResponse));

            string responseString = await verifyResponse.Content.ReadAsStringAsync();

            return responseString;
        }
        public void Validate() {
            try {
                string clientResponse = HttpContext.Current.Request["g-recaptcha-response"];

                string jsonResponse = Task.Run(() => getAPIResponse(clientResponse)).Result;
                RecaptchaResponse response = JsonConvert.DeserializeObject<RecaptchaResponse>(jsonResponse);

                if (response.Success) {
                    IsValid = true;
                } else {
                    IsValid = false;
                    Errors = response.Errors;
                }
            } catch (WebException) {
                IsValid = false;
                Errors = RecaptchaErrors.UnknownError;
            }
        }
        #endregion  

        public ReCaptchaValidator() {
            IsValid = true;
            PrefersColorSchemeAware = true;
            TestMode = false;
            Theme = ReCaptchaTheme.Light;
            Size = ReCaptchaSize.Normal;
        }

        #region Static methods
        private static string getResourceScript() {
            string scriptContent = string.Empty;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(SCRIPT_RESOURCE_NAME))
            using (StreamReader reader = new StreamReader(stream)) {
                scriptContent = reader.ReadToEnd();
            }

            return scriptContent;
        }
        private static string Script(FormattableString formattable) {
            return formattable.ToString(new LowerCaseFormatProvider());
        }
        #endregion
    }
}