using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Net;
using System.Web;

public class HTTP
{

    /* get HTTP response status code */
    public HttpStatusCode GetHttpStatusCode(string url)
    {
        WebRequest request;
        HttpStatusCode statusCode;
        
        request = HttpWebRequest.Create(url);
        request.Method = "HEAD";

        try
        {
            var response = (HttpWebResponse)request.GetResponse();
            statusCode = response.StatusCode;
        }

        /* in case of 4xx or 5xx /*/
        catch (WebException e)
        {
            statusCode = ((HttpWebResponse)e.Response).StatusCode;
        }

        return statusCode;
    }
    

    public string getWebpage(string url)
    {
        WebRequest request = WebRequest.Create(url);
        WebResponse response = request.GetResponse();
        Stream data = response.GetResponseStream();
        string html = String.Empty;
        using (StreamReader sr = new StreamReader(data))
        {
            html = sr.ReadToEnd();
        }

        return html;
    }

    /* checks if URL (HTTP/HTTPS) is valid */
    public bool ValidateURL(string url)
    {
        Uri uriResult;
        
        return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public NameValueCollection GetUrlParameters(string url)
    {
        Uri uri = new Uri(url);
        NameValueCollection parameterCollection;

        parameterCollection = HttpUtility.ParseQueryString(uri.Query);

        /* if there are no parameters return null */
        if (!url.Contains("?"))
        {
            return null;
        }
 
        return parameterCollection;
    }

    /* convert relative URL to absolute */
    public string GetAbsoluteUrlString(string baseUrl, string url)
    {
        var uri = new Uri(url, UriKind.RelativeOrAbsolute);
        if (!uri.IsAbsoluteUri)
            uri = new Uri(new Uri(baseUrl), uri);
        return uri.ToString();
    }

    public string GetAbsoluteUrlStringNoParam(string baseUrl, string url)
    {
        return GetAbsoluteUrlString(baseUrl, url).Split("?")[0];
    }
}
