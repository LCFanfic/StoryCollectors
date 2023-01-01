// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser.Tests;

public class StoryPartParserTest
{
  private readonly StoryPartParser _storyPartParser;

  public StoryPartParserTest ()
  {
    _storyPartParser = new StoryPartParser();
  }

  [Fact]
  public void GivenTestData_WhenStoryHasASinglePart_ThenReturnsOneTocEntryPartForStory_AndNoComments ()
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
                LengthInBytes: 6641,
                WordCount: 1106,
                Forum: Forum.Fanfic));
  }
}
