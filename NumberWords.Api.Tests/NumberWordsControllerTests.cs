using Microsoft.AspNetCore.Mvc;
using NumberWords.Api.Controllers;
using NumberWords.Api.Models;

namespace NumberWords.Api.Tests;

public class NumberWordsControllerTests
{
    [Fact]
    public void Post_WhenGivenUnsortedNumbers_ReturnsAlphabeticallySortedWordList()
    {
        NumberWordsConversionService service = new();
        NumberWordsController controller = new(service);
        NumberWordsRequest request = new()
        {
            Numbers = [1, 2, 3, 11, 8999, 16]
        };

        IActionResult actionResult = controller.Post(request);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(actionResult);
        NumberWordsResponse response = Assert.IsType<NumberWordsResponse>(okResult.Value);

        string[] expected =
        [
            "Eight Thousand Nine Hundred Ninety Nine",
            "Eleven",
            "One",
            "Sixteen",
            "Three",
            "Two"
        ];

        Assert.Equal(expected, response.NumberWords);
    }
}
