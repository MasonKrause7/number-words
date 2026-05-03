using NumberWords.Api.Models;

namespace NumberWords.Api.Tests;

public class NumberWordsConversionServiceTests
{
    private readonly NumberWordsConversionService _service = new();

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsZero_ReturnsZero()
    {
        long[] input = [0];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Zero", result[0].Word);
        Assert.Equal(0, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsNegative_PrefixesNegative()
    {
        long[] input = [-42];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Negative Forty Two", result[0].Word);
        Assert.Equal(-42, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsOnes_ReturnsExpectedWords()
    {
        long[] input = [7];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Seven", result[0].Word);
        Assert.Equal(7, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsTeens_ReturnsExpectedWords()
    {
        long[] input = [13];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Thirteen", result[0].Word);
        Assert.Equal(13, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsTens_ReturnsExpectedWords()
    {
        long[] input = [40];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Forty", result[0].Word);
        Assert.Equal(40, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsTensAndOnes_ReturnsExpectedWords()
    {
        long[] input = [42];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Forty Two", result[0].Word);
        Assert.Equal(42, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsHundredsOnly_ReturnsExpectedWords()
    {
        long[] input = [300];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Three Hundred", result[0].Word);
        Assert.Equal(300, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsHundredsWithRemainder_ReturnsExpectedWords()
    {
        long[] input = [305];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Three Hundred Five", result[0].Word);
        Assert.Equal(305, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIncludesThousands_ReturnsExpectedWords()
    {
        long[] input = [8999];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Eight Thousand Nine Hundred Ninety Nine", result[0].Word);
        Assert.Equal(8999, result[0].OriginalNumber);
        Assert.False(result[0].IsOver9000);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputHasMultipleValues_PreservesInputOrder()
    {
        long[] input = [2, 11, 1, -16];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

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
    public void ConvertNumbersToWords_WhenInputIsGreaterThan9000_SetsOver9000Flag()
    {
        long[] input = [9000, 9001];

        NumberWordItem[] result = _service.ConvertNumbersToWords(input);

        Assert.False(result[0].IsOver9000);
        Assert.True(result[1].IsOver9000);
    }

}
