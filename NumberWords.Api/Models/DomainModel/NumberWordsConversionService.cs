namespace NumberWords.Api.Models;

public class NumberWordsConversionService
{
    private static readonly string[] Ones = 
    [
        "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine"
    ];

    private static readonly string[] Teens = 
    [
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
    ];

    private static readonly string[] Tens = 
    [
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    ];

    private static readonly string[] Magnitudes = 
    [
        "", "Thousand", "Million", "Billion", "Trillion", "Quadrillion", "Quintillion"
    ];

    public string[] ConvertNumbersToWords(long[] numbers)
    {
        var words = new string[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            words[i] = ConvertNumberToWord(numbers[i]);
        }
        return words;
    }

    /// <summary>
    /// Converts numbers up/down to long.MaxValue/long.MinValue.
    /// Handles conversion in chunks of 3 digits, or `Magnitudes`.
    /// There are two components to conversion:
    /// 1. Convert 3 digits, values 0-999
    /// 2. Append the `magnitude`
    /// Then move to the next chunk until all digits are processed
    /// Then join the `parts` of the string into the `result`
    /// </summary>
    private string ConvertNumberToWord(long number)
    {
        if (number == 0) return "Zero";

        bool isNegative = number < 0;
        number = Math.Abs(number);

        var parts = new List<string>();
        int magIndex = 0;

        // iterate over chunks of 3 digits (a magnitude) at a time
        while (number > 0)
        {
            int chunk = (int)(number % 1000);

            if (chunk > 0)
            {
                string chunkWords = ConvertThreeDigits(chunk);
                if (!string.IsNullOrEmpty(Magnitudes[magIndex]))
                {
                    chunkWords = $"{chunkWords} {Magnitudes[magIndex]}";
                }

                parts.Insert(0, chunkWords);
            }

            number /= 1000;
            magIndex++;
        }

        string result = string.Join(" ", parts);
        return isNegative ? $"Negative {result}" : result;
    }

    private string ConvertThreeDigits(int number)
    {
        var parts = new List<string>();

        int hundreds = number / 100;
        int remainder = number % 100;

        if (hundreds > 0)
        {
            parts.Add($"{Ones[hundreds]} Hundred");
        }

        if (remainder >= 20)
        {
            int tensDigit = remainder / 10;
            int onesDigit = remainder % 10;

            parts.Add(Tens[tensDigit]);
            if (onesDigit > 0)
            {
                parts.Add(Ones[onesDigit]);
            }
        }
        else if (remainder >= 10)
        {
            parts.Add(Teens[remainder - 10]);
        }
        else if (remainder > 0)
        {
            parts.Add(Ones[remainder]);
        }

        return string.Join(" ", parts);
    }
}