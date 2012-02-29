using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Fiddler;
using Microsoft.Practices.ServiceLocation;
using WaspToucher;

public static class FiddlerSessionExtensions
{
    #region Public Method(s)

    public static String GetResponseContentType(this Session session)
    {
        if (session.oResponse.headers.Exists("content-type"))
            return (session.oResponse.headers["content-type"].ToLower());

        return (null);
    }

    public static bool IsResponseContentType(this Session session, String contentType)
    {
        string tmp = GetResponseContentType(session);
        return ((tmp != null && tmp.IndexOf(contentType) == 0) ? true : false);
    }

    public static bool IsResponseCharset(this Session session, String charset)
    {
        string tmp = GetResponseContentType(session);
        return ((tmp != null && tmp.IndexOf(charset) >= 0) ? true : false);
    }

    /// <summary>
    /// TODO: Fix up to support other variations of text/html.
    /// FIX: This will match Atom and RSS feeds now, which set text/html but use &lt;?xml&gt; in content
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static bool IsResponseHtml(this Session session)
    {
        if (session.responseBodyBytes != null)
        {
            return (IsResponseContentType(session, "text/html") || IsResponseXhtml(session));
        }
        else
        {
            return false;
        }
    }

    public static bool IsResponseXhtml(this Session session)
    {
        if (session.responseBodyBytes != null)
        {
            return (IsResponseContentType(session, "application/xhtml+xml") || IsResponseContentType(session, "application/xhtml"));
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// TODO: Fix up to support other variations of text/css
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static bool IsResponseCss(this Session session)
    {
        return (IsResponseContentType(session, "text/css"));
    }

    /// <summary>
    /// TODO: Fix up to support other variations of javascript
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static bool IsResponseJavascript(this Session session)
    {
        return (IsResponseContentType(session, "application/javascript") || IsResponseContentType(session, "application/x-javascript"));
    }

    /// <summary>
    /// TODO: Fix up to support other variations of text/xml
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static bool IsResponseXml(this Session session)
    {
        return (IsResponseContentType(session, "text/xml") || IsResponseContentType(session, "application/xml"));
    }

    public static bool IsResponsePlain(this Session session)
    {
        return (IsResponseContentType(session, "text/plain"));
    }

    /// <summary>
    /// Attempt to determine the character set used by the response document.  If the character
    /// set cannot be determined, return UTF-8 (a reasonable guess).
    /// </summary>
    /// <remarks>TODO: Extract XML/XHtml character sets?</remarks>
    /// <param name="session">The Fiddler HTTP session to examine.</param>
    /// <returns>The character set specified by the session content or a reasonable guess.</returns>
    public static String GetHtmlCharset(this Session session)
    {
        const String DefaultEncoding = "utf-8";     // Return UTF-8 if unsure, ASCII is preserved.

        // Favor the character set from the HTTP Content-Type header if it exists.
        String CharacterSet = session.oResponse.headers.GetTokenValue("Content-Type", "charset");
        if (!String.IsNullOrEmpty(CharacterSet))
        {
            // Found the character set in the header: normalize and return.
            return CharacterSet.Trim().ToLower();
        }

        // If there is no content, return the default character set.
        if (session.responseBodyBytes == null || session.requestBodyBytes.Length == 0)
        {
            Trace.TraceWarning("Warning: Response body byte-array is null, assuming default character set.");
            return DefaultEncoding;
        }

        // Otherwise, parse the document returned for character set hints.
        String ResponseBody = String.Empty;

        try
        {
            // TODO: Pretty hokey here, defaulting to 7-bit ASCII Encoding
            ResponseBody = Encoding.ASCII.GetString(session.responseBodyBytes);
        }

        catch (DecoderFallbackException e)
        {
            // Thrown if a character cannot be decoded
            Trace.TraceError("Error: DecoderFallbackException: {0}", e.Message);
            Trace.TraceWarning("Warning: Assuming default character encoding due to previous error.");
            return DefaultEncoding;
        }

        String Temp;

        // Find Meta tags specifying the content type, e.g.
        // <meta http-equiv="content-type" content="text/html; charset=utf-8"/>.

        foreach (Match m in HtmlUtilities.GetHtmlTags(ResponseBody, "meta"))
        {
            Temp = HtmlUtilities.GetHtmlTagAttribute(m.ToString(), "http-equiv");
            if (!String.IsNullOrEmpty(Temp))
            {
                if (Temp.Trim().ToLower(CultureInfo.InvariantCulture) == "content-type")
                {
                    CharacterSet = HtmlUtilities.GetHtmlTagAttribute(m.ToString(), "content");
                }
            }
        }

        // ... and return the last content type attribute if found
        // TODO: Extract the character set from the content type
        if (!String.IsNullOrEmpty(CharacterSet))
        {
            // Found the character set in the response body: normalize and return.
            return CharacterSet.Trim().ToLower();
        }

        // Return the default character set if unsure
        return DefaultEncoding;
    }

    /// <summary>
    /// This method returns the decompressed, dechunked, and normalized HTTP response body.
    /// </summary>
    /// <param name="session">The Fiddler HTTP session to examine.</param>
    /// <returns>Normalized HTTP response body.</returns>
    public static String GetResponseText(this Session session)
    {
        // Ensure the response body is available
        if (session.responseBodyBytes == null || session.responseBodyBytes.Length == 0)
        {
            Trace.TraceWarning("Warning: Response body is empty.");
            return String.Empty;
        }

        // Attempt to determine the character set used by the response document
        String CharacterSet = GetHtmlCharset(session);
        String ResponseBody = String.Empty;

        try
        {
            // Get the decoded session response.
            ResponseBody = Encoding.GetEncoding(CharacterSet).GetString(session.responseBodyBytes);
        }

        catch (DecoderFallbackException e)
        {
            // Thrown if a character cannot be decoded
            Trace.TraceError("Error: DecoderFallbackException: {0}", e.Message);
        }

        catch (ArgumentException e)
        {
            // Thrown if the GetEncoding argument is invalid
            Trace.TraceError("Error: ArgumentException: {0}", e.Message);
        }

        try
        {
            // Fallback to UTF-8 if we failed from a booty CharacterSet name.
            if (ResponseBody == String.Empty)
            {
                Trace.TraceInformation("Falling back to UTF-8 encoding.");
                ResponseBody = Encoding.UTF8.GetString(session.responseBodyBytes);
            }
        }

        catch (DecoderFallbackException e)
        {
            // Thrown if a character cannot be decoded
            Trace.TraceError("Error: DecoderFallbackException: {0}", e.Message);
        }

        return ResponseBody;
    }

    public static NameValueCollection GetRequestParameters(this Session session)
    {
        NameValueCollection nvc = null;
        String qs = null;

        // If this is GET request
        if (session.HTTPMethodIs("GET"))
        {
            // ...and has query string
            if (session.PathAndQuery.IndexOf("?") > 0)
            {
                // Get the query string
                qs = session.PathAndQuery.Substring(session.PathAndQuery.IndexOf("?") + 1);
            }
        }

        // If is a POST request
        if (session.HTTPMethodIs("POST"))
        {
            // ...and has a content-type
            if (session.oRequest.headers.Exists("content-type"))
            {
                // ... and is urlencoded form data
                if (session.oRequest.headers["content-type"] == "application/x-www-form-urlencoded")
                {
                    // TODO: is a decode needed?
                    //session.utilDecodeRequest();

                    // Get the request body as a string
                    qs = System.Text.Encoding.UTF8.GetString(session.requestBodyBytes);
                }
            }
        }

        // If we have a query string
        if (qs != null)
        {
            // Parse it...
            try
            {
                nvc = HttpUtility.ParseQueryString(qs);

                // Remove any nulls from ill-formed query strings
                List<string> lst = new List<string>();

                foreach (String param in nvc.Keys)
                {
                    if (param == null)
                    {
                        lst.Add(param);
                    }
                }

                foreach (String param in lst)
                {
                    nvc.Remove(param);
                }
            }

            // TODO: Could we be missing things here?  False negatives?
            catch (ArgumentNullException ane)
            {
                ServiceLocator.Current.GetInstance<ILogger>().Error(ane);
            }
        }

        return (nvc);
    }

    #endregion Public Method(s)
}