using WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApp.Controllers
{
    public class PersonController : Controller
    {
        private readonly ILogger<DepartmentController> _logger;
        //HINT task 8 start

        private readonly IConfiguration _config;
        private readonly string _api;
        public PersonController(IConfiguration config, ILogger<DepartmentController> logger)
        {
            _config = config;
            _api = _config.GetValue<string>("ApiSettings:ApiUrl");
            _logger = logger;
        }

        //HINT task 8 end
        public async Task<IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync($"{_api}persons");
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                List<PersonInformation> list = JsonConvert.DeserializeObject<List<PersonInformation>>(jstring);
                return View(list);
            }
            else
            {
                return View(new List<PersonInformation>());
            }

        }
        public IActionResult Add()
        {
            Person person = new Person();
            return View(person);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Person person)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var jsonPerson = JsonConvert.SerializeObject(person);
                StringContent content = new StringContent(jsonPerson, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync($"{_api}persons", content);
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(person);
                }
            }
            else
            {
                return View(person);
            }
        }

        public async Task<IActionResult> Update(int Id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync($"{_api}persons/" + Id);
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                Person person = JsonConvert.DeserializeObject<Person>(jstring);
                return View(person);
            }
            else
                return RedirectToAction("Add");
        }

        [HttpPost]
        public async Task<IActionResult> Update(Person person)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var jsonperson = JsonConvert.SerializeObject(person);
                StringContent content = new StringContent(jsonperson, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync($"{_api}persons", content);
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(person);
                }
            }
            else
            {
                return View(person);
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var client = new HttpClient();
            try
            {
                HttpResponseMessage message = await client.DeleteAsync($"{_api}persons/{Id}");
                if (message.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Person with Id = {Id} deleted successfully.");
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogWarning($"Failed to delete person with Id = {Id}. Status Code: {message.StatusCode}");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting person with Id = {Id}");
                return RedirectToAction("Index");
            }
        }
    }
}
