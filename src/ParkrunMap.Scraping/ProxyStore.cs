﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping
{
    public class ProxyStore
    {
        private static readonly Random Random = new Random();
        private IReadOnlyCollection<Uri> _addresses = new List<Uri>();
        private DateTime _nextUpdate = DateTime.MinValue;

        public async Task<Uri> GetRandomProxyAddress()
        {
            await TryUpdateAddresses().ConfigureAwait(false);

            var index = Random.Next(_addresses.Count);

            return _addresses.ElementAt(index);
        }

        private async Task TryUpdateAddresses()
        {
            var scheme = "http";
            if (DateTime.UtcNow > _nextUpdate || _addresses.Count == 0)
            {
                _nextUpdate = DateTime.UtcNow.AddHours(8);

                using (var httpClient = new HttpClient())
                {
                    var url = $"https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list.txt";
                    var response = await httpClient
                        .GetAsync(url)
                        .ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);

                    var addresses = result
                        .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                        .Skip(6)
                        .Where(x => !x.Contains("S") /* Not HTTPS */)
                        .Select(x =>
                        {
                            var hostPort = x.Split(' ')[0].Split(':');
                            return new UriBuilder(scheme, hostPort[0].Trim(), int.Parse(hostPort[1].Trim())).Uri;
                        }).ToArray();

                    _addresses = addresses;
                }
            }
        }

        public void RemoveAddress(Uri uri)
        {
            var uris = new List<Uri>(_addresses);
            uris.Remove(uri);

            _addresses = uris;
        }

    }
}