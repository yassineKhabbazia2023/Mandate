// <copyright file="EmailCommand.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class EmailCommand
    {
        public EmailCommand(string subject, string templateName, string from, string to, List<string> cc, List<AttachmentFileCommand> attachements, Dictionary<string, string> variables)
        {
            this.Subject = subject;
            this.TemplateName = templateName;
            this.From = from;
            this.To = to;
            this.Cc = cc;
            this.Attachements = attachements ?? new List<AttachmentFileCommand>();
            this.Variables = variables;
        }

        public string Subject { get; }

        public string TemplateName { get; }

        public string From { get; }

        public string To { get; }

        public List<string> Cc { get; }

        public List<AttachmentFileCommand> Attachements { get; }

        public Dictionary<string, string> Variables { get; }
    }
}
