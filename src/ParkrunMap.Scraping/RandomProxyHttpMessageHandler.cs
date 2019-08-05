using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping
{
    public class RandomProxyHttpMessageHandler : HttpClientHandler
    {
        private readonly ProxyStore _proxyStore;

        public RandomProxyHttpMessageHandler(ProxyStore proxyStore)
        {
            _proxyStore = proxyStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var address = await _proxyStore.GetRandomProxyAddress().ConfigureAwait(false);

            Proxy = new WebProxy(address);
            UseProxy = true;

            try
            {
                return await base.SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch
            {
                _proxyStore.RemoveAddress(address);
                throw;
            }
        }
    }
}