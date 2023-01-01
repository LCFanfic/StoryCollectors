// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using HtmlAgilityPack;

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser;

public class TocEntryParser
{
  public TocEntryParser ()
  {
  }


  public TocEntryPart[] GetTocEntryParts (Stream html)
  {
    var doc = new HtmlDocument();
    doc.Load(html);

    var tocNodes = doc.DocumentNode.SelectNodes(".//div[@class='post_inner']/div[@id]//a");

    return tocNodes
        .Select(n => (href: new Uri(n.GetAttributeValue("href", null)), text: n.InnerText))
        .Where(link => !IsCommentThread(link.text))
        .Where(link => IsFanficBoard(link.href))
        .Where(link => IsPostThread(link.text))
        .Select(link => new TocEntryPart(link.href))
        .ToArray();
  }

  private bool IsCommentThread (string linkText)
  {
    var trimmedLinkText = linkText.Trim();

    if (trimmedLinkText.StartsWith("Comments", StringComparison.OrdinalIgnoreCase))
      return true;
    else if (trimmedLinkText.StartsWith("Comments", StringComparison.OrdinalIgnoreCase))
      return true;
    else if (trimmedLinkText.EndsWith("Comments", StringComparison.OrdinalIgnoreCase))
      return true;
    else if (trimmedLinkText.StartsWith("FDK", StringComparison.OrdinalIgnoreCase))
      return true;
    else if (trimmedLinkText.EndsWith("FDK", StringComparison.OrdinalIgnoreCase))
      return true;
    else if (trimmedLinkText.StartsWith("Feedback", StringComparison.OrdinalIgnoreCase))
      return true;
    else if (trimmedLinkText.EndsWith("Feedback", StringComparison.OrdinalIgnoreCase))
      return true;
    else
      return false;
  }

  private bool IsPostThread (string linkText)
  {
    return true;
  }

  private bool IsFanficBoard (Uri uri)
  {
    if (uri.Host.Equals("www.lcficmbs.com"))
      return true;
    else
      return false;
  }

}
