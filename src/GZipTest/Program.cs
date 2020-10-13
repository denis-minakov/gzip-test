using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            args = new[] {"compress", "ImageToCompress.png", "ImageToCompress.gz"};

            const int successExitCode = 0;
            const int errorExitCode = -1;

            var result = Process(args);
            if (!result.IsSuccess)
            {
                Console.WriteLine(result.ErrorInfo.ErrorText);
                return errorExitCode;
            }

            Console.WriteLine("Program finished successfully.");
            return successExitCode;
        }

        private static VeeamResult Process(IReadOnlyList<string> args)
        {
            var gzipInputResult = ParseArgs(args);
            if (!gzipInputResult.HasValue)
            {
                return gzipInputResult.ErrorInfo;
            }

            var gzipInput = gzipInputResult.Value;
            return gzipInput!.CompressionMode switch
            {
                CompressionMode.Compress => MultithreadingCompressionModule.Compress(gzipInput.InputFileInfo, gzipInput.OutputFileInfo),
                CompressionMode.Decompress => MultithreadingCompressionModule.Decompress(gzipInput.InputFileInfo, gzipInput.OutputFileInfo),
                _ => new VeeamError($"Unsupported value of compression mode {gzipInput.CompressionMode}.")
            };
        }

        private static VeeamDataResult<GzipParsedArgs> ParseArgs(IReadOnlyList<string> args)
        {
            if (args.Count != 3)
            {
                var errorText = $"Expected 3 input parameters, got {args.Count}."; // TODO refer to help
                return new VeeamError(errorText);
            }

            const string compressionModeCommand = "compress";
            const string decompressionModeCommand = "decompress";

            CompressionMode compressionMode;
            var commandArgument = args[0];
            switch (commandArgument)
            {
                case compressionModeCommand:
                    compressionMode = CompressionMode.Compress;
                    break;
                case decompressionModeCommand:
                    compressionMode = CompressionMode.Decompress;
                    break;
                default:
                {
                    var errorText =
                        "Allowed first argument values are "
                      + $"{compressionModeCommand} or {decompressionModeCommand}, got {commandArgument}"; // TODO refer to help
                    return new VeeamError(errorText);
                }
            }

            var inputFileInfo = new FileInfo(args[1]);
            var outputFileInfo = new FileInfo(args[2]);
            return new GzipParsedArgs(compressionMode, inputFileInfo, outputFileInfo);
        }

        private sealed class GzipParsedArgs
        {
            public CompressionMode CompressionMode { get; }
            public FileInfo InputFileInfo { get; }
            public FileInfo OutputFileInfo { get; }

            public GzipParsedArgs(CompressionMode compressionMode, FileInfo inputFileInfo, FileInfo outputFileInfo)
            {
                CompressionMode = compressionMode;
                InputFileInfo = inputFileInfo;
                OutputFileInfo = outputFileInfo;
            }
        }
    }
}