// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LCFanfic.StoryCollectors.ArchiveOfOurOwn.Console;

public static class Program
{
  public static void Main (string[] args)
  {
    var outputPath = args.Length > 0 ? args[0] : "stories.json";

    using var handler = new HttpClientHandler();
    using var httpClient = new HttpClient(handler);

    const int monthApril = 4;
    var today = DateTime.Today;
    var previousYear = today.Year - 1;
    var currentYear = today.Year;
    var collectionYear = today.Month <= monthApril ? previousYear : currentYear;

    var storyReader = new StoryReader(System.Console.Out, httpClient);
    var storyMetadata = storyReader.ReadStoriesFromCollection(pages: 1..3, collectionYear);

    var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default);
    jsonSerializerOptions.WriteIndented = true;
    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    var json = JsonSerializer.SerializeToUtf8Bytes(storyMetadata, jsonSerializerOptions);
    File.WriteAllBytes(outputPath, json);
  }
}
