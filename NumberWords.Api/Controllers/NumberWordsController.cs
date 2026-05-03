using Microsoft.AspNetCore.Mvc;
using NumberWords.Api.Models;

namespace NumberWords.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //NumberWords -> /api/numberwords
public class NumberWordsController : ControllerBase 
{
    private readonly NumberWordsConversionService _numberWordsConversionService;
    // dependency injection
    public NumberWordsController(NumberWordsConversionService numberWordsConversionService)
    {
        _numberWordsConversionService = numberWordsConversionService;
    }

    [HttpPost]
    public IActionResult Post([FromBody] NumberWordsRequest request)
    {
        NumberWordItem[] items = _numberWordsConversionService.ConvertNumbersToWords(request.Numbers);
        Array.Sort(items, (a, b) => string.Compare(a.Word, b.Word, StringComparison.OrdinalIgnoreCase));
        return Ok(new NumberWordsResponse { NumberWordItems = items });
    }
}

