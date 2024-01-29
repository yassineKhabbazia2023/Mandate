// <copyright file="AttachmentFileCommand.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class AttachmentFileCommand
    {
        public AttachmentFileCommand(string fileName, string content)
        {
            this.FileName = fileName;
            this.Content = content;
        }

        public string FileName { get; }

        public string Content { get; }
    }
}
