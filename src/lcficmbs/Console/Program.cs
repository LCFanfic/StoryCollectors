// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using LCFanfic.StoryCollectors.lcficmbs.StoryParser;

namespace LCFanfic.StoryCollectors.lcficmbs.Console;

public static class Program
{
  public static void Main (string[] args)
  {
    var outputPath = args.Length > 0 ? args[0] : "stories.json";
    var rating = args.Length > 1 & string.Equals(args[1], "nfic", StringComparison.OrdinalIgnoreCase) ? Rating.NFic : Rating.GFic;
    var username = args.Length > 3 ? args[2] : null;
    var password = args.Length > 3 ? args[3] : null;
    if (rating == Rating.NFic && (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)))
    {
      System.Console.WriteLine("nfic stories require a username and password.");
      return;
    }

    using var handler = new HttpClientHandler();
    using var httpClient = new HttpClient(handler);

    if (rating == Rating.NFic)
    {
      Debug.Assert(username != null, "username != null");
      Debug.Assert(password != null, "password != null");

      var sessionCookieFactory = new SessionCookieFactory();
      handler.CookieContainer = sessionCookieFactory.GetSessionCookiesAsync(username, password);
    }

    var storyReader = new StoryReader(System.Console.Out, httpClient, rating);
    var storyMetadata = storyReader
        .ReadStoriesFromTableOfContents(tocPages: rating ==Rating.GFic ? 1..11 : 1..1, includeIncomplete: false)
        .Where(story => story.CompletionDate?.Year == 2022);

    var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default);
    jsonSerializerOptions.WriteIndented = true;
    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    var json = JsonSerializer.SerializeToUtf8Bytes(storyMetadata, jsonSerializerOptions);
    File.WriteAllBytes(outputPath, json);
  }
}
