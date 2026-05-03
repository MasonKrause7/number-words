using NumberWords.Api.Models;

namespace NumberWords.Api.Tests;

public class NumberWordsConversionServiceTests
{
    private readonly NumberWordsConversionService _service = new();

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsZero_ReturnsZero()
    {
        long[] input = [0];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Zero", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsNegative_PrefixesNegative()
    {
        long[] input = [-42];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Negative Forty Two", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsOnes_ReturnsExpectedWords()
    {
        long[] input = [7];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Seven", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsTeens_ReturnsExpectedWords()
    {
        long[] input = [13];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Thirteen", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsTens_ReturnsExpectedWords()
    {
        long[] input = [40];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Forty", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsTensAndOnes_ReturnsExpectedWords()
    {
        long[] input = [42];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Forty Two", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsHundredsOnly_ReturnsExpectedWords()
    {
        long[] input = [300];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Three Hundred", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIsHundredsWithRemainder_ReturnsExpectedWords()
    {
        long[] input = [305];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Three Hundred Five", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIncludesThousands_ReturnsExpectedWords()
    {
        long[] input = [8999];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Eight Thousand Nine Hundred Ninety Nine", result[0]);
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputHasMultipleValues_PreservesInputOrder()
    {
        long[] input = [2, 11, 1, -16];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Equal(4, result.Length);
        Assert.Equal("Two", result[0]);
        Assert.Equal("Eleven", result[1]);
        Assert.Equal("One", result[2]);
        Assert.Equal("Negative Sixteen", result[3]);
    }

}
