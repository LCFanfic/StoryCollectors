// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System;
using LCFanfic.StoryCollectors.ArchiveOfOurOwn.StoryParser;

namespace LCFanfic.StoryCollectors.ArchiveOfOurOwn.Console;

public class StoryReader
{
  private readonly TextWriter _logOutput;
  private readonly HttpClient _httpClient;


  public StoryReader (TextWriter logOutput, HttpClient httpClient)
  {
    _logOutput = logOutput;
    _httpClient = httpClient;
  }

  public IEnumerable<StoryMetadata> ReadStoriesFromCollection (Range pages, int collectionYear)
  {
    var baseUrl = $"https://archiveofourown.org/collections/{collectionYear}_Kerth_Award_Eligible_Stories/works/";
    var worksListingParser = new StoryParser.WorksListingParser(new Uri(baseUrl));
    return Enumerable.Range(pages.Start.Value, pages.End.Value)
        .Select(page => baseUrl + "?page=" + page)
        .Select(url => new Uri(url))
        .Select(
            url =>
            {
              _logOutput.WriteLine(url);
              try
              {
                return _httpClient.GetStreamAsync(url).GetAwaiter().GetResult();
              }
              catch (Exception ex)
              {
                _logOutput.WriteLine(ex.Message);
                return null;
              }
            })
        .SelectMany(file => file != null ? worksListingParser.GetStoryEntries(file) : Enumerable.Empty<StoryMetadata>())
        .Select(story =>
        {
          _logOutput.WriteLine(story.Title + " by " + story.Author);
          return story;
        });
  }
}
