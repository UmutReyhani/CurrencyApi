using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Collections.Generic;
using Currecny_Api;

namespace CurrencyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get(string baseCurrency, string targetCurrency, int amount)
        {
            if (string.IsNullOrEmpty(baseCurrency) | string.IsNullOrEmpty(targetCurrency))
            {
                return BadRequest("Base currency and target currency should not be empty.");
            }

            if (baseCurrency == targetCurrency)
            {
                return BadRequest("Base currency and target currency should not be the same.");
            }

            if (amount <= 0)
            {
                return BadRequest("Amount should be greater than zero.");
            }

            try
            {
                double baseRateInCHF = currenciesApi.getRate(baseCurrency);
                double targetRateInCHF = currenciesApi.getRate(targetCurrency);

                if (baseRateInCHF == 0 || targetRateInCHF == 0)
                {
                    return BadRequest("Could not retrieve exchange rates.");
                }


                double convertedAmount =Math.Round(amount / baseRateInCHF * targetRateInCHF,2);

                return Ok($"Converted Amount: {convertedAmount}");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}