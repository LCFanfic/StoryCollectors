// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System.Text;

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser.Tests;

public class StoryPartParserTest
{
  private readonly StoryPartParser _storyPartParser;

  public StoryPartParserTest ()
  {
    _storyPartParser = new StoryPartParser();
  }


  [Fact]
  public void GivenTestData_WhenStoryHasASinglePart_ThenReturnsOneStoryPart ()
  {
    using var testData = Resources.GetStoryPart();

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData
        .Should()
        .Be(
            new StoryPartMetadata(
                Author: "KSaraSara",
                Title: "A Lucky Strike 1/1",
                CompletionDate: new DateTime(2022, 11, 18, 21, 59, 00),
                LengthInBytes: 6710,
                WordCount: 1118,
                Forum: Forum.Fanfic,
                Summary: "As the events in Bowled Over continue, Lois' luck just keeps improving...enough that even she starts to wonder how she's playing so well."));
  }

  [Fact]
  public void GivenTestData_WhenStoryStartsWithSummaryTagAndColonInSameLineAsSummary_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
        Summary: this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStoryStartsWithSummaryTagAndNoColonInSameLineAsSummary_ThenReturnsStoryPartWithoutSummary ()
  {
    var body = $"""
        Summary this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be(null);
  }

  [Fact]
  public void GivenTestData_WhenStoryStartsWithSummaryAndColonAndHasWhitespace_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
          Summary:  this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStoryStartsWithSummaryTagAndColonInSeparateLinesFromSummary_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
        Summary:
        <br/>
        this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStoryStartsWithSummaryTagAndColonHeadingAndExtraWhitespace_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
        Summary:
        <br/>
        <br/>
         this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStoryStartsWithSummaryTagAndNoColonInSeparateLinesFromSummary_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
        Summary
        <br/>
        this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStoryStartsWithSummaryTagAndNoColonAndExtraWhitespaceInSeparateLinesFromSummary_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
        Summary
        <br/>
        this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStoryStartsWithSomeText_AndHasSummary_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
        This is some text
        <br/>
        By KSaraSara
        <br/>
        Author’s Note: this is the author notes...
        <br/>
        Summary: this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        <br/>
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStoryContainsNoASCIINewline_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
        Summary:<br/>this is the summary...<br/>this is the content<br/>some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStoryContainsNonClosedBRTags_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
        Summary:<br>this is the summary...<br>this is the content<br>some more content
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStory_AndHasSummaryWithinFirstTwentyLines_ThenReturnsStoryPartWithSummary ()
  {
    var body = $"""
         1 This is some text<br/>
         2 This is some text<br/>
         3 This is some text<br/>
         4 This is some text<br/>
         5 This is some text<br/>
         6 This is some text<br/>
         7 This is some text<br/>
         8 This is some text<br/>
         9 This is some text<br/>
        10 This is some text<br/>
        <br/>
        11 This is some text<br/>
        12 This is some text<br/>
        13 This is some text<br/>
        14 This is some text<br/>
        15 This is some text<br/>
        16 This is some text<br/>
        17 This is some text<br/>
        18 This is some text<br/>
        19 This is some text<br/>
        Summary:
        <br/>
        this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        <br/>
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be("this is the summary...");
  }

  [Fact]
  public void GivenTestData_WhenStory_AndHasSummaryNotWithinFirstTwentyLines_ThenReturnsStoryPartWithoutSummary ()
  {
    var body = $"""
         1 This is some text<br/>
         2 This is some text<br/>
         3 This is some text<br/>
         4 This is some text<br/>
         5 This is some text<br/>
         6 This is some text<br/>
         7 This is some text<br/>
         8 This is some text<br/>
         9 This is some text<br/>
        10 This is some text<br/>
        11 This is some text<br/>
        12 This is some text<br/>
        13 This is some text<br/>
        14 This is some text<br/>
        15 This is some text<br/>
        16 This is some text<br/>
        17 This is some text<br/>
        18 This is some text<br/>
        19 This is some text<br/>
        20 This is some text<br/>
        Summary: this is the summary...
        <br/>
        this is the content
        <br/>
        some more content
        <br/>
        """;

    using var testData = GetTestDataWithCustomBody(body);

    var storyPartMetaData = _storyPartParser.GetStoryPartMetadata(testData);

    storyPartMetaData.Summary.Should().Be(null);
  }

  private Stream GetTestDataWithCustomBody (string body)
  {
    var htmlSnippet = $"""
        <table class="t_inner">
        <tr>
        <td id="breadcrumbs" class="breadcrumbs">
        <a href="/ubb/ubbthreads.php/category/2/fan-fiction">Fan Fiction</a>
        </td>
        </tr>

        <tr>
        <td class="subjecttable" colspan="2">
        <div class="fblock" style="align-items:center;">
          <div class="truncate bold" title="A Lucky Strike 1/1"><a href="/ubb/ubbthreads.php/topics/293771/a-lucky-strike-1-1#Post293771" class="nd" rel="nofollow">A Lucky Strike 1/1</a></div>
          <div class="iblock">
            <span class="date">11/18/22</span> <span class="time">09:59 PM</span>
          </div>
        </div>
        </td>
        </tr>

        <tr>
        <td class="author-content alvt mblwide" colspan="2">
        <div class="bold author-name fwrap dblock">
        <a href="/ubb/ubbthreads.php/users/490/ksarasara"><span class='adminname'>KSaraSara</span></a>
        <span class="lmar rmar nw">
        <img src="/ubb/images/moods/default/offline.gif" alt="Offline" title="Offline">
        <span class="post-op">OP</span></span>
        </div>
        </td>
        </tr>

        <tr>
        <td class="post-content alvt">
        <div class="post_inner">
        <div id="body0">
        {body}
        </div>

        </td>
        </tr>
        </table>
        """;

    return new MemoryStream(Encoding.UTF8.GetBytes(htmlSnippet));
  }
}
