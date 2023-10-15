using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WPFTEST.DTOs;

namespace WPFTEST.Services
{
    public class VariantsService
    {
        private static VariantsService? instance;
        private readonly HttpClient _httpClient;

        private VariantsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public static VariantsService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VariantsService(new HttpClient()); // Vous pouvez injecter un HttpClient ici
                }
                return instance;
            }
        }

        public async Task<VariantDTO> GetVariantAsync(string eanNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7023/api/Variants/get-variant-by-ean-number/{eanNumber}");
                if (response.IsSuccessStatusCode)
                {
                    var variant = await response.Content.ReadFromJsonAsync<VariantDTO>();
                    return variant!;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new Exception();
            }
        }
        public async Task<IEnumerable<string>> GetAllVariantAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7023/api/Variants/get-all");
                if (response.IsSuccessStatusCode)
                {
                    var variants = await response.Content.ReadFromJsonAsync<IEnumerable<VariantDTO>>();
                    return variants.Select(v => v.Name).ToList();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new Exception();
            }
        }
    }


}
