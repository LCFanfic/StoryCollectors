﻿// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System.Globalization;
using System.Net.Mime;
using System.Text;
using HtmlAgilityPack;

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser;

public class StoryPartParser
{
  public StoryPartMetadata GetStoryPartMetadata (Stream html)
  {
    var doc = new HtmlDocument();
    doc.Load(html);

    var subjectBlock = doc.DocumentNode.SelectNodes(".//td[@class='subjecttable']/div").First();
    var authorBlock = doc.DocumentNode.SelectNodes(".//td[@class='author-content alvt mblwide']").First();
    var breadcrumbs = doc.DocumentNode.SelectNodes(".//td[@id='breadcrumbs']//a");

    var storyNode = doc.DocumentNode.SelectSingleNode(".//div[@id='body0']");
    foreach (HtmlNode brNode in storyNode.SelectNodes("//br"))
      brNode.ParentNode.ReplaceChild(doc.CreateTextNode("\n"), brNode);
    var story = storyNode.InnerText;

    var author = authorBlock.SelectSingleNode("./div[@class='bold author-name fwrap dblock']//span").InnerText.Trim();
    var title = subjectBlock.SelectSingleNode("./div[@class='truncate bold']").GetAttributeValue("title", "<missing>");
    var dateString = subjectBlock.SelectSingleNode("./div[@class='iblock']//span[@class='date']").InnerText.Trim();
    var timeString = subjectBlock.SelectSingleNode("./div[@class='iblock']//span[@class='time']").InnerText.Trim();
    var date = DateTime.ParseExact(dateString + " " + timeString, "MM/dd/yy hh:mm tt", CultureInfo.InvariantCulture);
    var forum = GetForum(breadcrumbs);

    var length = story.Length;
    var words = GetWordCount(story);

    string? summary = GetSummary(story);

    return new StoryPartMetadata(Author: author, Title: title, CompletionDate: date, LengthInBytes: length, WordCount: words, Forum: forum, Summary: summary);
  }

  private Forum GetForum (HtmlNodeCollection breadcrumbs)
  {
    var hrefs = breadcrumbs.Select(a => a.GetAttributeValue("href", null)).ToArray();

    if (hrefs.Contains("/ubb/ubbthreads.php/forums/1/1/lois-clark-fanfic", StringComparer.OrdinalIgnoreCase))
      return Forum.Fanfic;
    else if (hrefs.Contains("/ubb/ubbthreads.php/forums/2/1/lois-clark-nfanfic", StringComparer.OrdinalIgnoreCase))
      return Forum.NFanfic;
    else if (hrefs.Contains("/ubb/ubbthreads.php/forums/3/1/fanfic-challenge", StringComparer.OrdinalIgnoreCase))
      return Forum.FanficChallenge;
    else
      return Forum.Other;
  }

  private int GetWordCount (string story)
  {
    var wordCount = 0;
    var lastChar = ' ';

    foreach (var currentChar in story)
    {
      if (Char.IsWhiteSpace(lastChar) && !Char.IsWhiteSpace(currentChar))
        wordCount++;

      lastChar = currentChar;
    }

    return wordCount;
  }

  private string? GetSummary (string story)
  {
    var nonEmptyLinesBeforeSummary = 0;
    const int maxNonEmptyLinesBeforeSummary = 20;
    var storyLinesEnumerator = story.AsSpan().EnumerateLines();
    while (storyLinesEnumerator.MoveNext())
    {
      var line = storyLinesEnumerator.Current.Trim();

      if (line.StartsWith("Summary:") || line.SequenceEqual("Summary"))
      {
        var remainingLine = line.Slice("Summary".Length).TrimStart(new[] { ' ', ':' }).TrimEnd();
        if (remainingLine.Length > 0)
          return remainingLine.ToString();

        while (storyLinesEnumerator.MoveNext() && storyLinesEnumerator.Current.Trim().IsEmpty)
        {
          // skip whitespace and blank lines
        }

        var nextLine = storyLinesEnumerator.Current.Trim();
        return nextLine.ToString();
      }
      else if (!line.Trim().IsEmpty)
      {
        nonEmptyLinesBeforeSummary++;
        if (nonEmptyLinesBeforeSummary == maxNonEmptyLinesBeforeSummary)
          return null;
      }
    }

    return null;
  }
}
