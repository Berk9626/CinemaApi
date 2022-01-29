using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReservationController : ControllerBase
	{
		private readonly CinemaDbContext _context;

		public ReservationController(CinemaDbContext context)
		{
			_context = context;
		}


		[Authorize]
		[HttpPost]
		public IActionResult Post([FromBody]Reservation reservation)
		{
			reservation.ReservationTime = DateTime.Now;
			_context.Add(reservation);
			_context.SaveChanges();
			return StatusCode(StatusCodes.Status201Created);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public IActionResult GetReservations()
		{
			/* select *from Reservations r inner join Users u                        //(SQL SORGUSU)
             on
             r.UserId = u.Id join Movies m
             on
             r.MovieId = m.Id*/


			var reservations =
			from reservation in _context.Reservations
			join user in _context.Users on reservation.UserId equals user.Id
			join movie in _context.Movies on reservation.MovieId equals movie.Id
			select new
			{
				Id = reservation.Id,
				ReservationTime = reservation.ReservationTime,
				UserName = user.Name,
				MovieName = movie.Name
			};
			return Ok(reservations);

		}

		[Authorize(Roles = "Admin")]
		[HttpGet("{id}")]
		public IActionResult GetReservationDetail(int id)
		{
			var reservationResult =
			(from reservation in _context.Reservations
			join user in _context.Users on reservation.UserId equals user.Id
			join movie in _context.Movies on reservation.MovieId equals movie.Id
			where reservation.Id == id
			select new
			{
				Id = reservation.Id,
				ReservationTime = reservation.ReservationTime,
				UserName = user.Name,
				MovieName = movie.Name,
				Email = user.Email,
				Qty = reservation.Qty,
				Price = reservation.Price,
				Phone = reservation.Phone,
				PlayingDate = movie.PlayingDate,
				PlayingTime = movie.PlayingTime

			}).FirstOrDefault();
			return Ok(reservationResult);



		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var deletedReservations = _context.Reservations.Find(id);
			if (deletedReservations == null)
			{
				return NotFound("This Id already gone or missing");
			}

			else
			{
				_context.Reservations.Remove(deletedReservations);
				_context.SaveChanges();
				return Ok("Record deleted");

			}

		}


	}
}
