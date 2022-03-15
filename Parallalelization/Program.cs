using Parallalelization;
using System.Diagnostics;

using var httpClient = new HttpClient();

int parallelSize = 5;

var collection = Enumerable.Range(0, 100).Select(i => new string("https://www.google.com"));

var sw = Stopwatch.StartNew();

await collection.ParallelizeAsync(parallelSize, async (string url) => await GetAsync(url), true);

Console.WriteLine($"\nOperation finished in {sw.ElapsedMilliseconds}ms with parallelization of size {parallelSize}");

async Task GetAsync(string url)
{
    await httpClient.GetStringAsync(url);

    await Task.Delay(1000);
}