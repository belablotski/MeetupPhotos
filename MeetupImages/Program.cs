using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetupImages
{
    class Program
    {
        static void Main(string[] args)
        {
            MeetupComPhotoAlbum gallery = new MeetupComPhotoAlbum("28660769", "c:\\temp\\tests");
            gallery.Fetch();
            Console.WriteLine("done");
        }
    }
}
