using System.Text.RegularExpressions;

namespace YoutubeChanelMetadataAsFolders.App;

public static class HelperFunctions
{
    public static string ToSafeFolderName(string name)
    {
        var safeName = Regex.Replace(name, "[^0-9a-zA-Z _]+", "");
        safeName = safeName.Replace(' ', '_');
        return safeName;
    }

    public static async Task<string> CreateReadmeContent(
        Google.Apis.YouTube.v3.Data.PlaylistItem playlistItem,
        HttpClient httpClient)
    {
        var thumbnailUrl = playlistItem.Snippet.Thumbnails.Standard.Url;

        var httpResponse = await httpClient.GetAsync(thumbnailUrl);
        var imageBytes = await httpResponse.Content.ReadAsByteArrayAsync();
        var base64Image = Convert.ToBase64String(imageBytes);

        return
            $"# {playlistItem.Snippet.Title}\n\nPublished on: {playlistItem.Snippet.PublishedAt}\n\n![Thumbnail](data:image/png;base64,{base64Image})\n\n[Link to video](https://www.youtube.com/watch?v={playlistItem.Snippet.ResourceId.VideoId})\n\n{playlistItem.Snippet.Description}";
    }
}