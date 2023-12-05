// <copyright file="JeDeclareOptions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http
{
    /// <summary>
    /// Options to use the <see cref="IJeDeclareClient"/>.
    /// </summary>
    public class JeDeclareOptions
    {
        /// <summary>
        /// The '/' delimiter.
        /// </summary>
        private const string UriSlash = "/";

        /// <summary>
        /// The endpoint URL to the <c>JeDeclare API</c>.
        /// </summary>
        private Uri? baseUri;

        /// <summary>
        /// Gets or sets the URL under which <c>JeDeclare Service</c> is deployed.
        /// </summary>
        public Uri? BaseUri
        {
            get => this.baseUri;
            set
            {
                if (value != null)
                {
                    this.baseUri = AppendTrailingSlashIfNeed(value);
                }
                else
                {
                    this.baseUri = value;
                }
            }
        }

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string JdcCompteId { get; set; }

        public string HistoryDateEnabledBanks { get; set; }

        /// <summary>
        /// Check if the object is filled with valid properties.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BaseUri"/> is null.</exception>
        public void Validate()
        {
            if (this.BaseUri is null)
            {
                throw new InvalidOperationException($"Instance of {nameof(JeDeclareOptions)} is invalid, {nameof(JeDeclareOptions.BaseUri)} is null");
            }

            if (string.IsNullOrEmpty(this.Login))
            {
                throw new InvalidOperationException($"Instance of {nameof(JeDeclareOptions)} is invalid, {nameof(JeDeclareOptions.Login)} is null");
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                throw new InvalidOperationException($"Instance of {nameof(JeDeclareOptions)} is invalid, {nameof(JeDeclareOptions.Password)} is null");
            }

            if (string.IsNullOrEmpty(this.JdcCompteId))
            {
                throw new InvalidOperationException($"Instance of {nameof(JeDeclareOptions)} is invalid, {nameof(JeDeclareOptions.JdcCompteId)} is null");
            }
        }

        /// <summary>
        /// Appends a trailing slash at the end of the <paramref name="uri"/> if need.
        /// Needed for http client url building.
        /// </summary>
        /// <param name="uri"><see cref="Uri"/> which the trailing slash have to be added if need.</param>
        /// <returns>The <paramref name="uri"/> with a trailing slash at the end if not exists.</returns>
        private static Uri AppendTrailingSlashIfNeed(Uri uri)
        {
            var uriString = uri.ToString();

            if (!uriString.EndsWith(UriSlash))
            {
                return new Uri(uriString + UriSlash);
            }

            return uri;
        }
    }
}