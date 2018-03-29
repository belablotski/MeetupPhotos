using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetupImages
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 2)
            {
                DateTime t0 = DateTime.Now;
                string albumId = args[0];
                string outputDir = args[1];
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                MeetupComPhotoAlbum gallery = new MeetupComPhotoAlbum(albumId, outputDir);
                gallery.Fetch();
                Console.WriteLine($"All done, it took {DateTime.Now - t0}");
                return 0;
            }
            else
            {
                Console.WriteLine("Program requires two arguments: album id and path to target folder to save photos\nAlbum id is from meetup.com URL 'See all photos' right under images thumbnails\nLike 28660769 from https://www.meetup.com/adventurers-100/photos/all_photos/?photoAlbumId=28660769\nExample: MeetupImages.exe 28660769 c:\\temp");
                return 2;
            }
        }
    }
}
