// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISubtitleDb.cs" company="">
//   
// </copyright>
// <summary>
//   The SubtitleDb interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SubSearch.Data
{
    using System;
    using System.Net;

    using SubSearch.Resources;

    /// <summary>The query result.</summary>
    [Flags]
    public enum QueryResult
    {
        /// <summary>The fatal.</summary>
        Fatal = 0,

        /// <summary>The cancelled.</summary>
        Cancelled = 1 << 1,

        /// <summary>The skipped.</summary>
        Skipped = 1 << 2, 

        /// <summary>The success.</summary>
        Success = 1 << 3,

        /// <summary>The failure.</summary>
        Failure = 1 << 4
    }

    /// <summary>The SubtitleDb interface.</summary>
    public interface ISubtitleDb
    {
        /// <summary>The file path.</summary>
        string FilePath { get; }

        /// <summary>The language.</summary>
        Language Language { get; }

        /// <summary>Gets or sets the title.</summary>
        string Title { get; set; }

        /// <summary>The download subtitle.</summary>
        /// <param name="subtitleDownloadUrl">The subtitle download url.</param>
        /// <param name="cookies">The cookies.</param>
        /// <returns>The query result.</returns>
        QueryResult DownloadSubtitle(string subtitleDownloadUrl, CookieContainer cookies = null);

        /// <summary>Queries the subtitle database.</summary>
        /// <returns>The query result.</returns>
        QueryResult Query();
    }
}