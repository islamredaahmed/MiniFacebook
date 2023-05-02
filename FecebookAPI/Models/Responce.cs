using System.Net;

namespace FecebookAPI.Models
{
    public class Responce<T>
    {

        public Responce(HttpStatusCode status, string? message, List<T>? data)
        {
            this.status = status;
            this.message = message;
            this.data = data;
        }
        public Responce(HttpStatusCode status, string? message)
        {
            this.status = status;
            this.message = message;
        }

        public Responce(HttpStatusCode status, List<T>? data)
        {
            this.status = status;
            this.data = data;
        }

        public Responce()
        {
        }

        public HttpStatusCode status { get; set; }
        public string? message { get; set; }
        public List<T>? data { get; set; }
    }
}
