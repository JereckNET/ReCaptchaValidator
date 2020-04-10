'use strict';

/* Namespacing */
var JereckNET = JereckNET || {};
if (JereckNET.Web === undefined) {
    JereckNET.Web = {};
}

if (JereckNET.Web.Recaptcha === undefined) {
    /* Recpatcha JS */
    JereckNET.Web.Recaptcha = function (clientID, colorSchemeAware, configuration) {
        // Private variables
        var _mqLight;
        var _mqDark;
        var _widgetId;

        // Events
        var recaptchaLoaded = new Event("recaptcha:Loaded:" + clientID);

        // Private functions
        var ctor = function () {
            _mqLight = window.matchMedia('(prefers-color-scheme: light)');
            _mqDark = window.matchMedia('(prefers-color-scheme: dark)');
        };
        var colorSchemeChange = function (e) {
            //debugger;

            if (e.media === "(prefers-color-scheme: light)" && e.matches)
                configuration.theme = "light";

            if (e.media === "(prefers-color-scheme: dark)" && e.matches)
                configuration.theme = "dark";

            grecaptcha.reset(_widgetId, configuration);
        };

        // Public functions
        this.loaded = function () {
            if (colorSchemeAware) {
                _mqLight.addListener(colorSchemeChange);
                _mqDark.addListener(colorSchemeChange);

                if (_mqLight.matches) {
                    configuration.theme = "light";
                }

                if (_mqDark.matches) {
                    configuration.theme = "dark";
                }
            }

            _widgetId = grecaptcha.render('recaptcha-container-' + clientID, configuration);

            document.dispatchEvent(recaptchaLoaded);
        };

        // Call to constructor when everything is defined
        ctor();
    };
}