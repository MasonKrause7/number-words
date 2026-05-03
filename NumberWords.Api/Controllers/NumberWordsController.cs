using Microsoft.AspNetCore.Mvc;
using NumberWords.Api.Models;
using NumberWords.Api.Services;

namespace NumberWords.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //NumberWords -> /api/numberwords
public class NumberWordsController : ControllerBase 
{
    private readonly NumberToWordConversionService _numberToWordConversionService;
    // service injection
    public NumberWordsController(NumberToWordConversionService numberToWordConversionService)
    {
        _numberToWordConversionService = numberToWordConversionService;
    }

    [HttpPost]
    public IActionResult Post([FromBody] FormatNumbersAsWordsRequest request)
    {
        string[] words = _numberToWordConversionService.ConvertNumbersToWords(request.Numbers);
        return Ok(new FormatNumbersAsWordsResponse { NumberWords = words });
    }
}

