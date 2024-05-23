// <copyright file="CustomCompanyNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CustomCompanyNotFoundException : Exception
    {
        public CustomCompanyNotFoundException()
        {
        }

        public CustomCompanyNotFoundException(ExceptionType type)
        {
            this.Type = type;
        }

        public CustomCompanyNotFoundException(ExceptionType type, string message)
            : base(message)
        {
            this.Type = type;
        }

        public CustomCompanyNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public ExceptionType Type { get; set; }
    }
}
