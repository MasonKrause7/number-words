using NumberWords.Api.Models;

namespace NumberWords.Api.Tests;

public class NumberWordsConversionServiceTests
{
    private readonly NumberWordsConversionService _service = new();

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsZero_ReturnsZero()
    {
        long[] input = [0];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Zero", result[0].Word);
        Assert.Equal(0, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsNegative_PrefixesNegative()
    {
        long[] input = [-42];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Negative Forty Two", result[0].Word);
        Assert.Equal(-42, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsOnes_ReturnsExpectedWords()
    {
        long[] input = [7];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Seven", result[0].Word);
        Assert.Equal(7, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsTeens_ReturnsExpectedWords()
    {
        long[] input = [13];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Thirteen", result[0].Word);
        Assert.Equal(13, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsTens_ReturnsExpectedWords()
    {
        long[] input = [40];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Forty", result[0].Word);
        Assert.Equal(40, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsTensAndOnes_ReturnsExpectedWords()
    {
        long[] input = [42];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Forty Two", result[0].Word);
        Assert.Equal(42, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsHundredsOnly_ReturnsExpectedWords()
    {
        long[] input = [300];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Three Hundred", result[0].Word);
        Assert.Equal(300, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsHundredsWithRemainder_ReturnsExpectedWords()
    {
        long[] input = [305];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Three Hundred Five", result[0].Word);
        Assert.Equal(305, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIncludesThousands_ReturnsExpectedWords()
    {
        long[] input = [8999];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Eight Thousand Nine Hundred Ninety Nine", result[0].Word);
        Assert.Equal(8999, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputHasMultipleValues_PreservesInputOrder()
    {
        long[] input = [2, 11, 1, -16];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Equal(4, result.Length);
        Assert.Equal(2, result[0].OriginalNumber);
        Assert.Equal("Two", result[0].Word);
        Assert.False(result[0].IsOver9000);

        Assert.Equal(11, result[1].OriginalNumber);
        Assert.Equal("Eleven", result[1].Word);
        Assert.False(result[1].IsOver9000);

        Assert.Equal(1, result[2].OriginalNumber);
        Assert.Equal("One", result[2].Word);
        Assert.False(result[2].IsOver9000);

        Assert.Equal(-16, result[3].OriginalNumber);
        Assert.Equal("Negative Sixteen", result[3].Word);
        Assert.False(result[3].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsGreaterThan9000_SetsOver9000Flag()
    {
        long[] input = [9000, 9001];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.False(result[0].IsOver9000);
        Assert.True(result[1].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsLongMinValue_ReturnsExpectedWords()
    {
        long[] input = [long.MinValue];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Negative Nine Quintillion Two Hundred Twenty Three Quadrillion Three Hundred Seventy Two Trillion Thirty Six Billion Eight Hundred Fifty Four Million Seven Hundred Seventy Five Thousand Eight Hundred Eight", result[0].Word);
        Assert.Equal(long.MinValue, result[0].OriginalNumber);
        Assert.True(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsLongMaxValue_ReturnsExpectedWords()
    {
        long[] input = [long.MaxValue];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.Single(result);
        Assert.Equal("Nine Quintillion Two Hundred Twenty Three Quadrillion Three Hundred Seventy Two Trillion Thirty Six Billion Eight Hundred Fifty Four Million Seven Hundred Seventy Five Thousand Eight Hundred Seven", result[0].Word);
        Assert.Equal(long.MaxValue, result[0].OriginalNumber);
        Assert.True(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWordItems_WhenInputIsNegativeLessThanNeg9000_SetsOver9000Flag()
    {
        long[] input = [-9000, -9001];

        NumberWordItem[] result = _service.ConvertNumbersToWordItems(input);

        Assert.False(result[0].IsOver9000);
        Assert.True(result[1].IsOver9000);
    }

}
