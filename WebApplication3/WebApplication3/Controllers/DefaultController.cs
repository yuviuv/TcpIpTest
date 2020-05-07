using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/[controller]/{name}")]
        public string GetName(string name)
        {
            return "Name Controller Called : " + name;
            //return "Name Controller Called : ";
        }

    }
}