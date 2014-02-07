﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using MediaToolkit.Model;
using MediaToolkit.Options;
using NUnit.Framework;

namespace MediaToolkit.Test
{
    [TestFixture]
    public class ConvertTest
    {
        // TODO: Create assertions for conversion etc

        [TestFixtureSetUp]
        public void Init()
        {
            // Raise progress events?
            _printToConsoleEnabled = true;

            const string inputFile = @"";
            const string outputFile = @"";

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (inputFile != "")
            {
                _inputFilePath = inputFile;
                if (outputFile != "")
                    _outputFilePath = outputFile;

                return;
            }

            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            Debug.Assert(directoryInfo.Parent != null, "directoryInfo.Parent != null");

            DirectoryInfo testDirectoryInfo = directoryInfo.Parent.Parent;
            Debug.Assert(testDirectoryInfo != null, "testDirectoryInfo != null");

            string testDirectoryPath = testDirectoryInfo.FullName + @"\TestVideo\";

            Debug.Assert(Directory.Exists(testDirectoryPath), "Directory not found: " + testDirectoryPath);
            Debug.Assert(File.Exists(testDirectoryPath + @"BigBunny.m4v"),
                "Test file not found: " + testDirectoryPath + @"BigBunny.m4v");

            _inputFilePath = testDirectoryPath + @"BigBunny.m4v";
            _outputFilePath = testDirectoryPath + @"OuputBunny.mp4";
        }

        private string _inputFilePath = "";
        private string _outputFilePath = "";
        private bool _printToConsoleEnabled;

        [TestCase]
        public void Can_CutVideo()
        {
            string outputPath = string.Format(@"{0}\Cut_Video_Test.mp4", Path.GetDirectoryName(_outputFilePath));

            var inputFile = new MediaFile {Filename = _inputFilePath};
            var outputFile = new MediaFile {Filename = outputPath};

            using (var engine = new Engine())
            {
                engine.ConvertProgressEvent += engine_ConvertProgressEvent;
                engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;

                engine.GetMetadata(inputFile);

                var options = new ConversionOptions();
                options.CutMedia(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(25));

                engine.Convert(inputFile, outputFile, options);
            }
        }

        [TestCase]
        public void Can_GetThumbnail()
        {
            string outputPath = string.Format(@"{0}\Get_Thumbnail_Test.jpg", Path.GetDirectoryName(_outputFilePath));

            var inputFile = new MediaFile {Filename = _inputFilePath};
            var outputFile = new MediaFile {Filename = outputPath};

            using (var engine = new Engine())
            {
                engine.ConvertProgressEvent += engine_ConvertProgressEvent;
                engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;

                engine.GetMetadata(inputFile);

                var options = new ConversionOptions
                {
                    Seek = TimeSpan.FromSeconds(inputFile.Metadata.Duration.TotalSeconds/2)
                };
                engine.GetThumbnail(inputFile, outputFile, options);
            }
        }

        [TestCase]
        public void Can_GetMetadata()
        {
            var inputFile = new MediaFile {Filename = _inputFilePath};

            using (var engine = new Engine())
                engine.GetMetadata(inputFile);

            Metadata inputMeta = inputFile.Metadata;

            Debug.Assert(inputMeta.Duration != TimeSpan.Zero, "Media duration is zero", "  Likely due to Regex code");
            Debug.Assert(inputMeta.VideoData.Format != null, "Video format not found", "  Likely due to Regex code");
            Debug.Assert(inputMeta.VideoData.ColorModel != null, "Color model not found", "   Likely due to Regex code");
            Debug.Assert(inputMeta.VideoData.FrameSize != null, "Frame size not found", "    Likely due to Regex code");
            Debug.Assert(inputMeta.VideoData.Fps.ToString(CultureInfo.InvariantCulture) != "0", "Fps not found",
                "           Likely due to Regex code");
            Debug.Assert(inputMeta.VideoData.BitRateKbs != 0, "Video bitrate not found", " Likely due to Regex code");
            Debug.Assert(inputMeta.AudioData.Format != null, "Audio format not found", "  Likely due to Regex code");
            Debug.Assert(inputMeta.AudioData.SampleRate != null, "Sample rate not found", "   Likely due to Regex code");
            Debug.Assert(inputMeta.AudioData.ChannelOutput != null, "Channel output not found",
                "Likely due to Regex code");
            Debug.Assert(inputMeta.AudioData.BitRateKbs != 0, "Audio bitrate not found", " Likely due to Regex code");

            PrintMetadata(inputMeta);
        }

        [TestCase]
        public void Can_ConvertBasic()
        {
            string outputPath = string.Format(@"{0}\Convert_Basic_Test.avi", Path.GetDirectoryName(_outputFilePath));

            var inputFile = new MediaFile { Filename = _inputFilePath };
            var outputFile = new MediaFile { Filename = outputPath };


            using (var engine = new Engine())
            {
                engine.ConvertProgressEvent += engine_ConvertProgressEvent;
                engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;

                engine.Convert(inputFile, outputFile);
                engine.GetMetadata(inputFile);
                engine.GetMetadata(outputFile);
            }

            Metadata inputMeta = inputFile.Metadata;
            Metadata outputMeta = outputFile.Metadata;

            PrintMetadata(inputMeta);
            PrintMetadata(outputMeta);
        }

