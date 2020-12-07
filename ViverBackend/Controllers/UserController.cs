using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViverBackend.Entities;
using ViverBackend.Entities.Models;
using ViverBackend.Payloads;
using static ViverBackend.Enums;

namespace ViverBackend.Controllers
{
    [Authorize]
    public class UserController : ControllerBase
    {
        // comm
        private readonly ViverContext _db;

        public UserController(ViverContext db)
        {
            _db = db;
        }

        // /user/getall?pageSize=50&pageNumber=2&sortType=1
        [HttpGet]
        public ActionResult<List<User>> GetAll(int pageSize, int pageNumber, UsersSortType sortType)
        {
            var currentUser = HttpContext.User;

            if (currentUser.HasClaim(claim => claim.Type == "Role"))
            {
                var role = currentUser.Claims.FirstOrDefault(c => c.Type == "Role").Value;
                if (role == "Admin")
                {
                    // as no tracking for performance improvement when you do not need to track changes
                    var usersQuery = _db.Users.AsNoTracking();

                    if (sortType == UsersSortType.FirstNameAscendent)
                        usersQuery = usersQuery.OrderBy(u => u.FirstName);
                    else if (sortType == UsersSortType.FirstNameDescendent)
                        usersQuery = usersQuery.OrderByDescending(u => u.FirstName);
                    else if (sortType == UsersSortType.LastNameAscendent)
                        usersQuery = usersQuery.OrderBy(u => u.LastName);
                    else if (sortType == UsersSortType.LastNameDescendent)
                        usersQuery = usersQuery.OrderByDescending(u => u.LastName);
                    else
                        usersQuery = usersQuery.OrderBy(u => u.FirstName);

                    usersQuery = usersQuery
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize);

                    var users = usersQuery.ToList();

                    return users;
                }
            }
        }

        [HttpGet]
        public ActionResult<User> GetById(int Id)
        {
            return _db.Users.Where(user => Id == user.Id).Single();
        }

        [HttpPost]
        public ActionResult<User> Create([FromBody] UserPayload payload)
        {
            try
            {
                var userToAdd = new User
                {
                    FirstName = payload.FirstName,
                    LastName = payload.LastName,
                    Email = payload.Email,
                    Gender = payload.Gender
                };

                _db.Users.Add(userToAdd);
                _db.SaveChanges();

                return Ok(userToAdd);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
               
            }
        }

        [HttpPost]
        public ActionResult<User> Update([FromBody] UserPayload payload)
        {
            try
            {
                if (payload.Id.HasValue)
                {
                    var userToUpdate = _db.Users.SingleOrDefault(user => payload.Id.Value == user.Id);

                    userToUpdate.FirstName = payload.FirstName;
                    userToUpdate.LastName = payload.LastName;
                    userToUpdate.Email = payload.Email;
                    userToUpdate.Gender = payload.Gender;

                    _db.SaveChanges();
                    return Ok(userToUpdate);
                }
                else
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int Id)
        {
            try
            {
                var userToDelete = _db.Users.Single(user => Id == user.Id);

                _db.Users.Remove(userToDelete);
                _db.SaveChanges();
                return Ok(new {status=true});
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
