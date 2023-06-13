using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using YoutubeChanelMetadataAsFolders.App;

const string youtubeApiKey = "YOUR_YOUTUBE_API_KEY";
const string channelId = "YOUR_YOUTUBE_CHANNEL_ID";
const string baseDirectory = "BASE_FOLDER";

const string videosDirectory = "Videos";
const string shortsDirectory = "Shorts";
const string applicationName = "YoutubeChannelMetadataAsFolders";

int? onlyLastMessages = null;

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

var nextPageToken = string.Empty;
var videoCounter = 0;
while (nextPageToken != null)
{
    var playlistRequest = youtubeService.PlaylistItems.List("snippet");
    playlistRequest.PlaylistId = playlistId;
    playlistRequest.MaxResults = 50;
    playlistRequest.PageToken = nextPageToken;

    var playlistResponse = await playlistRequest.ExecuteAsync();

    foreach (var playlistItem in playlistResponse.Items)
    {
        videoCounter++;
        
        if (videoCounter > onlyLastMessages)
        {
            Console.WriteLine($"Processed the specified number of {onlyLastMessages} videos. Exiting...");
            return;
        }

        var videoTitle = playlistItem.Snippet.Title;
        var dateStamp = playlistItem.Snippet.PublishedAt?.ToString("yyyyMMdd") ?? string.Empty;
        var safeFolderName = HelperFunctions.ToSafeFolderName(videoTitle);
        var folderName = $"{dateStamp}_{safeFolderName}";

        Console.WriteLine($"Processing video {videoCounter}: {videoTitle}");

        var videoOrShortsPath = Path.Combine(
            baseDirectory,
            videoTitle.Contains("#shorts") ? shortsDirectory : videosDirectory);
        var folderPath = Path.Combine(videoOrShortsPath, folderName);
        Directory.CreateDirectory(folderPath);

        var readmeContent = await HelperFunctions.CreateReadmeContent(playlistItem, httpClient);
        File.WriteAllText(Path.Combine(folderPath, "README.md"), readmeContent);
    }

    nextPageToken = playlistResponse.NextPageToken;
}

Console.WriteLine($"Finished - total videos processed: {videoCounter}");