using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trs80.Level1Basic.Common.Extensions;

namespace Trs80.Level1Basic.Extensions.Test
{
    [TestClass]
    public class StringExtensionsTest
    {
        private const string testString = "four Score-and_seven_YeArs AGO";
        private const string testOneWordString = "fOUr";
        private const string testUpperOneLetterString = "F";
        private const string testLowerOneLetterString = "f";

        [TestMethod]
        public void Can_Convert_String_To_Pascal_Case()
        {
            string? result = testString.ToPascalCase();
            result.Should().Be("FourScoreAndSevenYearsAgo");
        }

        [TestMethod]
        public void Can_Convert_String_To_Camel_Case()
        {
            string? result = testString.ToCamelCase();
            result.Should().Be("fourScoreAndSevenYearsAgo");
        }

        [TestMethod]
        public void Can_Convert_String_To_Snake_Case()
        {
            string? result = testString.ToSnakeCase();
            result.Should().Be("four_score_and_seven_years_ago");
        }

        [TestMethod]
        public void Can_Convert_String_To_Caps_Case()
        {
            string? result = testString.ToCapsCase();
            result.Should().Be("FOUR_SCORE_AND_SEVEN_YEARS_AGO");
        }

        [TestMethod]
        public void Can_Convert_String_To_Kebab_Case()
        {
            string? result = testString.ToKebabCase();
            result.Should().Be("four-score-and-seven-years-ago");
        }

        [TestMethod]
        public void Can_Convert_One_Word_String_To_Pascal_Case()
        {
            string? result = testOneWordString.ToPascalCase();
            result.Should().Be("Four");
        }

        [TestMethod]
        public void Can_Convert_One_Word_String_To_Camel_Case()
        {
            string? result = testOneWordString.ToCamelCase();
            result.Should().Be("four");
        }

        [TestMethod]
        public void Can_Convert_One_Word_String_To_Snake_Case()
        {
            string? result = testOneWordString.ToSnakeCase();
            result.Should().Be("four");
        }

        [TestMethod]
        public void Can_Convert_One_Word_String_To_Caps_Case()
        {
            string? result = testOneWordString.ToCapsCase();
            result.Should().Be("FOUR");
        }

        [TestMethod]
        public void Can_Convert_One_Word_String_To_Kebab_Case()
        {
            string? result = testOneWordString.ToKebabCase();
            result.Should().Be("four");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Upper_String_To_Pascal_Case()
        {
            string? result = testUpperOneLetterString.ToPascalCase();
            result.Should().Be("F");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Upper_String_To_Camel_Case()
        {
            string? result = testUpperOneLetterString.ToCamelCase();
            result.Should().Be("f");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Upper_String_To_Snake_Case()
        {
            string? result = testUpperOneLetterString.ToSnakeCase();
            result.Should().Be("f");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Upper_String_To_Caps_Case()
        {
            string? result = testUpperOneLetterString.ToCapsCase();
            result.Should().Be("F");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Upper_String_To_Kebab_Case()
        {
            string? result = testUpperOneLetterString.ToKebabCase();
            result.Should().Be("f");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Lower_String_To_Pascal_Case()
        {
            string? result = testLowerOneLetterString.ToPascalCase();
            result.Should().Be("F");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Lower_String_To_Camel_Case()
        {
            string? result = testLowerOneLetterString.ToCamelCase();
            result.Should().Be("f");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Lower_String_To_Snake_Case()
        {
            string? result = testLowerOneLetterString.ToSnakeCase();
            result.Should().Be("f");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Lower_String_To_Caps_Case()
        {
            string? result = testLowerOneLetterString.ToCapsCase();
            result.Should().Be("F");
        }

        [TestMethod]
        public void Can_Convert_One_Letter_Lower_String_To_Kebab_Case()
        {
            string? result = testLowerOneLetterString.ToKebabCase();
            result.Should().Be("f");
        }
    }
}