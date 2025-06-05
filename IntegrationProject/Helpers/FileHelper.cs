namespace IntegrationProject.Helpers
{
    public class FileHelper
    {
        internal async Task SaveResponseToFileAsync(HttpResponseMessage response, string filePath)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var file   = File.Create(filePath);

            await stream.CopyToAsync(file);
        }

        internal StreamReader ReadDataFromFilePath(string filePath)
        {
            return new StreamReader(filePath);
        }

        internal string GetLocalPathToSaveFile(string fileName)
        {
            return Path.Combine(Path.GetTempPath(), fileName);
        }
    }
}
