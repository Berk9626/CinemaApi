using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController : ControllerBase
	{
		private readonly CinemaDbContext _context;

		public MoviesController(CinemaDbContext context)
		{
			_context = context;
		}

		[Authorize]
		[HttpGet("[action]")]
		public IActionResult AllMovies(string sort, int? pageNumber, int? pagesize) //biz filmlerin ID adı ve,dilini ve puanını, Img döndürmek istiyoruz
		{
			var currentPageNumber = pageNumber ?? 1;
			var currentPageSize = pagesize ?? 5;


			  var movies =    from movie in _context.Movies
			                  select new
			 {
				         Id = movie.Id,
				         Name = movie.Name,
				         Duration = movie.Duration,
				         Language = movie.Language,
				         Rating = movie.Rating,
				         Genre = movie.Genre,
				         ImageUrl = movie.ImageUrl,

			};

			switch (sort)
			{
				case "desc":
					return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));
				case "asc":
					return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));
				default:
					return Ok(movies.Skip((currentPageNumber - 1)* currentPageSize).Take(currentPageSize));

				
			}	

		}
		[Authorize]
		[HttpGet("[action]")]
		public IActionResult FindMovies(string movieName)
		{
			var movies = from movie in _context.Movies
						 where movie.Name.StartsWith(movieName)
						 select new
						 {
							 Id = movie.Id,
							 Name = movie.Name,
							 ImageUrl = movie.ImageUrl,

						 };
			return Ok(movies);

		}

		[Authorize]
		[HttpGet("[action]/{id}")]
		public IActionResult MovieDetail(int id)
		{
			var movie =_context.Movies.Find(id);
			if (movie == null)
			{
				return NotFound();
			}


			return Ok(movie);

		}





		[Authorize(Roles ="Admin")]
		[HttpPost]
		public IActionResult Post([FromForm] Movie movie) //harika muhteşem.Postmande de POST (FORMDATA)
		{//eğer sadece sallıyorum movie entity'den seçseydik FromBody olacaktı. 
			//guidi neden kullandık ?  Gönderilen Image'i  eşsiz yapmalıyız ki server tarafından yollanan aynı resim hata yaratmasın.
			var guid = Guid.NewGuid();
			var filepath = Path.Combine("wwwroot", guid + ".jpg");
			if (movie.Image != null)
			{
				var filestream = new FileStream(filepath, FileMode.Create);
				movie.Image.CopyTo(filestream);
			}
			movie.ImageUrl = filepath/*.Remove(0,7)*/;
			//save file path inside DB,hemen altı
			_context.Movies.Add(movie);
			_context.SaveChanges();

			return StatusCode(StatusCodes.Status201Created);
		}


		[Authorize(Roles = "Admin")]
		[HttpPut("{id}")]
		public IActionResult Put(int id, [FromForm] Movie movie)
		{
			var updatedMovie = _context.Movies.Find(id);
			if (updatedMovie == null)
			{
				return NotFound("No record found against this Id");
			}
			else
			{


				var guid = Guid.NewGuid();
				var filepath = Path.Combine("wwwroot", guid + ".jpg");
				if (movie.Image != null)
				{
					var filestream = new FileStream(filepath, FileMode.Create);
					movie.Image.CopyTo(filestream);
					movie.ImageUrl = filepath/*.Remove(0,7)*/;
				}


				updatedMovie.Name = movie.Name;
				updatedMovie.Description = movie.Description;
				updatedMovie.Language = movie.Language;
				updatedMovie.Duration = movie.Duration;
				updatedMovie.PlayingDate = movie.PlayingDate;
				updatedMovie.PlayingTime = movie.PlayingTime;
				updatedMovie.Rating = movie.Rating;
				updatedMovie.Genre = movie.Genre;
				updatedMovie.TrailorUrl = movie.TrailorUrl;
				updatedMovie.TicketPrice = movie.TicketPrice;



				_context.SaveChanges();
				return Ok("Updated Successfully");
			}

		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var deletedMovie = _context.Movies.Find(id);
			if (deletedMovie == null)
			{
				return NotFound("This Id already gone or missing");
			}

			else
			{
				_context.Movies.Remove(deletedMovie);
				_context.SaveChanges();
				return Ok("Record deleted");

			}

		}


	}
}
