// Dependencies
using System.Text.RegularExpressions;
using VideoLibrary;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos.Streams;

class Program
{
    static void Main(string[] args)
    {
        runProgram();
        Console.ReadKey();
    }

    static string MakeValidFileName(string name)
    {
        string invalidChars = new string(Path.GetInvalidFileNameChars());
        string regexPattern = $"[{Regex.Escape(invalidChars)}]";
        return Regex.Replace(name, regexPattern, "_");
    }

    static async void runProgram()
    {
        var youtube = new YoutubeClient();

        Console.Write("WELCOME TO YTSCOTT!\n" +
                      "-------------------\n" +
                      "Please provide a valid playlist URL.\n\n");

        startLabel:
        Console.Write("Playlist URL: ");
        var playlistUrl = Console.ReadLine();

        var playlist = youtube.Playlists.GetAsync(playlistUrl);
        Console.WriteLine($"\nLoaded: \"{playlist.Result.Title}\" by {playlist.Result.Author}");

        confirmSelection:
        Console.Write("\nIs this information correct? (Y/N): ");
        var input = Console.ReadLine();
        if (input != null)
        {
            input = input.ToUpper();
            if (input == "Y")
            {
                Console.WriteLine("\nCONFIRMED.\nProceeding with downloads...");
                var videos = youtube.Playlists.GetVideosAsync(playlist.Result.Id);
                await foreach (PlaylistVideo video in videos)
                {
                    Console.Write($"\nDOWNLOADING --> ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(video.Title + "\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    var manifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                    var streamInfo = manifest.GetAudioStreams().GetWithHighestBitrate();
                    /*var stream = await youtube.Videos.Streams.GetAsync(streamInfo);*/
                    Directory.CreateDirectory(playlist.Result.Title);
                    var filePath = $"./{playlist.Result.Title}/{MakeValidFileName(video.Title)}.mp3";
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
                    if (File.Exists(filePath))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("DOWNLOADED.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FAILED.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            else if (input == "N")
            {
                goto startLabel;
            }
            else
            {
                Console.WriteLine("\nINVALID INPUT. TRY AGAIN.");
                goto confirmSelection;
            }
        }
        else
        {
            Console.WriteLine("\nINVALID INPUT. TRY AGAIN.");
            goto confirmSelection;
        }
    }
}