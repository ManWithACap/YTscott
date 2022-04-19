using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YTscott
{
    class Program
    {
        static YoutubeClient youtube = new YoutubeClient();

        static string url;

        // !!! START OF THE APP !!!
        static void Main(string[] args)
        {
            Startup();
        }

        // HELPER FUNCTION : used as a sort of "title card" to start and introduce the app.
        public static void Startup()
        {
            Console.WriteLine(
                " _  _  ____  ___   ___  _____  ____  ____\n" +
                "( \\/ )(_  _)/ __) / __)(  _  )(_  _)(_  _)\n" +
                " \\  /   )(  \\__ \\( (__  )(_)(   )(    )(\n" +
                " (__)  (__) (___/ \\___)(_____) (__)  (__)\n\n" +
                "---------------------------------------------\n"
                );
            Console.WriteLine("Welcome to YTscott, " +
                "\na command-line console application YouTube playlist to MP3 set converter \n" +
                "designed for use almost exclusively by Scott Bader. " +
                "\nUse it at your own risk, I guess. \n" +
                "(It's like, kinda illegal. Just don't do bad people things. I use it for my own playlists on old MP3 players." +
                "\nI sync my Spotify playlists with my YouTube playlists and then just download them through this thing to the players)\n" +
                "\nPRESS ANY KEY TO CONTINUE...");
            Console.ReadKey();
            Task.WaitAll(AskForUrl());
            Task.WaitAll(GetSongs());
            Console.WriteLine("\n\n\nPRESS ANY KEY TO EXIT...");
            Console.ReadKey();
        }

        // HELPER FUNCTION : used to get the URL variable for the program.
        public static async Task AskForUrl()
        {
            Console.Write("\n\nYouTube URL: ");
            string url = Console.ReadLine();
            try
            {
                Program.url = url;
                var videos = await youtube.Playlists.GetVideosAsync(Program.url);
                var playlist = await youtube.Playlists.GetAsync(Program.url);
                Console.WriteLine("\n\n:  " + playlist.Title + "  :  by " + playlist.Author + "  :\n" +
                                  "---------------------------------------------------\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n\n");
                Task.WaitAll(AskForUrl());
            }
        }

        // HELPER FUNCTION : used to get the data for every song in the playlist
        public static async Task GetSongs()
        {
            try
            {
                var videos = await youtube.Playlists.GetVideosAsync(Program.url);
                
                foreach (var song in videos)
                {
                    Console.WriteLine(":  " + song.Title + "  :  by " + song.Author + "  :");
                }

                Console.WriteLine("PRESS ANY BUTTON TO CONTINUE TO DOWNLOAD ALL...");
                Console.ReadKey();
                Task.WaitAll(DownloadSongs());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n\n");
                Task.WaitAll(AskForUrl());
            }
        }

        public static async Task DownloadSongs()
        {
            try
            {
                var videos = await youtube.Playlists.GetVideosAsync(Program.url);
                var playlist = await youtube.Playlists.GetAsync(Program.url);
                Directory.CreateDirectory("./playlists/" + playlist.Title + "/");

                foreach (var song in videos)
                {
                    Console.Write("\n:  " + song.Title + "  :  ");
                    youtube.Videos.DownloadAsync(song.Url, "./playlists/" + playlist.Title + "/" + song.Title + ".mp3");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("■");
                    Console.ForegroundColor = ConsoleColor.White;
                    //■
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message + "\n\n");
                Task.WaitAll(DownloadSongs());
            }
        }
    }
}
