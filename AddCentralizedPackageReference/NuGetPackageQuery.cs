using System.Text.Json;

namespace AddCentralizedPackageReference
{
    record LatestPackageVersion(string PackageName, string Version);

    internal static class NuGetPackageQuery
    {
        static readonly HttpClient s_client = new();

        public static async Task<LatestPackageVersion?> GetLatestVersion(string packageName)
        {
            var indexJson = await s_client.GetStringAsync("https://api.nuget.org/v3/index.json");
            var doc = JsonDocument.Parse(indexJson);

            var target = doc.RootElement.GetProperty("resources")
                .EnumerateArray()
                .First(x => x.GetProperty("@type").GetString() == "SearchQueryService")
                .GetProperty("@id").GetString();

            var queryJson = await s_client.GetStringAsync($"{target}?q={packageName}");
            doc = JsonDocument.Parse(queryJson);

            var latestVersion = doc.RootElement.GetProperty("data")
                .EnumerateArray()
                .First(x => x.GetProperty("id").GetString() == packageName)
                .GetProperty("versions").EnumerateArray().Last().GetProperty("version").GetString();

            return latestVersion is null ? null : new LatestPackageVersion(packageName, latestVersion);
        }
    }
}