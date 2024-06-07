using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonDetailController : Controller
    {

        private readonly ILogger<PersonDetailController> _logger;
        private readonly IConfiguration _config;
        private readonly string _api;

        public PersonDetailController(ILogger<PersonDetailController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _api = _config.GetValue<string>("ApiSettings:ApiUrl");
        }

        public IActionResult Add()
        {
            PersonDetail personDetail = new PersonDetail();
            return View(personDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PersonDetail personDetail)
        {
            if (!ModelState.IsValid)
            {
                return View(personDetail);
            }

            try
            {
                var client = new HttpClient();
                {
                    var json = JsonConvert.SerializeObject(personDetail);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage message = await client.PostAsync($"{_api}persondetails", content);

                    if (message.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Person");
                    }
                    else
                    {
                        _logger.LogWarning("Failed to add person detail. Status code: {StatusCode}", message.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding person detail.");
            }

            return View(personDetail);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync($"{_api}persondetails/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var personDetail = JsonConvert.DeserializeObject<PersonDetail>(json);
                    return View(personDetail);
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve person detail. Status code: {StatusCode}", response.StatusCode);
                    return RedirectToAction("Index", "Person");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving person detail.");
                return RedirectToAction("Index", "Person");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PersonDetail personDetail)
        {
            if (!ModelState.IsValid)
            {
                return View(personDetail);
            }

            try
            {
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(personDetail);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync($"{_api}persondetails/{id}", content);

                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Person");
                }
                else
                {
                    _logger.LogWarning("Failed to update person detail. Status code: {StatusCode}", message.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating person detail.");
            }

            return View(personDetail);
        }
    }
}
