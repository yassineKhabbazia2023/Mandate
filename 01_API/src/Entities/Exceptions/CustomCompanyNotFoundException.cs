// <copyright file="CustomCompanyNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class CustomCompanyNotFoundException : CustomException
    {
        public CustomCompanyNotFoundException()
        {
        }

        public CustomCompanyNotFoundException(ExceptionType type, string message)
            : base(type, message)
        {
        }
    }
}
