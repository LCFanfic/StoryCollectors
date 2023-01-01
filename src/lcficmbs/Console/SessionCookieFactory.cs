// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: COPYRIGHT Lois & Clark Fanfiction Tooling

using System.Net;
using Microsoft.Playwright;
using Cookie = System.Net.Cookie;

namespace LCFanfic.StoryCollectors.lcficmbs.Console;

public class SessionCookieFactory
{
  public SessionCookieFactory ()
  {
  }

  public CookieContainer GetSessionCookiesAsync (string username, string password)
  {
    var browserCookies = GetCookiesFromBrowserLogin(username, password);
    browserCookies.Wait();

    var cookieContainer = new CookieContainer();
    foreach (var cookie in browserCookies.Result)
      cookieContainer.Add(ConvertBrowserCookie(cookie));

    return cookieContainer;
  }

  private static async Task<IReadOnlyList<BrowserContextCookiesResult>> GetCookiesFromBrowserLogin (string username, string password)
  {
    using var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
    var loginPage = await browser.NewPageAsync();
    await loginPage.GotoAsync("https://www.lcficmbs.com/ubb/ubbthreads.php/ubb/login");
    await loginPage.Locator("input[name=Loginname]").First.FillAsync(username);
    await loginPage.Locator("input[name=Loginpass]").First.FillAsync(password);
    await loginPage.ClickAsync("input[name=buttlogin]");

    return await loginPage.Context.CookiesAsync();
  }

  private static Cookie ConvertBrowserCookie (BrowserContextCookiesResult cookie)
  {
    return new Cookie(name: cookie.Name, value: cookie.Value, path: cookie.Path, domain: cookie.Domain)
           {
               Expires = cookie.Expires < 0 ? DateTime.MinValue : new DateTime(ticks: (long)cookie.Expires),
               HttpOnly = cookie.HttpOnly,
           };
  }
}
