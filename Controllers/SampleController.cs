using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SampleController : ControllerBase
	{
		// GET: api/<SampleController>
		[HttpGet]
		[Authorize(Roles = "Users")]
		public string Get()
		{
			return "Hello from the user" ;
		}

		// GET api/<SampleController>/5
		[HttpGet("{id}")]
		[Authorize(Roles ="Admin")]
		public string Get(int id)
		{
			return "Hello from the Admin";
		}

		// POST api/<SampleController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<SampleController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<SampleController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
