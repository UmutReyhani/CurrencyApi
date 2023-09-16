using RestSharp;

namespace Currecny_Api
{
    public class currenciesApi
    {
        public static DateTime lastCheck = DateTime.MinValue;
        public static Dictionary<string, double> currencies { get; set; } = new Dictionary<string, double>();

        public static void UpdateAllRatesFromCHF()
        {
            var client = new RestClient($"https://v6.exchangerate-api.com/v6/api_key/latest/CHF");
            var request = new RestRequest(Method.GET);

            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
                var rates = jsonResponse.conversion_rates;

                foreach (var rate in rates)
                {
                    string currency = rate.Name;
                    double value = rate.Value;
                    currencies[currency] = value;
                }
                lastCheck = DateTime.Now;
            }
            else
            {
                throw new Exception($"Failed to update all rates: {response.ErrorMessage}");
            }
        }
        public static double getRate(string targetCurrency)
        {
            bool exist = currencies.ContainsKey(targetCurrency) && lastCheck > DateTime.Now.AddHours(-6);

            if (!exist)
            {
                UpdateAllRatesFromCHF();
            }

            if (currencies.ContainsKey(targetCurrency))
            {
                return currencies[targetCurrency];
            }
            else
            {
                throw new Exception($"Failed to get rate for currency {targetCurrency}");
            }
        }
    }
}