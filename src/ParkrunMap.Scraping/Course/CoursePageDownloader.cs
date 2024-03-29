﻿using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping.Course
{
    public class CoursePageDownloader
    {
        private readonly HttpClient _httpClient;

        public CoursePageDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> DownloadAsync(string domain, string path, CancellationToken cancellationToken)
        {
            using var response = await _httpClient
                .GetAsync($"https://{domain}{path}/course/", cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);
        }
    }
}
