using System.ComponentModel.DataAnnotations;
using NumberWords.Api.Models;

namespace NumberWords.Api.Tests;

public class NumberWordsRequestValidationTests
{
    [Fact]
    public void Validate_WhenNumbersAreValid_PassesValidation()
    {
        NumberWordsRequest request = new()
        {
            Numbers = [1, 2, 3]
        };

        bool isValid = TryValidate(request, out List<ValidationResult> results);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_WhenNumbersAreEmpty_FailsMinLengthValidation()
    {
        NumberWordsRequest request = new()
        {
            Numbers = []
        };

        bool isValid = TryValidate(request, out List<ValidationResult> results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "'numbers' must contain at least one value.");
    }

    [Fact]
    public void Validate_WhenNumbersExceedMaxLength_FailsMaxLengthValidation()
    {
        NumberWordsRequest request = new()
        {
            Numbers = Enumerable.Repeat(1L, 1001).ToArray()
        };

        bool isValid = TryValidate(request, out List<ValidationResult> results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "'numbers' can contain at most 1000 values.");
    }

    [Fact]
    public void Validate_WhenNumbersAreNull_FailsRequiredValidation()
    {
        NumberWordsRequest request = new()
        {
            Numbers = null!
        };

        bool isValid = TryValidate(request, out List<ValidationResult> results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "'numbers' is required.");
    }

    private static bool TryValidate(NumberWordsRequest request, out List<ValidationResult> results)
    {
        ValidationContext context = new(request);
        results = [];
        return Validator.TryValidateObject(request, context, results, validateAllProperties: true);
    }
}
