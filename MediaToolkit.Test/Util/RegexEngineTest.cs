using MediaToolkit.Model;
using System;
using System.Linq;
using Xunit;

namespace MediaToolkit.Test.Util
{
  public class RegexEngineTest
  {
    [Fact]
    public void TestVideo_VideoStreamInformationWithoutFPS_IsIgnored()
    {
      // this throws on before the patch

      const string faultyData = @"    Stream #0:1: Video: mjpeg, yuvj420p(pc), 200x198 [SAR 96:96 DAR 100:99], 90k tbr, 90k tbn, 90k tbc";

      var regexEngineType = typeof(Engine).Assembly.GetTypes()
                                                   .Where(x => x.Name == "RegexEngine")
                                                   .Single();

      var engineParametersType = typeof(Engine).Assembly.GetTypes()
                                                        .Where(x => x.Name == "EngineParameters")
                                                        .Single();

      var engineParameters = Activator.CreateInstance(engineParametersType);
      engineParametersType.GetProperty("InputFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(engineParameters, new MediaFile());

      var testMethod = regexEngineType.GetMethod("TestVideo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
      testMethod.Invoke(null, new object[]
      {
                faultyData,
                engineParameters
      });
    }
  }
}
