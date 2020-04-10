# ![reCAPTCHA Validator Logo](images/ReCaptchaNET.png)  reCAPTCHA Validator

_reCAPTCHA Validator_ makes it easy to include _[Google's reCAPTCHA](https://www.google.com/recaptcha/) v2 (Checkbox)_  in ASP.NET pages.

# Installation

| Component                     | Package                                                                                                                                              |
|-------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------|
| `JereckNET.Web.ReCaptcha` | [![reCAPTCHA Validator NuGet Package](https://img.shields.io/nuget/v/JereckNET.Web.ReCaptcha.svg)](https://www.nuget.org/packages/JereckNET.Web.ReCaptcha) |

Install _reCAPTCHA Validator_ by searching for `reCAPTCHA` in Visual Studio's Package Manager or by using the Package Manager Console :
```
PM > Install-Package JereckNET.Web.ReCaptcha
```


# User Guide
## Register for Google reCAPTCHA
To start using reCAPTCHA, you need to [sign up for an API key pair](http://www.google.com/recaptcha/admin) for your site. The key pair consists of a site key and secret key. The site key is used to invoke reCAPTCHA service on your site. The secret key authorizes communication between your application backend and the reCAPTCHA server to [verify the user's response](https://developers.google.com/recaptcha/docs/verify). The secret key needs to be kept safe for security purposes.

This library is currently compatible only with the reCAPTCHA v2 [Checkbox](https://developers.google.com/recaptcha/docs/display) mode.

For more information, please refer to [Google's reCAPTCHA](https://developers.google.com/recaptcha/) website.

## Use in your ASP.NET project
### In  the ASPX front-end
From the Designer toolbox, drop the `ReCaptchaValidator` control on your designer surface, or add the following code to your ASPX header markup :
```html
<%@ Register Assembly="JereckNET.Web.ReCaptcha" Namespace="JereckNET.Web.UI" TagPrefix="jcw" %>
```
and this control wherever you want your reCAPTCHA to be located :
```html
<jcw:ReCaptchaValidator ID="captchaValidator" runat="server" 
    SecretKey="your_secret_key" 
    SiteKey="your_site_key" />
```
### In your code back-end
The ReCaptchaValidator control implements the`IValidator` interface.
To check the validity of the user's response, a simple call to `Page.IsValid` will trigger the check with the reCAPTCHA's API :
```csharp
protected void Button1_Click(object sender, EventArgs e) {
    if (Page.IsValid) {
        /* ... */
    }
}
```

## Public properties
The `ReCaptchaValidator` class derives from [WebControl](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.webcontrol) and has the following additional public properties :

Property                | Description
----------------------- | -----------
Callback                | The name of your client-side callback function, executed when the user submits a successful response.<br>The g-recaptcha-response token is passed to your callback.
ErrorCallback           | The name of your client-side callback function, executed when reCAPTCHA encounters an error.
Errors¹                 | The errors returned from reCAPTCHA's API as a `RecaptchaErrors` enum.
ExpiredCallback         | The name of your client-side callback function, executed when the reCAPTCHA response expires and the user needs to re-verify.
LoadedCallback          | The name of your client-side callback function, executed when the reCAPTCHA control is created.
PrefersColorSchemeAware | Instruct the control to select the theme to use based on the client's browser preference.<br>If the browser is [not compatible](https://www.caniuse.com/#search=prefers-color-scheme) with the `prefers-color-scheme`media query, or is configured with `no-preference`, the theme configured with the `Theme` property is used.<br> Defaults to **true**.
SecretKey               | The secret key as configured in [reCAPTCHA Administration Console](https://www.google.com/recaptcha/admin/).<br>Defaults to **your_secret_key** (and will not work as is).
SiteKey                 | The site key as configured in [reCAPTCHA Administration Console](https://www.google.com/recaptcha/admin/).<br>Defaults to **your_site_key** (and will not work as is).
Size                    | The size of the widget.<br>Defaults to **Normal**
TestMode²               | This enables the reCAPTCHA validation to pass without check (for test purposes only).<br>Defaults to **false**
Theme                   | The color theme of the widget.<br>Defaults to **Light**

> ¹ The enumeration returned is decorated with the `[Flags]` attribute and can contain multiple values.
>
> ² The test mode is designed to be used for automatic testing (which, by itself is contrary the the very principle of CAPTCHA validation). The verification requests will always pass.<br>This uses a special set of SiteKey/SecretKey [provided by Google](https://developers.google.com/recaptcha/docs/faq#id-like-to-run-automated-tests-with-recaptcha.-what-should-i-do), and displays a warning message to ensure it's not used for production traffic.

## `RecaptchaErrors` enum values
* **MissingInputSecret** : The secret parameter is missing.
* **InvalidInputSecret** : The secret parameter is invalid or malformed.
* **MissingInputResponse** : The response parameter is missing.
* **InvalidInputResponse** : The response parameter is invalid or malformed.
* **BadRequest** : The request is invalid or malformed.
* **TimeoutOrDuplicate** : The response is no longer valid: either is too old or has been used previously.        
* **UnknownError** : Self-explanatory


# License
_reCAPTCHA Validator_ is licensed under the MIT License - the details are at [LICENSE.txt](LICENSE.txt)