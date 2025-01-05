// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System;
using System.Diagnostics;
using System.Globalization;
using HtmlAgilityPack;

namespace LCFanfic.StoryCollectors.ArchiveOfOurOwn.StoryParser;

public class WorksListingParser
{
  public StoryMetadata[] GetStoryEntries (Stream html)
  {
    var doc = new HtmlDocument();
    doc.Load(html);

    var storyNodes = doc.DocumentNode.SelectNodes(".//li[@role='article']") ?? Enumerable.Empty<HtmlNode>();

    return storyNodes.Select(ParseStoryNode).ToArray();
  }

   private StoryMetadata ParseStoryNode (HtmlNode node)
  {
    Trace.Assert(node.NodeType == HtmlNodeType.Element, $"Expected node.NodeType == HtmlNodeType.Element but was '{node.NodeType}'");
    Trace.Assert(node.Name == "li", $"Expected node.Name == 'li' but was '{node.Name}'");

    var storyLinkNode = node.SelectSingleNode(".//h4[@class='heading']/a[starts-with(@href, '/works/')]");//
    Trace.Assert(storyLinkNode != null, "storyLinkNode != null");

    var linkUrlBuilder = new UriBuilder("https", "archiveofourown.org");
    var postLinkHref = storyLinkNode.GetAttributeValue("href", "");
    linkUrlBuilder.Path = postLinkHref;
    var storyUrl = linkUrlBuilder.Uri;
    var storyTitle = storyLinkNode.InnerText;

    var storyAuthorNode = node.SelectSingleNode(".//h4[@class='heading']/a[@rel='author']");
    Trace.Assert(storyAuthorNode != null, "storyAuthorNode != null");
    var storyAuthor = storyAuthorNode.InnerText;

    var storyDateNode =  node.SelectSingleNode(".//p[@class='datetime']");
    Trace.Assert(storyDateNode != null, "storyDateNode != null");
    DateTime storyDate = DateTime.ParseExact(storyDateNode.InnerText, "dd MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

    var storyWordCountNode =  node.SelectSingleNode(".//dd[@class='words']");
    Trace.Assert(storyWordCountNode != null, "storyWordCountNode != null");
    var storyWordCount = Int32.Parse(storyWordCountNode.InnerText, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
    var storyLengthInBytes = (int) Math.Round(storyWordCount * 5.7, 0);

    var storySummaryNode = node.SelectSingleNode(".//blockquote[@class='userstuff summary']/p");
    Trace.Assert(storySummaryNode != null, "storySummaryNode != null");
    var storySummary = storySummaryNode.InnerText;

    var storyRatingNode = node.SelectSingleNode(".//ul[@class='required-tags']//span[contains(@class, 'rating')]");
    Trace.Assert(storyRatingNode != null, "storyRatingNode != null");
    var storyRatingText = storyRatingNode.GetClasses().SingleOrDefault(value => value.StartsWith("rating-"));
    var storyRating = storyRatingText switch
    {
        "rating-notrated" => Rating.NotRated,
        "rating-general-audience" => Rating.GeneralAudience,
        "rating-teen" => Rating.Teen,
        "rating-mature" => Rating.Mature,
        "rating-explicit" => Rating.Explicit,
        _ => throw new ArgumentOutOfRangeException($"Story rating not supported: {storyRatingText}")
    };

    return new StoryMetadata(
        Url: storyUrl,
        Rating: storyRating,
        Title: storyTitle,
        Author: storyAuthor,
        CompletionDate: storyDate,
        LengthInBytes: storyLengthInBytes,
        WordCount: storyWordCount,
        Summary: storySummary);
  }
}
