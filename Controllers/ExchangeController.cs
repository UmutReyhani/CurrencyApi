using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Currecny_Api;
using System.ComponentModel.DataAnnotations;

namespace CurrencyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        public class ExchangeRequest
        {
            [Required]
            public string baseCurrency { get; set; }
            [Required]
            public string targetCurrency { get; set; }
            [Required]
            public decimal amount { get; set; }
            [Required]
            public int captcha { get; set; }
        }

        public class ExchangeResponse
        {
            [Required]
            public string type { get; set; }
            public string message { get; set; }
        }

        [HttpPost]
        public ActionResult<ExchangeResponse> Post([FromBody] ExchangeRequest req)
        {
            var response = new ExchangeResponse();

            string storedCaptcha = HttpContext.Session.GetString("CaptchaAnswer");
            if (storedCaptcha == null || !storedCaptcha.Equals(req.captcha.ToString()))
            {
                response.type = "Error";
                response.message = "Invalid CAPTCHA.";
                return BadRequest(response);
            }

            if (string.IsNullOrEmpty(req.baseCurrency) || string.IsNullOrEmpty(req.targetCurrency))
            {
                response.type = "Error";
                response.message = "Base currency and target currency should not be empty.";
                return BadRequest(response);
            }

            if (req.baseCurrency == req.targetCurrency)
            {
                response.type = "Error";
                response.message = "Base currency and target currency should not be the same.";
                return BadRequest(response);
            }

            if (req.amount <= 0)
            {
                response.type = "Error";
                response.message = "Amount should be greater than zero.";
                return BadRequest(response);
            }

            try
            {
                double baseRateInCHF = currenciesApi.getRate(req.baseCurrency);
                double targetRateInCHF = currenciesApi.getRate(req.targetCurrency);

                if (baseRateInCHF == 0 | targetRateInCHF == 0)
                {
                    response.type = "Error";
                    response.message = "Could not retrieve exchange rates.";
                    return BadRequest(response);
                }

                double convertedAmount = Math.Round((double)req.amount / baseRateInCHF * targetRateInCHF, 2);

                response.type = "Success";
                response.message = $"Converted Amount: {convertedAmount}";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.type = "Error";
                response.message = e.Message;
                return StatusCode(500, response);
            }
        }
    }
}
