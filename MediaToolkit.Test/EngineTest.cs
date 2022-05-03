using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using System.Runtime.InteropServices;

namespace MediaToolkit.Test
{
  public class EngineTest
  {
    /// <summary>
    /// Path to ffprobe.exe should be composed using the same directory as ffmpeg
    /// </summary>
    [Fact]
    public void Should_Initialize_FFprobe_Path()
    {
      var ffmpeg_string = @"c:\some\folder\path\ffmpeg.exe";
      var ffprobe_string = @"c:\some\folder\path\ffprobe.exe";
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        ffmpeg_string = @"/some/folder/path/ffmpeg";
        ffprobe_string = @"/some/folder/path/ffprobe";
      }

      // Create SUT directly intentionally
      var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {ffmpeg_string, new MockFileData("")},
                {ffprobe_string, new MockFileData("")}
            });

      var engine = new Engine(ffmpeg_string, fileSystem);

      Assert.Equal(ffmpeg_string, engine.FfmpegFilePath);
      Assert.Equal(ffprobe_string, engine.FfprobeFilePath);
    }
  }
}
