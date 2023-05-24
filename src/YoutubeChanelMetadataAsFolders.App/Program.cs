using System.Text.RegularExpressions;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

const string youtubeApiKey = "YOUR_YOUTUBE_API_KEY";
const string channelId = "YOUR_YOUTUBE_CHANNEL_ID";
const string baseDirectory = "BASE_FOLDER";

const string videosDirectory = "Videos";
const string shortsDirectory = "Shorts";
const string applicationName = "YoutubeChannelMetadataAsFolders";

using var httpClient = new HttpClient();

var youtubeService = new YouTubeService(new BaseClientService.Initializer
{
    ApiKey = youtubeApiKey,
    ApplicationName = applicationName
});

var channelRequest = youtubeService.Channels.List("contentDetails");
channelRequest.Id = channelId;
var channelResponse = await channelRequest.ExecuteAsync();

var channel = channelResponse.Items.FirstOrDefault();
var playlistId = channel?.ContentDetails.RelatedPlaylists.Uploads;

var nextPageToken = "";
while (nextPageToken != null)
{
    var playlistRequest = youtubeService.PlaylistItems.List("snippet");
    playlistRequest.PlaylistId = playlistId;
    playlistRequest.MaxResults = 50;
    playlistRequest.PageToken = nextPageToken;

    var playlistResponse = await playlistRequest.ExecuteAsync();

    foreach (var playlistItem in playlistResponse.Items)
    {
        var videoTitle = playlistItem.Snippet.Title;
        var dateStamp = playlistItem.Snippet.PublishedAt?.ToString("yyyyMMdd") ?? "";
        var folderName = dateStamp + "_" + ToSafeFolderName(videoTitle);

        var videoOrShortsPath = Path.Combine(
            baseDirectory,
            videoTitle.Contains("#shorts") ? shortsDirectory : videosDirectory);
        var folderPath = Path.Combine(videoOrShortsPath, folderName);
        Directory.CreateDirectory(folderPath);

        var readmeContent = await CreateReadmeContent(playlistItem, httpClient);
        File.WriteAllText(Path.Combine(folderPath, "README.md"), readmeContent);
    }

    nextPageToken = playlistResponse.NextPageToken;
}

static string ToSafeFolderName(string name)
{
    var safeName = Regex.Replace(name, "[^0-9a-zA-Z _]+", "");
    safeName = safeName.Replace(' ', '_');
    return safeName;
}

static async Task<string> CreateReadmeContent(
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