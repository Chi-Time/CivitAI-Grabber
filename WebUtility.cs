// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace CivitAI_Grabber
{
    /// <summary>Utilities for performing web requests and formatting with the CivitAI API.</summary>
    public static class WebUtility
    {
        private static readonly HttpClient _Client = new ()
        {
            Timeout = Timeout.InfiniteTimeSpan
        };
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger ();

        /// <summary>Download a file from the given url to the filePath provided.</summary>
        /// <param name="url">The url to download data from.</param>
        /// <param name="filePath">The filepath to save the downloaded data to.</param>
        /// <returns><see langword="true"/> if download operation successful. <see langword="false"/> if failed.</returns>
        public static bool DownloadFile (string url, string filePath)
        {
            //TODO: Switch to async and perform progress logging for user feedback.
            try
            {
                using HttpResponseMessage response = GetRedirectResponse (new Uri (url));
                if (response.IsSuccessStatusCode == false)
                {
                    _Logger.Warn ($"Request responded with non-success status code {response.StatusCode}");
                    return false;
                }

                //TODO: Move to async to download file in chunks and avoid memory issues.
                using Stream stream = response.Content.ReadAsStream ();
                using FileStream fs = new (filePath, FileMode.OpenOrCreate);
                stream.Seek (0, SeekOrigin.Begin);
                stream.CopyTo (fs);

                return true;
            }
            catch (Exception e)
            {
                _Logger.Error (e, $"Failed to download file: {filePath} from url: {url}");
                return false;
            }
        }

        /// <summary>GET a string response from the provided url asynchronously.</summary>
        /// <param name="url">The url to GET request a response from.</param>
        /// <returns>A <see cref="string"/> response from the url if received. An empty <see cref="string"/> if failed.</returns>
        public static async Task<string> GetResponseAsync (string url)
        {
            string text = "";

            try
            {
                using HttpResponseMessage response = await GetRedirectResponseAsync (new Uri (url));
                if (response.IsSuccessStatusCode == false)
                {
                    _Logger.Warn ($"Request responded with non-success status code {response.StatusCode}");
                    return "";
                }

                using Stream stream = await response.Content.ReadAsStreamAsync ();
                using StreamReader streamReader = new (stream);
                text = await streamReader.ReadToEndAsync ();
            }
            catch (Exception e)
            {
                _Logger.Error (e, $"Could not retrieve response from url: {url}");
                return "";
            }

            return text;
        }

        /// <summary>GET's a Http response from the given url asynchronously following redirects if any are found.</summary>
        /// <param name="url">The URL to request a GET response from.</param>
        /// <returns>The Http response from the url and any redirects.</returns>
        private static async Task<HttpResponseMessage> GetRedirectResponseAsync (Uri url)
        {
            HttpResponseMessage response = await _Client.GetAsync (url);

            // Currently uses recursion which could exhaust stack.
            if (IsRedirect (response.StatusCode) && response.Headers.Location != null)
            {
                // Dispose of response as we're about to generate a new request.
                ( (IDisposable)response ).Dispose ();
                return GetRedirectResponse (response.Headers.Location);
            }

            return response;
        }

        /// <summary>GET a string response from the provided url.</summary>
        /// <param name="url">The url to GET request a response from.</param>
        /// <returns>A <see cref="string"/> response from the url if received. An empty <see cref="string"/> if failed.</returns>
        public static string GetResponse (string url)
        {
            string text = "";

            try
            {
                using HttpResponseMessage response = GetRedirectResponse (new Uri (url));
                if (response.IsSuccessStatusCode == false)
                {
                    _Logger.Warn ($"Request responded with non-success status code {response.StatusCode}");
                    return "";
                }

                using Stream stream = response.Content.ReadAsStream ();
                using StreamReader streamReader = new (stream);
                text = streamReader.ReadToEnd ();
            }
            catch (Exception e)
            {
                _Logger.Error (e, $"Could not retrieve response from url: {url}");
                return "";
            }

            return text;
        }

        /// <summary>GET's a Http response from the given url following redirects if any are found.</summary>
        /// <param name="url">The URL to request a GET response from.</param>
        /// <returns>The Http response from the url and any redirects.</returns>
        private static HttpResponseMessage GetRedirectResponse (Uri url)
        {
            using HttpRequestMessage request = new (HttpMethod.Get, url);
            HttpResponseMessage response = _Client.Send (request);

            // Currently uses recursion which could exhaust stack.
            if (IsRedirect (response.StatusCode) && response.Headers.Location != null)
            {
                // Dispose of response as we're about to generate a new request.
                ((IDisposable)response).Dispose ();
                return GetRedirectResponse (response.Headers.Location);
            }

            return response;
        }

        /// <summary>Determine if the status code is a redirect response.</summary>
        /// <param name="statusCode">The status code to check.</param>
        /// <returns><see langword="true"/> if the status code is a redirect response. <see langword="false"/> if not.</returns>
        public static bool IsRedirect (HttpStatusCode statusCode)
        {
            return statusCode >= HttpStatusCode.Moved
                && statusCode <= HttpStatusCode.PermanentRedirect;
        }

        /// <summary>GET a formatted json string response from the provided url.</summary>
        /// <param name="url">The url to GET request a response from.</param>
        /// <returns>A formatted json <see cref="string"/> if received. An empty <see cref="string"/> if failed.</returns>
        public static string GetJSON (string url)
        {
            string response = GetResponse (url);
            if (string.IsNullOrWhiteSpace (response))
                return "";

            if (IsValidJSON (response))
            {
                // Parse and serialise json to make it human readable.
                var doc = JsonDocument.Parse (response);
                return JsonSerializer.Serialize (doc, new JsonSerializerOptions () { WriteIndented = true });
            }

            return "";
        }

        /// <summary>Determines if a given JSON string is valid and parseable.</summary>
        /// <param name="json">The JSON string to check.</param>
        /// <returns><see langword="true"/> if successful. <see langword="false"/> if failed.</returns>
        public static bool IsValidJSON (string json)
        {
            try
            {
                JsonNode.Parse (json);
                return true;
            }
            catch (Exception e)
            {
                _Logger.Warn ($"JSON string is invalid.");
                return false;
            }
        }

        /// <summary>Parse a basic HTML string using <see cref="Regex"/> to remove HTML formatting.</summary>
        /// <param name="html">The HTML string to parse.</param>
        /// <returns>A <see cref="Regex"/> parsed string with HTML formatting removed.</returns>
        public static string RemoveHTML (string html)
        {
            // Method Referenced from
            // Source: StackOverflow
            // Author: Ben Anderson
            // https://stackoverflow.com/a/16407272

            //matches one or more (white space or line breaks) between '>' and '<'
            const string whitespacePattern = @"(>|$)(\W|\n|\r)+<";
            //matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            const string lineBreakPattern = @"<(br|BR)\s{0,1}\/{0,1}>";
            //match any character between '<' and '>', even when end tag is missing
            const string tagPattern = @"<[^>]*(>|$)";

            string content = html;
            //Decode html specific characters
            content = System.Net.WebUtility.HtmlDecode (content);
            //Remove tag whitespace/line breaks
            content = Regex.Replace (content, whitespacePattern, "><", RegexOptions.Multiline);
            //Replace <br /> with line breaks
            content = Regex.Replace (content, lineBreakPattern, Environment.NewLine, RegexOptions.Multiline);
            //Strip tags
            content = Regex.Replace (content, tagPattern, string.Empty, RegexOptions.Multiline);

            return content;
        }
    }
}
