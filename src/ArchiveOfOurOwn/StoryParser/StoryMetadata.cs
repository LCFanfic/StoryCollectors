// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System;

namespace LCFanfic.StoryCollectors.ArchiveOfOurOwn.StoryParser;

public record StoryMetadata(Uri Url, Rating Rating, string Title, string Author, DateTime? CompletionDate, int? LengthInBytes, int? WordCount, string? Summary);
