using Microsoft.AspNetCore.Mvc;
using NumberWords.Api.Models;

namespace NumberWords.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //NumberWords -> /api/numberwords
public class NumberWordsController : ControllerBase 
{
    private readonly NumberToWordConversionService _numberToWordConversionService;
    // dependency injection
    public NumberWordsController(NumberToWordConversionService numberToWordConversionService)
    {
        _numberToWordConversionService = numberToWordConversionService;
    }

    [HttpPost]
    public IActionResult Post([FromBody] NumberWordsRequest request)
    {
        string[] words = _numberToWordConversionService.ConvertNumbersToWords(request.Numbers);
        return Ok(new NumberWordsResponse { NumberWords = words });
    }
}

