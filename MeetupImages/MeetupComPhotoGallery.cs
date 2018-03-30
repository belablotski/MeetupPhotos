using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace MeetupImages
{
    class MeetupComPhotoAlbum
    {
        private bool SaveHtmlForDebug = false;
        public string AlbumId { get; }
        public string OutputDir { get; }

        public MeetupComPhotoAlbum(string AlbumId, string OutputDir)
        {
            this.AlbumId = AlbumId;
            this.OutputDir = OutputDir;
        }

        /// <summary>
        /// Fetch photo album
        /// </summary>
        public void Fetch()
        {
            int count = 1;
            HashSet<string> urlsToFetch = FetchAlbumHtmlPage();
            var en = urlsToFetch.GetEnumerator();
            while (en.MoveNext()) {
                Console.WriteLine($"Urls fetched {count} of {urlsToFetch.Count}");
                FetchPhotoHtmlPage(en.Current);
                Thread.Sleep(new TimeSpan(0, 0, 3));    // I wouldn't like to overload meetup.com
                count++;
            }
        }

        /// <summary>
        /// Fetch and parse photo album HTML page
        /// </summary>
        /// <returns>Set of photo page URLs</returns>
        private HashSet<string> FetchAlbumHtmlPage()
        {
            HashSet<string> result = new HashSet<string>();
            string url = $"https://www.meetup.com/adventurers-100/photos/all_photos/?photoAlbumId={AlbumId}";
            Console.WriteLine($"Fetching photo album page '{url}'");
            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString(url);
                if (SaveHtmlForDebug)
                {
                    File.WriteAllText(Path.Combine(OutputDir, $"album_{AlbumId}.html"), html);
                }
                string patternPage = $"<a href=\"(https://www.meetup.com/adventurers-100/photos/{AlbumId}/.{{1,12}}/)\"";
                foreach (Match m in Regex.Matches(html, patternPage, RegexOptions.IgnoreCase))
                {
                    Trace.Assert(m.Groups.Count == 2, "Internal URL group isn't found in HTML (parsing album page)");
                    result.Add(m.Groups[1].Value);
                }
            }
            return result;
        }

        /// <summary>
        /// Fetch one HTML page from gallery
        /// </summary>
        /// <returns>Set of URLs of other gallery pages, referenced on the current page</returns>
        private void FetchPhotoHtmlPage(string url)
        {
            url = Regex.Replace(url, @"/\s*$", "");
            string imageId = Regex.Replace(url, @"^.*/", "");
            Console.WriteLine($"Fetching page of image '{imageId}' the url is '{url}'...");
            string patternHighres = $@"https://secure.meetupstatic.com/photos/event/.{{2,12}}/highres_{imageId}.jpeg";
            string patternGalleryPages = "<a href=\"(https://www.meetup.com/adventurers-100/photos/28660769/.{6,12}/)\" class=\"J_onClick J_photoGallery_getImage\" title=\"\">";
            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString(url);
                if (SaveHtmlForDebug)
                {
                    File.WriteAllText(Path.Combine(OutputDir, $"{imageId}.html"), html);
                }
                Match match = Regex.Match(html, patternHighres, RegexOptions.IgnoreCase);
                Trace.Assert(match.Success, "Can't extract URL to high-resolution image from photo HTML page");
                FetchImage(match.Value);
            }
        }

        /// <summary>
        /// Fetch one image and save it to the file with the same name as in URL
        /// </summary>
        private void FetchImage(string url)
        {
            string filePath = Path.Combine(OutputDir, Regex.Replace(url, "^.*/", ""));
            using (WebClient client = new WebClient())
            {
                Console.WriteLine($"Fetching image file '{url}' into '{filePath}'");
                client.DownloadFile(url, filePath);
            }
        }
    }
}
