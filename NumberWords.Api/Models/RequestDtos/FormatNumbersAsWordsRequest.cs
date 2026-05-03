using System.ComponentModel.DataAnnotations;

namespace NumberWords.Api.Models;

public class FormatNumbersAsWordsRequest
{
    private const int MAX_LIST_LENGTH = 1000;

    [Required(ErrorMessage = "'numbers' is required.")]
    [MinLength(1, ErrorMessage = "'numbers' must contain at least one value.")]
    [MaxLength(MAX_LIST_LENGTH, ErrorMessage = "'numbers' can contain at most 1000 values.")]
    public long[]? Numbers { get; set; }
}