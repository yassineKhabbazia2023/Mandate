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
            this.Attachements = attachements;
            this.Variables = variables;
        }

        public string Subject { get; set; }

        public string TemplateName { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public List<string> Cc { get; set; }

        public List<AttachmentFileCommand> Attachements { get; set; }

        public Dictionary<string, string> Variables { get; set; }
    }
}
