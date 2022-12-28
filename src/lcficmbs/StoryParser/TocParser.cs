// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System.Diagnostics;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LCFanfic.StoryCollectors.lcficmbs.StoryParser;

public class TocParser
{
  private static Regex s_titleTagRegex = new(
      @"^(?<beforeTag>.*)(?:\((?<tag>Complete|Completed|Incomplete|WIP|Series|Series\sTOC)\))(?<afterTag>.*)$",
      RegexOptions.IgnoreCase | RegexOptions.Compiled);

  private static Regex s_titleAuthorRegex = new(@"^(?<title>.*)(?:\sby\s)(?<author>.*)$", RegexOptions.Compiled);


  public TocParser ()
  {
  }

  public TocEntry[] GetTocEntries (Stream html)
  {
    var doc = new HtmlDocument();
    doc.Load(html);

    var tocLinkNodes = doc.DocumentNode.SelectNodes(".//td[@class='topicsubject  alvt' or @class='alt-topicsubject  alvt']");

    return tocLinkNodes.Select(ParseTocEntry).ToArray();
  }

  private TocEntry ParseTocEntry (HtmlNode node)
  {
    Trace.Assert(node.NodeType == HtmlNodeType.Element, $"Expected node.NodeType == HtmlNodeType.Element but was '{node.NodeType}'");
    Trace.Assert(node.Name == "td", $"Expected node.Name == 'td' but was '{node.Name}'");

    var postLinkNode = node.Descendants("a").FirstOrDefault(a => a.GetAttributeValue("href", "").StartsWith("/ubb/ubbthreads.php/topics/"));
    Trace.Assert(postLinkNode != null, "postLinkNode != null");

    var linkUrlBuilder = new UriBuilder("https", "www.lcficmbs.com");
    var postLinkHref = postLinkNode!.GetAttributeValue("href", "");
    linkUrlBuilder.Path = postLinkHref.Split("#", count: 2)[0];
    var linkUrl = linkUrlBuilder.Uri;

    var linkText = postLinkNode.InnerText;
    var titleTagMatchResult = s_titleTagRegex.Match(linkText);

    string remainingTitleText;
    bool isComplete;
    bool isSeries;
    if (titleTagMatchResult.Success)
    {
      remainingTitleText = titleTagMatchResult.Groups["beforeTag"].Value + titleTagMatchResult.Groups["afterTag"].Value;
      var titleTag = titleTagMatchResult.Groups["tag"].Value;
      isComplete = titleTag.StartsWith("Complete", StringComparison.OrdinalIgnoreCase);
      isSeries = titleTag.StartsWith("Series", StringComparison.OrdinalIgnoreCase);
    }
    else
    {
      remainingTitleText = linkText;
      isComplete = false;
      isSeries = false;
    }

    var titleAuthorMatchResult = s_titleAuthorRegex.Match(remainingTitleText);

    string storyTitle;
    string storyAuthor;
    if (titleAuthorMatchResult.Success)
    {
      storyTitle = titleAuthorMatchResult.Groups["title"].Value.Trim();
      storyAuthor = titleAuthorMatchResult.Groups["author"].Value.Trim();
    }
    else
    {
      var postCreatorNode = node.Descendants("a").FirstOrDefault(a => a.GetAttributeValue("href", "").StartsWith("/ubb/ubbthreads.php/users/"));
      Trace.Assert(postCreatorNode != null, "postCreatorNode != null");

      storyTitle = remainingTitleText.Trim();
      storyAuthor = postCreatorNode!.InnerText;
    }

    return new TocEntry(Url: linkUrl, StoryTitle: storyTitle, StoryAuthor: storyAuthor, IsComplete: isComplete) with { IsSeries = isSeries };
  }
}
