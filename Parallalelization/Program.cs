using Parallalelization;
using System.Diagnostics;

using var httpClient = new HttpClient();

var sw = Stopwatch.StartNew();

int parallelSize = 1;

var collection = Enumerable.Range(0, 100).Select(i => new string("https://www.google.com"));

await collection.ParallelizeAsync(parallelSize, async (string url) => await GetAsync(url), true);

Console.WriteLine($"Operation finished in {sw.ElapsedMilliseconds}ms with parallelization of size {parallelSize}");

async Task GetAsync(string url)
{
    await httpClient.GetStringAsync(url);

    await Task.Delay(1000);
}