using System;
using System.IO;

namespace DataAnalysis.Framework.Stream
{
    public class DataAnalysisStreamReader : StreamReader
    {
        public DataAnalysisStreamReader(FileInfo fileInfo)
            : base(GetBufferedStream(fileInfo))
        {
        }

        private static BufferedStream GetBufferedStream(FileInfo fileInfo)
        {
            if (fileInfo == null || fileInfo.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("fileInfo", "file info cannot be null or empty.");
            }

            var bufferSize = (int)Math.Max(StreamConstants.MIN_BUFFER_SIZE, Math.Min(fileInfo.Length, StreamConstants.MAX_BUFFER_SIZE));
            var fileStream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var bufferedStream = new BufferedStream(fileStream, bufferSize);
            return bufferedStream;
        }
    }
}
