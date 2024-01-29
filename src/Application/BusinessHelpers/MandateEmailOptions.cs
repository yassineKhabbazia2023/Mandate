// <copyright file="MandateEmailOptions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class MandateEmailOptions
    {
        /// <summary>
        /// Gets or sets the subject line for emails sent about mandate cancellations.
        /// </summary>
        public string MandateCancellationSubject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the template name for emails sent about mandate cancellations.
        /// </summary>
        public string MandateCancellationTemplateName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the from email address for emails sent about mandate cancellations.
        /// </summary>
        public string MandateCancellationFromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the to email address for emails sent about mandate cancellations.
        /// </summary>
        public string MandateCancellationToEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of CC email addresses for emails sent about mandate cancellations.
        /// </summary>
        public List<string> MandateCancellationCcEmails { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the subject line for emails sent about newly uploaded mandates.
        /// </summary>
        public string MandateUploadedSubject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the template name for emails sent about newly uploaded mandates.
        /// </summary>
        public string MandateUploadedTemplateName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the from email address for emails sent about newly uploaded mandates.
        /// </summary>
        public string MandateUploadedFromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the to email address for emails sent about newly uploaded mandates.
        /// </summary>
        public string MandateUploadedToEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of CC email addresses for emails sent about newly uploaded mandates.
        /// </summary>
        public List<string> MandateUploadedCcEmails { get; set; } = new List<string>();
    }
}
