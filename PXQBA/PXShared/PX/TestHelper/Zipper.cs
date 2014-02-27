namespace TestHelper
{
    using System;
    using System.IO;
    using System.IO.Packaging;

    /// <summary>
    /// This is utility to create zip file and extract file using WindowsBase -- System.IO.Package
    /// </summary>
    public class Zipper : IZipper
    {
        #region Variable

        private const long BufferSize = 4096;
        private IZipStreamProvider ZipStreamProvider { get; set; }

        #endregion

        #region Zipper Property

        /// <summary>
        /// Gets or sets zip file name
        /// </summary>
        public string ZipFileName { get; set; }

        #endregion

        public Zipper(string zipFilename)
        {
            ZipFileName = zipFilename;
            //By default, write to file
            ZipStreamProvider = new ZipStreamProvider();
        }

        public void ExtractAll()
        {
            ExtractAll(Environment.CurrentDirectory);
        }

        public void ExtractAll(string folder)
        {
            using (var zip = Package.Open(ZipStreamProvider.GetStream(ZipFileName)))
            {
                foreach (var part in zip.GetParts())
                {
                    using (var reader = new StreamReader(part.GetStream(FileMode.Open, FileAccess.Read)))
                    {
                        using (var writer = ZipStreamProvider.GetStream(folder + "\\" + Path.GetFileName(part.Uri.OriginalString)))
                        {
                            var buffer = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
                            writer.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to create a file on the fly with byte array and then zip the file using Zipper Add<see cref="AddFile"/> method
        /// </summary>
        /// <param name="allBytes">byte array to write</param>
        /// <param name="fileName">file name to be used for byte array to push</param>
        public void CreateZipFromByte(byte[] allBytes, string fileName ="meta.xml")
        {
            string destFilename = string.Format(".\\{0}", fileName);
            File.WriteAllBytes(destFilename, allBytes);
            this.AddFile(destFilename);
            if (File.Exists(destFilename))
            {
                File.Delete(destFilename);
            }
        }

        /// <summary>
        /// Add file to zip
        /// </summary>
        /// <param name="fileToAdd">file name to add into zip</param>
        public void AddFile(string fileToAdd)
        {
            using (var zip = Package.Open(ZipFileName, FileMode.OpenOrCreate))
            {
                var destFilename = ".\\" + Path.GetFileName(fileToAdd);
                var uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                if (zip.PartExists(uri))
                {
                    zip.DeletePart(uri);
                }
                var part = zip.CreatePart(uri, "", CompressionOption.Normal);
                using (var fileStream = ZipStreamProvider.GetStream(fileToAdd))
                {
                    if (part != null)
                    {
                        using (var dest = part.GetStream())
                        {
                            this.CopyStream(fileStream, dest);
                        }
                    }
                }
            }
        }
        
        // ReSharper disable once UnusedMethodReturnValue.Local
        private long CopyStream(Stream inputStream, Stream outputStream)
        {
            var bufferSize = inputStream.Length < BufferSize ? inputStream.Length : BufferSize;
            var buffer = new byte[bufferSize];
            int bytesRead;
            var bytesWritten = 0L;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }

            return bytesWritten;
        }
    }

    public interface IZipper
    {
        //void Create();
        void ExtractAll();
        void ExtractAll(string folder);
        //void Extract(string fileName);
        void AddFile(string fileName);

        void CreateZipFromByte(byte[] allBytes, string fileName = "meta.xml");
    }

    public interface IZipStreamProvider
    {
        Stream GetStream(string fileName);
    }

    public class ZipStreamProvider : IZipStreamProvider
    {
        public Stream GetStream(string fileName)
        {
            //Create a read/writable file
            return new FileStream(fileName, FileMode.OpenOrCreate);
        }
    }
}
