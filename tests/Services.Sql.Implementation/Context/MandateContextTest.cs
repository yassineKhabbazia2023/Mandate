// <copyright file="MandateContextTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    public class MandateContextTest
    {
        [Fact]
        public void Context_OptionsNullException()
        {
            Action act = () => { _ = new MandateContext(options: null!); };

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'options')");
        }
    }
}
