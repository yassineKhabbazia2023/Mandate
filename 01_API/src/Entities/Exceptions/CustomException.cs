// <copyright file="CustomException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(ExceptionType type)
        {
            this.Type = type;
        }

        public CustomException(string message)
            : base(message)
        {
        }

        public CustomException(ExceptionType type, string message)
            : base(message)
        {
            this.Type = type;
        }

        public CustomException(ExceptionType type, string message, Exception? innerException)
            : base(message, innerException)
        {
            this.Type = type;
        }

        public ExceptionType Type { get; set; }
    }
}
