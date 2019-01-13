using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping
{
    public class RedirectHandler : DelegatingHandler
    {
        private static readonly HttpStatusCode[] RedirectOn = {
            HttpStatusCode.MovedPermanently,
            HttpStatusCode.Found
        };

        public RedirectHandler(HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (request.Method == HttpMethod.Get && RedirectOn.Contains(response.StatusCode))
            {
                request.RequestUri = response.Headers.Location;

                return await base.SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);
            }

            return response;
        }
    }
}
