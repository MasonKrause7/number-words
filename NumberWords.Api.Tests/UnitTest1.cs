using NumberWords.Api.Models;

namespace NumberWords.Api.Tests;

public class NumberToWordConversionServiceTests
{
    private readonly NumberToWordConversionService _service = new();

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
    public void ConvertNumbersToWords_WhenInputIsSingleDigit_ReturnsExpectedWords()
    {
        long[] input = [7]

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Seven", result[0])
    }

    [Fact]
    public void ConvertNumbersToWords_WhenInputIncludesThousands_ReturnsExpectedWords()
    {
        long[] input = [8999];

        string[] result = _service.ConvertNumbersToWords(input);

        Assert.Single(result);
        Assert.Equal("Eight Thousand Nine Hundred Ninety Nine", result[0]);
    }

}
