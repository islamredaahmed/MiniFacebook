using System.Xml.Linq;

namespace FecebookAPI.ExceptionHandler
{
    public class CustomValidationException : Exception
    {
        public CustomValidationException(Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString localizedHtmlString)
        : base(String.Format("ValidationException: {0}", localizedHtmlString.Value)) 
        {
        }

        public CustomValidationException(string name)
            : base(String.Format("ValidationException: {0}", name))
        {

        }
    }
}
