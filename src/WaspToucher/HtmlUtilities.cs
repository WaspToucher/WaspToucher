using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WaspToucher
{
    public static class HtmlUtilities
    {
        /// <summary>
        /// TODO: Update with balanced group constructs
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static MatchCollection GetHtmlTags(String body, String tagName)
        {
            return (Regex.Matches(body, "<\\s*?" + tagName + "((\\s*?)|(\\s+?\\w.*?))>", RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Parse single and multi-line comments from HTML.
        /// <!-- this is a comment -->
        /// <!-- this-is-a comment -->
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static MatchCollection GetHtmlComment(String body)
        {
            // avoid catastrophic backtracking
            return (Regex.Matches(body, "<!--.*?-->", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.CultureInvariant));
        }

        /// <summary>
        /// Parse single and multi-line comments from javascript
        /// //this is a comment
        /// /* this is a comment */
        /// /* this is a
        /// * comment
        /// ****/
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static MatchCollection GetJavascriptMultiLineComment(String body)
        {
            return (Regex.Matches(body, @"(/\*.*?\*/)", RegexOptions.Singleline | RegexOptions.Compiled));
        }

        public static MatchCollection GetJavascriptSingleLineComment(String body)
        {
            return (Regex.Matches(body, @"(//.*)", RegexOptions.Compiled));
        }

        public static String GetHtmlTagAttribute(String tag, String attributeName)
        {
            String attribute = null;

            // Parse out attribute field looking for values in single or double quotes
            Match m = Regex.Match(tag, attributeName + "\\s*?=\\s*?(\'|\").*?(\'|\")", RegexOptions.IgnoreCase);

            // Parse out attribute field looking for values that aren't wrapped in single or double quotes
            // TEST: Passed
            Match m1 = Regex.Match(tag, attributeName + "\\s*?=\\s*?.*?(\\s|>)", RegexOptions.IgnoreCase);

            if (m.Success)
            {
                // Parse out attribute value
                Match a = Regex.Match(m.ToString(), "(\'|\").*?(\'|\")", RegexOptions.IgnoreCase);

                if (a.Success)
                {
                    // BUGBUG: Removing UrlDecode() from here, not sure why we're doing this here.
                    // It should be up to a check to want UrlDecoded values.
                    // Otherwise + turns to a space, and other values may break.
                    //
                    // attribute = StripQuotes(HttpUtility.UrlDecode(a.ToString()));
                    attribute = Utilities.StripQuotes(a.ToString());
                }
            }
            else if (m1.Success)
            {
                // Parse out attribute value, matching to the next whitespace or closing tag
                Match a = Regex.Match(m1.ToString(), "(=).*?(\\s|>)", RegexOptions.IgnoreCase);

                if (a.Success)
                {
                    // BUGBUG: Removing UrlDecode() from here, not sure why we're doing this here.
                    // It should be up to a check to want UrlDecoded values.
                    // Otherwise + turns to a space, and other values may break.
                    //
                    // attribute = HttpUtility.UrlDecode(a.ToString());
                    attribute = a.ToString();

                    // Trim the leading = character
                    attribute = attribute.Substring(1).Trim();
                }
            }

            return attribute;
        }

        /// <summary>
        /// TODO: Update with balanced group constructs
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tagName"></param>
        /// <param name="stripEnclosingTags"></param>
        /// <returns></returns>
        public static String[] GetHtmlTagBodies(String body, String tagName, bool stripEnclosingTags)
        {
            MatchCollection mc = null;
            String[] bodies = null;
            String tmp = null;
            int x = 0;

            // Match opening->closing tag, nested tags not handled
            mc = Regex.Matches(body, @"<\s*?" + tagName + @"((\s*?)|(\s+?\w.*?))>.*?<\s*?\/\s*?" + tagName + @"\s*?>", RegexOptions.Singleline | RegexOptions.Compiled);

            if (mc != null && mc.Count > 0)
            {
                bodies = new String[mc.Count];

                foreach (Match m in mc)
                {
                    tmp = m.ToString();

                    if (stripEnclosingTags)
                    {
                        tmp = Regex.Replace(tmp, @"<\s*?" + tagName + @"((\s*?)|(\s+?\w.*?))>", "");
                        tmp = Regex.Replace(tmp, @"<\s*?\/\s*?" + tagName + @"\s*?>", "");
                    }

                    bodies[x++] = tmp;
                }
            }
            // Don't return null, return empty string array
            if (bodies == null)
            {
                bodies = new String[] { };
            }
            return bodies;
        }

        public static String[] GetHtmlTagBodies(String body, String tagName)
        {
            return (GetHtmlTagBodies(body, tagName, true));
        }

        public static String GetUriDomainName(String src)
        {
            String dom = null;

            // if uri begins with "http://" or "https://"
            if (src != null && (src.IndexOf("http://") == 0 || src.IndexOf("https://") == 0))
            {
                // get text past ://
                dom = src.Substring(src.IndexOf("://") + 3);

                // If contains "/"
                if (dom.IndexOf("/") >= 0)
                {
                    // Remove everything including "/" and after
                    dom = dom.Substring(0, dom.IndexOf("/"));
                }
            }

            return dom;
        }

        /// <summary>
        /// Checks a URL to see if it's already contained in a running list of URL's
        /// </summary>
        /// <param name="url">A full URI, must include the scheme as in http://www.nottrusted.org.  Provided by session.fullUrl.</param>
        /// <param name="urls">The List<> of URL's to maintain.</param>
        /// <returns></returns>
        public static bool UrlNotInList(String url, List<string> urls)
        {
            // We need to reset our URL List when a user clicks the
            // Clear() button.  This is done through clear button
            // event handler.
            lock (urls)
            {
                Uri uri = new Uri(url);
                url = uri.ToString();// String.Concat(uri.Host, uri.AbsolutePath);

                // URL has already been checked
                if (urls.Contains(url))
                {
                    return false;
                }

                // Host has not been checked yet
                else
                {
                    urls.Add(url);
                    return true;
                }
            }
        }
    }
}