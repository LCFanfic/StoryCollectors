// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser.Tests;

public class TocEntryParserTest
{
  private readonly TocEntryParser _tocEntryParser;


  public TocEntryParserTest ()
  {
    _tocEntryParser = new TocEntryParser();
  }

  [Fact]
  public void GivenTestData_WhenStoryHasASinglePart_ThenReturnsOneTocEntryPartForStory_AndNoComments ()
  {
    using var testData = Resources.GetTocSinglePart();

    var tocEntryEntryParts = _tocEntryParser.GetTocEntryParts(testData);

    tocEntryEntryParts
        .Should()
        .Equal(new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293771/a-lucky-strike-1-1#Post293771")));
  }

  [Fact]
  public void GivenTestData_WhenStoryHasMultipleParts_ThenReturnsAllTocEntryPartsForStory_AndNoComments ()
  {
    using var testData = Resources.GetTocMultiPart();

    var tocEntryEntryParts = _tocEntryParser.GetTocEntryParts(testData);

    tocEntryEntryParts
        .Should()
        .ContainInOrder(
            new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293464#Post293464")),
            new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293474#Post293474")),
            new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293496#Post293496")),
            new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293517#Post293517")),
            new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293542#Post293542")),
            new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293570#Post293570")),
            new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293925#Post293925")),
            new TocEntryPart(new Uri("https://www.lcficmbs.com/ubb/ubbthreads.php/topics/293946#Post293946"))
            )
        .And.HaveCount(19);
  }

  //https://www.lcficmbs.com/ubb/ubbthreads.php/topics/170794
  //https://www.lcficmbs.com/ubb/ubbthreads.php/topics/292983/so-far-away-by-ksarasara-complete#Post292983
}
