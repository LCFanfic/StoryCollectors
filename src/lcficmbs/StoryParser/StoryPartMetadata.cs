// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser;

public record StoryPartMetadata(string Title, string Author, DateTime CompletionDate, int LengthInBytes, int WordCount, Forum Forum);
