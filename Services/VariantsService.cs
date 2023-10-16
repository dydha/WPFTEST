using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WPFTEST.DTOs;
using WPFTEST.Exceptions;

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
                    return variant;
                }
               
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new NotFoundException();
                }
                else
                {
                    throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
                }
                
              
            }          
            catch(Exception)
            {
                throw;
            }
        }
       
    }


}
