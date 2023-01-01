// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using LCFanfic.StoryCollectors.lcficmbs.StoryParser;

namespace LCFanfic.StoryCollectors.lcficmbs.Console;

internal class StoryReader
{
  private readonly Rating _rating;
  private readonly TextWriter _logOutput;
  private readonly HttpClient _httpClient;


  public StoryReader (TextWriter logOutput, HttpClient httpClient, Rating rating)
  {
    _rating = rating;
    _logOutput = logOutput;
    _httpClient = httpClient;
  }

  public IEnumerable<StoryMetadata> ReadStoriesFromTableOfContents (Range tocPages, bool includeIncomplete)
  {
    return Enumerable.Range(tocPages.Start.Value, tocPages.End.Value)
        .Select(page => new Uri($"https://www.lcficmbs.com/ubb/ubbthreads.php/forums/{(_rating == Rating.GFic ? 6 : 7)}/{page}"))
        .Select(url => _httpClient.GetStreamAsync(url).GetAwaiter().GetResult())
        .SelectMany(file => GetTocEntries(file))
        .Where(toc => !toc.IsSeries)
        .Where(toc => toc.IsComplete || includeIncomplete)
        .Select(toc => (Toc:toc, StoryPartUrls: GetStoryPartUrls(toc)))
        .Select(item => GetStoryMetadata(item.Toc,item.StoryPartUrls));
  }

  private TocEntry[] GetTocEntries (Stream file)
  {
    return new TocParser().GetTocEntries(file);
  }

  private Uri[] GetStoryPartUrls (TocEntry toc)
  {
    var tocEntryParser = new TocEntryParser();
    var tocEntry = _httpClient.GetStreamAsync(toc.Url).GetAwaiter().GetResult();
    _logOutput.WriteLine($"Getting TOC: {toc.StoryTitle}");
    return tocEntryParser.GetTocEntryParts(tocEntry).Select(p => p.Url).ToArray();
  }

  private StoryMetadata GetStoryMetadata (TocEntry toc, Uri[] storyPartUrls)
  {
    var storyPartParser = new StoryPartParser();
    var storyParts = new List<StoryPartMetadata>();

    _logOutput.WriteLine($"Getting story metadata: {toc.StoryTitle}");
    for (var index = 0; index < storyPartUrls.Length; index++)
    {
      var url = storyPartUrls[index];
      _logOutput.WriteLine($"Reading part {index + 1} of {storyPartUrls.Length}");

      var storyPart = _httpClient.GetStreamAsync(url).GetAwaiter().GetResult();
      try
      {
        var storyPartMetadata = storyPartParser.GetStoryPartMetadata(storyPart);
        if (storyPartMetadata.Forum is Forum.Fanfic or Forum.NFanfic or Forum.FanficChallenge)
          storyParts.Add(storyPartMetadata);
      }
      catch (Exception e)
      {
        _logOutput.WriteLine($"Error parsing {toc.StoryTitle} part {index}: {url}");
        _logOutput.WriteLine(e);
      }
    }

    return new StoryMetadata(
        Toc: toc.Url,
        Rating: _rating,
        Title: toc.StoryTitle,
        Author: toc.StoryAuthor,
        CompletionDate: toc.IsComplete ? storyParts.Max(p => p.CompletionDate) : null,
        LengthInBytes: toc.IsComplete ? storyParts.Sum(p => p.LengthInBytes) : null,
        WordCount: toc.IsComplete ? storyParts.Sum(p => p.WordCount) : null
        );
  }
}