        [TestCase]
        public void Can_ConvertToGif()
        {
            string outputPath = string.Format(@"{0}\Convert_GIF_Test.gif", Path.GetDirectoryName(_outputFilePath));

            var inputFile = new MediaFile { Filename = _inputFilePath };
            var outputFile = new MediaFile { Filename = outputPath };


            using (var engine = new Engine())
            {
                engine.ConvertProgressEvent += engine_ConvertProgressEvent;
                engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;

                engine.Convert(inputFile, outputFile);
                engine.GetMetadata(inputFile);
                engine.GetMetadata(outputFile);
            }

            Metadata inputMeta = inputFile.Metadata;
            Metadata outputMeta = outputFile.Metadata;

            PrintMetadata(inputMeta);
            PrintMetadata(outputMeta);
        }


        [TestCase]
        public void Can_ConvertToDVD()
        {
            string outputPath = string.Format("{0}/Convert_DVD_Test.vob", Path.GetDirectoryName(_outputFilePath));

            var inputFile = new MediaFile {Filename = _inputFilePath};
            var outputFile = new MediaFile {Filename = outputPath};

            var conversionOptions = new ConversionOptions {Target = Target.DVD, TargetStandard = TargetStandard.PAL};

            using (var engine = new Engine())
            {
                engine.ConvertProgressEvent += engine_ConvertProgressEvent;
                engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;

                engine.Convert(inputFile, outputFile, conversionOptions);

                engine.GetMetadata(inputFile);
                engine.GetMetadata(outputFile);
            }

            PrintMetadata(inputFile.Metadata);
            PrintMetadata(outputFile.Metadata);
        }

        [TestCase]
        public void Can_TranscodeUsingConversionOptions()
        {
            string outputPath = string.Format("{0}/Transcode_Test.avi", Path.GetDirectoryName(_outputFilePath));

            var inputFile = new MediaFile {Filename = _inputFilePath};
            var outputFile = new MediaFile {Filename = outputPath};
            var conversionOptions = new ConversionOptions
            {
                MaxVideoDuration = TimeSpan.FromSeconds(30),
                VideoAspectRatio = VideoAspectRatio.R16_9,
                VideoSize = VideoSize.Hd720,
                AudioSampleRate = AudioSampleRate.Hz44100
            };


            using (var engine = new Engine())
                engine.Convert(inputFile, outputFile, conversionOptions);
        }

        private void engine_ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            if (!_printToConsoleEnabled) return;

            Console.WriteLine("Bitrate: {0}", e.Bitrate);
            Console.WriteLine("Fps: {0}", e.Fps);
            Console.WriteLine("Frame: {0}", e.Frame);
            Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Console.WriteLine("SizeKb: {0}", e.SizeKb);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }

        private void engine_ConversionCompleteEvent(object sender, ConversionCompleteEventArgs e)
        {
            if (!_printToConsoleEnabled) return;

            Console.WriteLine("\n------------\nConversion complete!\n------------");
            Console.WriteLine("Bitrate: {0}", e.Bitrate);
            Console.WriteLine("Fps: {0}", e.Fps);
            Console.WriteLine("Frame: {0}", e.Frame);
            Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Console.WriteLine("SizeKb: {0}", e.SizeKb);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }

        private void PrintMetadata(Metadata meta)
        {
            if (!_printToConsoleEnabled) return;

            Console.WriteLine("\n------------\nMetadata\n------------");
            Console.WriteLine("Duration:                {0}", meta.Duration);
            if (meta.VideoData != null)
            {
                Console.WriteLine("VideoData.Format:        {0}", meta.VideoData.Format);
                Console.WriteLine("VideoData.ColorModel:    {0}", meta.VideoData.ColorModel);
                Console.WriteLine("VideoData.FrameSize:     {0}", meta.VideoData.FrameSize);
                Console.WriteLine("VideoData.Fps:           {0}", meta.VideoData.Fps);
                Console.WriteLine("VideoData.BitRate:       {0}", meta.VideoData.BitRateKbs);
            }
            else if (meta.AudioData != null)
            {
                Console.WriteLine("AudioData.Format:        {0}", meta.AudioData.Format ?? "");
                Console.WriteLine("AudioData.SampleRate:    {0}", meta.AudioData.SampleRate ?? "");
                Console.WriteLine("AudioData.ChannelOutput: {0}", meta.AudioData.ChannelOutput ?? "");
                Console.WriteLine("AudioData.BitRate:       {0}\n", (int?)meta.AudioData.BitRateKbs);
            }
            
        }
    }
}