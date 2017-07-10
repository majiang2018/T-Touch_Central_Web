using System.IO;
using System.Web;

namespace T_Touch_Central_Web
{
    public class FileModel
    {
        public HttpPostedFileBase MyFile { get; set; }

        public FileInfo[] MyInfo { get; set; }
    }
}