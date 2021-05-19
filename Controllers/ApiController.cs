using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using mainModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealCommerce.DTO;
using mainModel = RealCommerce.DB.mainModel;

namespace RealCommerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {

        [HttpGet("GetQuery/{q}")]
        public ActionResult<List<DTO.Search>> GetQuery(string q)
        {

            return Api.GetQuery(q);

        }
        
        [HttpGet("GetWeather/{key}")]
        public ActionResult<DTO.WeatherResult> GetWeather(string key)
        {

            return Api.GetWeather(key);
            
        }

        [HttpGet("Add/{key}")]
        public ActionResult<int> Add(string key)
        {
            
            return Api.Add(key);
            
        }
        
        [HttpGet("Delete/{key}")]
        public ActionResult<int> Delete(string key)
        {

            return Api.Delete(key);

        }
    }
}
