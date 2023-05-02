using System.Xml.Linq;

namespace FecebookAPI.ExceptionHandler
{
    public class CustomAuthorizationException : Exception
    {
        public CustomAuthorizationException(Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString localizedHtmlString)
        : base(String.Format("AuthorizationException: {0}", localizedHtmlString.Value)) 
        {
        }

        public CustomAuthorizationException(string name)
            : base(String.Format("AuthorizationException: {0}", name))
        {

        }
    }
}
