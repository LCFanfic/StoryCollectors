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
    // https://archiveofourown.org/collections/{collectionYear}_Kerth_Award_Eligible_Stories/works
    return Enumerable.Range(pages.Start.Value, pages.End.Value)
        .Select(
            page =>
                $"https://archiveofourown.org/works?work_search[sort_column]=created_at&work_search[complete]=T&work_search[date_from]={collectionYear}-01-01&work_search[date_to]={collectionYear}-12-31&commit=Sort+and+Filter&fandom_id=11144&page={page}")
        .SelectMany(baseUrl => new[] { "CalliopeWayne", "90sfangirl79" }.Select(author => baseUrl + "&user_id=" + author))
        .Select(url => new Uri(url))
        .Select(
            url =>
            {
              _logOutput.WriteLine(url);
              return _httpClient.GetStreamAsync(url).GetAwaiter().GetResult();
            })
        .SelectMany(file => new StoryParser.WorksListingParser().GetStoryEntries(file))
        .Select(story =>
        {
          _logOutput.WriteLine(story.Title + " by " + story.Author);
          return story;
        });
  }
}
