// <copyright file="StringExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    public class StringExtensionsTest
    {
        [Theory]
        [InlineData("Hello World", 0, 5, "Hello")]
        [InlineData("Hello World", 6, 5, "World")]
        [InlineData("Data", 0, 4, "Data")]
        public void Extract_ShouldReturnCorrectSubstring(string input, int index, int length, string expected)
        {
            var result = StringExtensions.Extract(input, index, length);
            result.Should().Be(expected);
        }

        [Fact]
        public void Extract_ShouldReturnInput_WhenNullOrEmpty()
        {
            var result = StringExtensions.Extract(null!, 0, 1);
            result.Should().BeNull();

            result = StringExtensions.Extract(string.Empty, 0, 1);
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("HELLO", true)]
        [InlineData("HeLLo", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void AllCharactersUpperCase_ShouldEvaluateCorrectly(string input, bool expected)
        {
            var result = StringExtensions.AllCharactersUpperCase(input);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("m John DOE", "m", "DOE", "John")]
        [InlineData("mme Jane DOE SMITH", "mme", "DOE SMITH", "Jane")]
        public void ExtractPersonInfo_ShouldExtractCorrectly(string input, string expectedSexe, string expectedNom, string expectedPrenom)
        {
            var (sexe, nom, prenom) = StringExtensions.ExtractPersonInfo(input);
            sexe.Should().Be(expectedSexe);
            nom.Should().Be(expectedNom);
            prenom.Should().Be(expectedPrenom);
        }

        [Theory]
        [InlineData("invalid input")]
        [InlineData("m John")]
        [InlineData("A John DAVID")]
        [InlineData("m john david")]
        public void ExtractPersonInfo_ShouldThrow_WhenInvalid(string input)
        {
            Action act = () => StringExtensions.ExtractPersonInfo(input);
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
