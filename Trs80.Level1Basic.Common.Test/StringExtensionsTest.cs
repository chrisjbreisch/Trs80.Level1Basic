using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Common.Extensions;

namespace Trs80.Level1Basic.Common.Test;

[TestClass]
public class StringExtensionsTest
{
    private const string testString = "four Score-and_seven_YeArs AGO";
    private const string testOneWordString = "fOUr";
    private const string testUpperOneLetterString = "F";
    private const string testLowerOneLetterString = "f";
    private const string testCamelCaseString = "fourScoreAndSevenYearsAgo";

    [TestMethod]
    public void Line_Editor_Buffer_Supports_Cursor_Editing()
    {
        var buffer = new LineEditorBuffer("AC");

        buffer.MoveRight();
        buffer.Insert('B');
        buffer.MoveLeft();
        buffer.Delete();
        buffer.Backspace();
        buffer.MoveRight();

        buffer.ToString().Should().Be("C");
        buffer.CursorIndex.Should().Be(1);
    }

    [TestMethod]
    public void Line_Editor_Buffer_Supports_Home_And_End_Movement()
    {
        var buffer = new LineEditorBuffer("ABC");

        buffer.MoveHome();
        buffer.CursorIndex.Should().Be(0);

        buffer.MoveEnd();
        buffer.CursorIndex.Should().Be(3);
    }

    [TestMethod]
    public void Line_Editor_Buffer_Can_Clear_The_Current_Line()
    {
        var buffer = new LineEditorBuffer("CANCEL ME");

        buffer.Clear();

        buffer.ToString().Should().BeEmpty();
        buffer.CursorIndex.Should().Be(0);
    }

    [TestMethod]
    public void Line_Editor_History_Recalls_Recent_Lines_In_Reverse_Order()
    {
        var history = new LineEditorHistory();
        history.Add("FIRST");
        history.Add("SECOND");

        history.TryGetPrevious(out string second).Should().BeTrue();
        history.TryGetPrevious(out string first).Should().BeTrue();
        history.TryGetPrevious(out string oldest).Should().BeTrue();

        second.Should().Be("SECOND");
        first.Should().Be("FIRST");
        oldest.Should().Be("FIRST");
    }

    [TestMethod]
    public void Line_Editor_History_Recalls_Newer_Lines_And_Clears_After_Latest()
    {
        var history = new LineEditorHistory();
        history.Add("FIRST");
        history.Add("SECOND");
        history.TryGetPrevious(out _).Should().BeTrue();
        history.TryGetPrevious(out _).Should().BeTrue();

        history.TryGetNext(out string second).Should().BeTrue();
        history.TryGetNext(out string cleared).Should().BeTrue();

        second.Should().Be("SECOND");
        cleared.Should().BeEmpty();
    }

    [TestMethod]
    public void Line_Editor_History_Restores_Draft_After_Recall_Navigation()
    {
        var history = new LineEditorHistory();
        history.Add("FIRST");
        history.Add("SECOND");

        history.TryGetPrevious("DRAFT", out string recalled).Should().BeTrue();
        history.TryGetPrevious(recalled, out _).Should().BeTrue();
        history.TryGetNext(out _).Should().BeTrue();
        history.TryGetNext(out string draft).Should().BeTrue();

        recalled.Should().Be("SECOND");
        draft.Should().Be("DRAFT");
    }

    [TestMethod]
    public void Auto_Line_Numbering_Generates_Successive_Numbered_Lines()
    {
        var numbering = new AutoLineNumbering();
        numbering.Start(100, 20);

        numbering.TryNumber("PRINT 1", out string first).Should().BeTrue();
        numbering.TryNumber("PRINT 2", out string second).Should().BeTrue();

        first.Should().Be("100 PRINT 1");
        second.Should().Be("120 PRINT 2");
    }

    [TestMethod]
    public void Auto_Line_Numbering_Exposes_The_Next_Line_Number()
    {
        var numbering = new AutoLineNumbering();
        numbering.Start(10, 10);

        numbering.NextLineNumber.Should().Be(10);
        numbering.TryNumber("PRINT 1", out _).Should().BeTrue();
        numbering.NextLineNumber.Should().Be(20);
    }

    [TestMethod]
    public void Auto_Line_Numbering_Rejects_Blank_Lines_And_Can_Stop()
    {
        var numbering = new AutoLineNumbering();
        numbering.Start();

        numbering.TryNumber("  ", out string blank).Should().BeFalse();
        numbering.IsActive.Should().BeTrue();

        numbering.Stop();

        numbering.IsActive.Should().BeFalse();
        numbering.TryNumber("PRINT 1", out string stopped).Should().BeFalse();
        stopped.Should().BeEmpty();
    }

    [TestMethod]
    public void Can_Convert_Empty_String_To_Pascal_Case()
    {
        string? result = string.Empty.ToPascalCase();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Can_Convert_Empty_String_To_Camel_Case()
    {
        string? result = string.Empty.ToCamelCase();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Can_Convert_Empty_String_To_Kebab_Case()
    {
        string? result = string.Empty.ToKebabCase();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Can_Convert_Empty_String_To_Snake_Case()
    {
        string? result = string.Empty.ToSnakeCase();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Can_Convert_Empty_String_To_Caps_Case()
    {
        string? result = string.Empty.ToCapsCase();
        result.Should().BeEmpty();
    }

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

    [TestMethod]
    public void Can_Separate_Camel_Case_String()
    {
        string? result = testCamelCaseString.SeparateWordsByCase('_');
        result.Should().Be("four_Score_And_Seven_Years_Ago");
    }

    [TestMethod]
    public void Can_Convert_Separated_String_To_Pascal_Case()
    {
        string? result = testCamelCaseString.SeparateWordsByCase('_').ToPascalCase();
        result.Should().Be("FourScoreAndSevenYearsAgo");

    }

    [TestMethod]
    public void Can_Retrieve_Left_String()
    {
        string? result = testString.Left(4);
        result.Should().Be("four");
    }

    [TestMethod]
    public void Can_Retrieve_Left_String_Of_Zero_Chars()
    {
        string? result = testString.Left(0);
        result.Should().BeNull();
    }

    [TestMethod]
    public void Can_Retrieve_Left_String_Of_Empty_String()
    {
        string? result = string.Empty.Left(1);
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Can_Retrieve_Left_String_Of_Negative_Chars()
    {
        string? result = testString.Left(-1);
        result.Should().BeNull();
    }

    [TestMethod]
    public void Can_Retrieve_Left_String_Beyond_Length_Of_String()
    {
        string? result = testOneWordString.Left(20);
        result.Should().Be(testOneWordString);
    }
}