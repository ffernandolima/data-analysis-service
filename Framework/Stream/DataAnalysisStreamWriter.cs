using System;
using System.IO;

namespace DataAnalysis.Framework.Stream
{
    public class DataAnalysisStreamWriter : StreamWriter
    {
        public DataAnalysisStreamWriter(string path)
            : base(GetBufferedStream(path))
        {
        }

        private static BufferedStream GetBufferedStream(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentOutOfRangeException("path", "Path cannot be null or empty.");
            }

            var bufferSize = Math.Max(StreamConstants.MIN_BUFFER_SIZE, Math.Min(path.Length, StreamConstants.MAX_BUFFER_SIZE));
            var fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            var bufferedStream = new BufferedStream(fileStream, bufferSize);
            return bufferedStream;
        }
    }
}
