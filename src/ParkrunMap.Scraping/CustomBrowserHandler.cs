using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping
{
    public class CustomBrowserHandler : DelegatingHandler
    {
        public CustomBrowserHandler(HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.Clear();
            request.Headers.Accept.Clear();
            request.Headers.AcceptLanguage.Clear();

            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)");
            request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
            request.Headers.AcceptLanguage.ParseAdd("en-GB,en-US;q=0.9,en;q=0.8");

            return await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
