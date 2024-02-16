using Google.Protobuf;
using System.Drawing;

namespace Common.Interfaces
{
    public interface IFileService
    {
        public void SaveFile(string filename, ByteString content);
        public void SaveFile(string filename, Bitmap content);
    }
}
