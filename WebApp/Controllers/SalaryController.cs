using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApp.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ILogger<SalaryController> _logger;
        private readonly IConfiguration _config;
        private readonly string _api;

        public SalaryController(ILogger<SalaryController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _api = _config.GetValue<string>("ApiSettings:ApiUrl");
        }
        public async Task<IActionResult> Index()
        {
            List<Salary> list = new List<Salary>();
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync($"{_api}Salaries");
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<Salary>>(jstring);
                return View(list);
            }
            else
                return View(list);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Salary salary)
        {
            var client = new HttpClient();

            try
            {
                var json = JsonConvert.SerializeObject(salary);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync($"{_api}Salaries", content);

                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogWarning("Failed to add salary. Status code: {StatusCode}", message.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding salary.");
            }

            return View(salary);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.DeleteAsync($"{_api}Salaries/" + Id);
            if (message.IsSuccessStatusCode)
                return RedirectToAction("Index");
            else
                return View();
        }
    }
}
