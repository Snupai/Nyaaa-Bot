using Newtonsoft.Json.Linq;
using System.Net;

namespace DC_BOT.Commands.Neko
{
    class NekoService : INekoService
    {
        string apiKey = Environment.GetEnvironmentVariable("apiKey");

        public string GetNeko(NekoKind kind)
        {
            var url = this.GetUrl(kind);

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Headers["Authorization"] = apiKey;


            string result;
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            dynamic jsonObj = JObject.Parse(result);

            return jsonObj.file;
        }

        private string GetUrl(NekoKind kind) {
            switch (kind)
            {
                case NekoKind.NekoBoy:
                    return "https://gallery.fluxpoint.dev/api/sfw/img/nekoboy";
                case NekoKind.Neko:
                    return "https://gallery.fluxpoint.dev/api/sfw/img/neko";
                case NekoKind.NekoGif:
                    return "https://gallery.fluxpoint.dev/api/sfw/gif/neko";
                case NekoKind.NekoPara:
                    return "https://gallery.fluxpoint.dev/api/sfw/img/nekopara";
            }

            throw new Exception();
        }
    }
}
