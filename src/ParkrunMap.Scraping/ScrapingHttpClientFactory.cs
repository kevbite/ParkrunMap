using System.Net;
using System.Net.Http;

namespace ParkrunMap.Scraping
{
    public class ScrapingHttpClientFactory
    {
        private readonly ProxyStore _proxyStore;

        public ScrapingHttpClientFactory(ProxyStore proxyStore)
        {
            _proxyStore = proxyStore;
        }

        public HttpClient Create()
        {
            var httpClientHandler = new RandomProxyHttpMessageHandler(_proxyStore)
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            var httpMessageHandler = new RedirectHandler(httpClientHandler);
            var customBrowserHandler = new CustomBrowserHandler(httpMessageHandler);

            return new HttpClient(customBrowserHandler);
        }
    }
}