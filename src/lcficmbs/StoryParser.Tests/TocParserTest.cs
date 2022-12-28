// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System.Text;

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser.Tests;

public class TocParserTest
{
  private readonly TocParser _tocParser;

  public TocParserTest ()
  {
    _tocParser = new TocParser();
  }

  [Fact]
  public void GivenTestData_FindsMatchingEntries ()
  {
    using var testData = Resources.GetTocPage();

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Contain(
            new TocEntry(
                Url: new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/294497/new-years-season-2-by-carrie-rene-complete"),
                StoryTitle: "New Year's Season 2",
                StoryAuthor: "Carrie Rene",
                IsComplete: true))
        .And.Contain(
            new TocEntry(
                Url: new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/294083/safe-in-his-arms-by-superbek-complete"),
                StoryTitle: "Safe In His Arms",
                StoryAuthor: "SuperBek",
                IsComplete: true)
            )
        .And.HaveElementAt(
            0,
            new TocEntry(
                Url: new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/294551/ficathon-beyond-closed-doors-for-ksarasara"),
                StoryTitle: "Ficathon: Beyond Closed Doors (for Ksarasara)",
                StoryAuthor: "bakasi",
                IsComplete: false))
        .And.HaveCount(25);
  }

  [Fact]
  public void GivenTestData_WhenAuthorIsSpecifiedInTitle_AndTitleIndicatesCompletedStory_ThenReturnsTocEntry_WithStoryName_AndAuthor_AndMarkedAsCompleted ()
  {
    using var testData = GetTestDataForTocEntryWithStoryNameAndAuthor("New Year's Season 2 by Carrie Rene (Complete)");

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "New Year's Season 2" && toc.StoryAuthor == "Carrie Rene" && toc.IsComplete == true);
  }

  [Fact]
  public void GivenTestData_WhenAuthorIsSpecifiedInTitle_AndTitleIndicatesCompletedStoryWithAlternativeSpelling_ThenReturnsTocEntry_WithStoryName_AndAuthor_AndMarkedAsCompleted ()
  {
    using var testData = GetTestDataForTocEntryWithStoryNameAndAuthor("New Year's Season 2 by Carrie Rene (Completed)");

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "New Year's Season 2" && toc.StoryAuthor == "Carrie Rene" && toc.IsComplete == true);
  }

  [Fact]
  public void GivenTestData_WhenAuthorIsSpecifiedInTitle_AndTitleIndicatesCompletedStoryWithMismatchedCase_ThenReturnsTocEntry_WithStoryName_AndAuthor_AndMarkedAsCompleted ()
  {
    using var testData = GetTestDataForTocEntryWithStoryNameAndAuthor("New Year's Season 2 by Carrie Rene (cOmplete)");

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "New Year's Season 2" && toc.StoryAuthor == "Carrie Rene" && toc.IsComplete == true);
  }

  [Fact]
  public void GivenTestData_WhenAuthorIsSpecifiedInTitle_AndTitleIndicatesWorkInProcess_ThenReturnsTocEntry_WithStoryName_AndAuthor_AndMarkedAsIncomplete ()
  {
    using var testData = GetTestDataForTocEntryWithStoryNameAndAuthor("Savior by SuperBek (WIP)");

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "Savior" && toc.StoryAuthor == "SuperBek" && toc.IsComplete == false);
  }

  [Fact]
  public void GivenTestData_WhenAuthorIsSpecifiedInTitle_AndTitleIndicatesWorkInProcessWithAlternativeSpelling_ThenReturnsTocEntry_WithStoryName_AndAuthor_AndMarkedAsIncomplete ()
  {
    using var testData = GetTestDataForTocEntryWithStoryNameAndAuthor("Savior by SuperBek (Incomplete)");

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "Savior" && toc.StoryAuthor == "SuperBek" && toc.IsComplete == false);
  }

  [Fact]
  public void GivenTestData_WhenAuthorIsSpecifiedInTitle_AndTitleDoesNotIndicateCompletion_ThenReturnsTocEntry_WithStoryName_AndAuthor_AndMarkedAsIncomplete ()
  {
    using var testData = GetTestDataForTocEntryWithStoryNameAndAuthor("Savior by SuperBek");

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "Savior" && toc.StoryAuthor == "SuperBek" && toc.IsComplete == false);
  }

  [Fact]
  public void GivenTestData_WhenAuthorIsSpecifiedInTitle_AndTitleIndicatesSeriesStory_ThenReturnsTocEntry_WithStoryName_AndAuthor_AndMarkedAsSeries ()
  {
    using var testData = GetTestDataForTocEntryWithStoryNameAndAuthor("The Mission Series (series TOC) by cuidadora");

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "The Mission Series" && toc.StoryAuthor == "cuidadora" && toc.IsComplete == false && toc.IsSeries == true);
  }

  [Fact]
  public void GivenTestData_WhenAuthorIsMissingFromTitle_ThenReturnsTocEntry_WithStoryName_AndAuthorFromCreatorColumn ()
  {
    using var testData = GetTestDataForTocEntryWithStoryName("Ficathon: Beyond Closed Doors (for Ksarasara)", "bakasi");

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "Ficathon: Beyond Closed Doors (for Ksarasara)" && toc.StoryAuthor == "bakasi");
  }

  [Fact]
  public void GivenTestData_WhenEntryIsSpecifiedAsAlternatingLine_ThenReturnsTocEntry ()
  {
    using var testData = GetTestDataForAlternatingLineTocEntry();

    var tocEntries = _tocParser.GetTocEntries(testData);
    tocEntries
        .Should()
        .Satisfy(toc => toc.StoryTitle == "Story" && toc.StoryAuthor == "Author");
  }

  private Stream GetTestDataForTocEntryWithStoryNameAndAuthor (string linkText)
  {
    var htmlSnippet = $"""
        <tr>
          <td class="topicsubject  alvt">
            <div>
              <a href="/ubb/ubbthreads.php/topics/123">{linkText}</a>
            </div>
          </td>
        </tr>
        """;

    return new MemoryStream(Encoding.UTF8.GetBytes(htmlSnippet));
  }

  private Stream GetTestDataForTocEntryWithStoryName (string linkText, string creator)
  {
    var htmlSnippet = $"""
        <tr>
          <td class="topicsubject  alvt">
            <div>
              <a href="/ubb/ubbthreads.php/topics/123">{linkText}</a>
            </div>

            <div class="small">by <a href="/ubb/ubbthreads.php/users/456" rel="nofollow"><span class='userstyle'>{creator}</span></a></div>
          </td>
        </tr>
        """;

    return new MemoryStream(Encoding.UTF8.GetBytes(htmlSnippet));
  }

  private Stream GetTestDataForAlternatingLineTocEntry ()
  {
    var htmlSnippet = $"""
        <tr>
          <td class="alt-topicsubject  alvt">
            <div>
              <a href="/ubb/ubbthreads.php/topics/123">Story by Author</a>
            </div>
          </td>
        </tr>
        """;

    return new MemoryStream(Encoding.UTF8.GetBytes(htmlSnippet));
  }
}
