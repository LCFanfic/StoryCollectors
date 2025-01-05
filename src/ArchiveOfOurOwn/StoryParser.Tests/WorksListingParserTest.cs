// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System;
using System.Text;

namespace LCFanfic.StoryCollectors.ArchiveOfOurOwn.StoryParser.Tests;

public class WorksListingParserTest
{
  private readonly WorksListingParser _worksListingParser;

  public WorksListingParserTest ()
  {
    _worksListingParser = new WorksListingParser();
  }

  [Fact]
  public void GivenTestData_FindsMatchingEntries ()
  {
    using var testData = Resources.GetAo3Listing();

    var tocEntries = _worksListingParser.GetStoryEntries(testData);
    tocEntries
        .Should()
        .Contain(
            new StoryMetadata(
                Url: new Uri("https://archiveofourown.org/works/57670891"),
                Title: "Interview with a Spaceman",
                Author: "CalliopeWayne",
                CompletionDate: new DateTime(2024, 7, 26),
                Rating: Rating.NotRated,
                LengthInBytes: 20919,
                WordCount: 3670,
                Summary: "Lois is preparing for a date with her best friend and second guessing her choice to go out with the bumbling idiot when Superman drops by for an interview."))
        .And.Contain(
            new StoryMetadata(
                Url: new Uri("https://archiveofourown.org/works/61568836"),
                Title: "Snapshot",
                Author: "90sfangirl79",
                CompletionDate: new DateTime(2024,12, 23),
                Rating: Rating.GeneralAudience,
                LengthInBytes: 5740,
                WordCount: 1007,
                Summary: "It’s Christmas in Metropolis…but which Metropolis?"))
        .And.HaveCount(8);
  }

  [Fact]
  public void GivenTestData_WhenEntryIsNotRated_ThenReturnsWithRatingNotRated ()
  {
    using var testData = GetTestDataForRating("rating-notrated");

    var storyEntries = _worksListingParser.GetStoryEntries(testData);
    storyEntries
        .Should()
        .Satisfy(storyMetadata => storyMetadata.Rating == Rating.NotRated);
  }

  [Fact]
  public void GivenTestData_WhenEntryIsGeneralAudience_ThenReturnsWithRatingGeneralAudience ()
  {
    using var testData = GetTestDataForRating("rating-general-audience");

    var storyEntries = _worksListingParser.GetStoryEntries(testData);
    storyEntries
        .Should()
        .Satisfy(storyMetadata => storyMetadata.Rating == Rating.GeneralAudience);
  }

  [Fact]
  public void GivenTestData_WhenEntryIsTeen_ThenReturnsWithRatingTeen ()
  {
    using var testData = GetTestDataForRating("rating-teen");

    var storyEntries = _worksListingParser.GetStoryEntries(testData);
    storyEntries
        .Should()
        .Satisfy(storyMetadata => storyMetadata.Rating == Rating.Teen);
  }

  [Fact]
  public void GivenTestData_WhenEntryIsMature_ThenReturnsWithRatingMature ()
  {
    using var testData = GetTestDataForRating("rating-mature");

    var storyEntries = _worksListingParser.GetStoryEntries(testData);
    storyEntries
        .Should()
        .Satisfy(storyMetadata => storyMetadata.Rating == Rating.Mature);
  }

  [Fact]
  public void GivenTestData_WhenEntryIsExplicit_ThenReturnsWithRatingExplicit ()
  {
    using var testData = GetTestDataForRating("rating-explicit");

    var storyEntries = _worksListingParser.GetStoryEntries(testData);
    storyEntries
        .Should()
        .Satisfy(storyMetadata => storyMetadata.Rating == Rating.Explicit);
  }

  private Stream GetTestDataForRating (string rating)
  {
    var htmlSnippet = $"""
        <li id="work_123456" class="work blurb group work-123456 user-98765" role="article">
          <div class="header module">
            <h4 class="heading">
              <a href="/works/123456">The Title</a>
              by
              <a rel="author" href="/users/TheUser/pseuds/TheUser">TheUser</a>
            </h4>
            <ul class="required-tags">
              <li><a><span class="{rating} rating"></span></a></li>
            </ul>
            <p class="datetime">17 Jun 2024</p>
          </div>
          <blockquote class="userstuff summary">
            <p>The Summary</p>
          </blockquote>
          <dl class="stats">
            <dt class="words">Words:</dt>
            <dd class="words">1,007</dd>
          </dl>
        </li>
        """;

    return new MemoryStream(Encoding.UTF8.GetBytes(htmlSnippet));
  }
}
