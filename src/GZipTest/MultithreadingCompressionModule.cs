using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    // TODO compress blocks with System.IO.Compression.GZipStream
    // TODO Write blocks independently into out file
    public static class MultithreadingCompressionModule
    {
        private const int FileChunkSizeBytes = 1024;

        public static VeeamResult Compress(FileInfo inputFileInfo, FileInfo outputFileInfo)
        {
            if (!inputFileInfo.Exists)
            {
                return new VeeamError($"File {inputFileInfo.FullName} does not exist.");
            }

            using var inputFileStream = inputFileInfo.OpenRead(); // TODO try-catch
            using var outputFileStream = outputFileInfo.Create(); // TODO try-catch
            var chunkIndex = -1;
            var additionalThreads = new Thread[0];//new Thread[Environment.ProcessorCount - 1];
            for (var threadIndex = 0; threadIndex < additionalThreads.Length; threadIndex++)
            {
                // ReSharper disable AccessToDisposedClosure
                // ReSharper disable once AccessToModifiedClosure
                additionalThreads[threadIndex] = new Thread(() => ProcessChunk(inputFileStream, outputFileStream, ref chunkIndex));
                // ReSharper enable AccessToDisposedClosure
                additionalThreads[threadIndex].Start();
            }

            // Use current thread as additional processing thread (or as the only thread in one-processor environment)
            ProcessChunk(inputFileStream, outputFileStream, ref chunkIndex);

            foreach (var thread in additionalThreads)
            {
                thread!.Join();
            }

            return true;
        }

        private static VeeamResult ProcessChunk(Stream inputStream, Stream outputStream, ref int chunkIndex)
        {
            var chunk = ReadChunk(inputStream, ref chunkIndex);
            var compressedChunk = CompressChunk(chunk.Value);
            return WriteChunk(compressedChunk.Value, chunkIndex, outputStream);
        }

        private static VeeamDataResult<byte[]> ReadChunk(Stream stream, ref int chunkIndex)
        {
            chunkIndex = Interlocked.Increment(ref chunkIndex);
            var chunk = new byte[FileChunkSizeBytes];
            var offset = chunkIndex * FileChunkSizeBytes;
            var count = FileChunkSizeBytes; // TODO count last chunk size
            // TODO use BeginRead ?
            stream.Read(buffer: chunk, offset: offset, count: count); // TODO try-catch
            return chunk;
        }

        private static VeeamDataResult<byte[]> CompressChunk(byte[] chunk)
        {
            using var compressedChunkStream = new MemoryStream(FileChunkSizeBytes);
            using var compressionStream = new GZipStream(compressedChunkStream, CompressionMode.Compress);
            using var chunkStream = new MemoryStream(chunk);
            chunkStream.CopyTo(compressionStream); // TODO try-catch
            var compressedChunk = compressedChunkStream.ToArray();
            return compressedChunk;
        }

        private static readonly object WriterLock = new object();
        private static VeeamResult WriteChunk(IReadOnlyCollection<byte> chunk, int chunkIndex, Stream outputStream)
        {
            using var streamWriter = new StreamWriter(outputStream);
            // TODO threads are depending on each other with this lock
            lock (WriterLock)
            {
                //streamWriter.Write(chunkIndex);
                //streamWriter.Write(chunk.Count);
                //streamWriter.Write();
            }

            return true;
        }

        public static VeeamResult Decompress(FileInfo inputFileInfo, FileInfo outputFileInfo) => true;
    }
}