// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System;

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser;

public record StoryMetadata(Uri Toc, Rating Rating, string Title, string Author, DateTime? CompletionDate, int? LengthInBytes, int? WordCount);
