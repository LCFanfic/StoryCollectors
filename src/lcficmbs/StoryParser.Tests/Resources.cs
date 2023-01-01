// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System.Diagnostics;

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser.Tests;

public static class Resources
{
  public static Stream GetTocPage ()
  {
    var resourceStream = GetTestData("toc-page.html");
    return resourceStream;
  }

  public static Stream GetTocMultiPart ()
  {
    var resourceStream = GetTestData("toc-multipart.html");
    return resourceStream;
  }

  public static Stream GetTocSinglePart ()
  {
    var resourceStream = GetTestData("toc-singlepart.html");
    return resourceStream;
  }

  private static Stream GetTestData (string localResourceName)
  {
    var resourceName = typeof(Resources).Namespace + ".TestData." + localResourceName;
    Stream? resourceStream = typeof(Resources).Assembly.GetManifestResourceStream(resourceName);
    Debug.Assert(resourceStream != null, $"'{resourceName}' not found.");

    return resourceStream;
  }
}
