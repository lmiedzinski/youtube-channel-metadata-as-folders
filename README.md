# Youtube Channel Metadata As Folders

This application generates folder structures for a given YouTube channel. Each video on the channel is represented by a separate folder, named according to the video's title, and prepended by the video's publication date.

The folders are created in a specified base directory, within which two subdirectories are created: one for standard videos, and one for YouTube Shorts. Each video's folder contains a `README.md` file, which includes the video title, a link to the video, the video's description, and the video's thumbnail image.

Additionally, the application provides an option to only process a specified number of the most recent videos on the channel.

## Project Structure

The project is composed of the following files:

- `YoutubeChanelMetadataAsFolders.App.csproj`: This is the project file which includes project configurations and package references.
- `HelperFunctions.cs`: This file includes helper functions that are used in the main program. The functions include `ToSafeFolderName`, which sanitizes the video's title for use as a folder name, and `CreateReadmeContent`, which generates the content for the `README.md` file in each video's folder.
- `Program.cs`: This is the main program file which includes the main logic of the application.

## Usage

Before running the application, replace `YOUR_YOUTUBE_API_KEY`, `YOUR_YOUTUBE_CHANNEL_ID`, and `BASE_FOLDER` in `Program.cs` with your actual YouTube API key, channel ID, and desired directory path, respectively.

To process a specified number of the most recent videos, set the `onlyLastMessages` variable at the beginning of `Program.cs` to the desired number. If this variable is set to `null`, the program will process all videos.

To run the application, simply execute the `Program.cs` file.

## Dependencies

- .NET 7.0
- Google.Apis.YouTube.v3 1.60.0.3043
