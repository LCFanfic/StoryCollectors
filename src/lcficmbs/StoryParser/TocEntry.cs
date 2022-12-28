// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser;

public record TocEntry(Uri Url, string StoryTitle, string StoryAuthor, bool IsComplete)
{
  public bool IsSeries { get; init; }
}
