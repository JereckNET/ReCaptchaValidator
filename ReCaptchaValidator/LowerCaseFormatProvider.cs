using System;

namespace JereckNET.Web.UI {
    class LowerCaseFormatProvider : IFormatProvider, ICustomFormatter {
        public object GetFormat(Type formatType) {
            if (formatType == typeof(ICustomFormatter))
                return this;
            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider) {
            if (arg == null) return string.Empty;
            else if (format == "L") return arg.ToString().ToLower();
            else return arg.ToString();
        }
    }
}