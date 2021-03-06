﻿namespace SubSearch.Data.Handlers
{
    using System.IO;
    using System.Net;

    using SharpCompress.Readers;

    /// <summary>
    /// The <see cref="SubtitleDownloader"/> class.
    /// </summary>
    public static class SubtitleDownloader
    {
        /// <summary>
        /// Downloads the subtitle from the specified <paramref name="url"/> for the <paramref name="movieFilePath"/>.
        /// </summary>
        /// <param name="movieFilePath">The movie file path.</param>
        /// <param name="url">The subtitle URL to be downloaded.</param>
        public static void DownloadSubtitle(this string movieFilePath, string url)
        {
            var targetFileWithoutExtension = Path.GetFileNameWithoutExtension(movieFilePath);
            var targetPath = Path.GetDirectoryName(movieFilePath) ?? string.Empty;
            var response = GetRequest(url).GetResponse();

            using (var respStream = response.GetResponseStream())
            {
                if (respStream == null)
                {
                    return;
                }

                using (var ms = new MemoryStream())
                {
                    respStream.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var reader = ReaderFactory.Open(ms);
                    if (reader == null)
                    {
                        var fileName = response.Headers["Content-Disposition"].Replace("attachment; filename=", string.Empty).Replace("\"", string.Empty);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var outputFile = Path.Combine(targetPath, fileName);
                            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                            {
                                ms.Seek(0, SeekOrigin.Begin);
                                ms.WriteTo(fs);
                            }
                        }
                    }
                    else
                    {
                        while (reader.MoveToNextEntry())
                        {
                            var entry = reader.Entry;
                            if (entry.IsDirectory)
                            {
                                continue;
                            }

                            var extension = Path.GetExtension(entry.Key);
                            if (string.IsNullOrEmpty(extension))
                            {
                                extension = ".srt";
                            }

                            var entryPath = targetFileWithoutExtension + extension;
                            var outputFile = Path.Combine(targetPath, entryPath);
                            reader.WriteEntryToFile(outputFile);
                        }
                    }
                }
            }
        }

        /// <summary>The get request.</summary>
        /// <param name="url">The url.</param>
        /// <param name="referrer">The referrer.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="isMobile">The is Mobile.</param>
        /// <returns>The <see cref="HttpWebRequest"/>.</returns>
        public static HttpWebRequest GetRequest(this string url, string referrer = "", CookieContainer cookies = null, bool isMobile = true)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = isMobile
                                    ? "Mozilla/5.0 (Linux; U; Android 4.2; en-us; SonyC6903 Build/14.1.G.1.518) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30"
                                    : "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; AS; rv:11.0) like Gecko";
            request.Referer = referrer;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

            if (cookies == null)
            {
                request.CookieContainer = new CookieContainer();
            }
            else
            {
                request.CookieContainer = cookies;
            }

            return request;
        }
    }
}
