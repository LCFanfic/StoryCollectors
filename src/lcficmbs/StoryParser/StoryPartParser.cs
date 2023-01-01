// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System.Globalization;
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
    var story = doc.DocumentNode.SelectSingleNode(".//div[@id='body0']").InnerText;

    var author = authorBlock.SelectSingleNode("./div[@class='bold author-name fwrap dblock']//span").InnerText.Trim();
    var title = subjectBlock.SelectSingleNode("./div[@class='truncate bold']").GetAttributeValue("title", "<missing>");
    var dateString = subjectBlock.SelectSingleNode("./div[@class='iblock']//span[@class='date']").InnerText.Trim();
    var timeString = subjectBlock.SelectSingleNode("./div[@class='iblock']//span[@class='time']").InnerText.Trim();
    var date = DateTime.ParseExact(dateString + " " + timeString, "MM/dd/yy hh:mm tt", CultureInfo.InvariantCulture);
    var forum = GetForum(breadcrumbs);

    var length = story.Length;
    var words = length / 6;

    return new StoryPartMetadata(Author: author, Title: title, CompletionDate: date, LengthInBytes: length, WordCount: words, Forum: forum);
  }


  private Forum GetForum (HtmlNodeCollection breadcrumbs)
  {
    var hrefs = breadcrumbs.Select(a => a.GetAttributeValue("href", null)).ToArray();

    if (hrefs.Contains("/ubb/ubbthreads.php/forums/1/1/lois-and-clark-fanfic", StringComparer.OrdinalIgnoreCase))
      return Forum.Fanfic;
    else if (hrefs.Contains("/ubb/ubbthreads.php/forums/2/1/lois-and-clark-nfanfic", StringComparer.OrdinalIgnoreCase))
      return Forum.NFanfic;
    else if (hrefs.Contains("/ubb/ubbthreads.php/forums/3/1/fanfic-challenge", StringComparer.OrdinalIgnoreCase))
      return Forum.FanficChallenge;
    else
      return Forum.Other;
  }
}
