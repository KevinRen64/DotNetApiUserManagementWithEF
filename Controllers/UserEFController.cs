using DotNetApi.Data;
using DotNetApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using DotNetApi.Models;
using AutoMapper;
using System.Linq.Expressions;

namespace DotNetApi.Controllers;

[ApiController]  // Indicates this class defines a Web API controller
[Route("[controller]")]  // Route will be based on the controller name: /User
public class UserEFController : ControllerBase
{

  // Dependency for interacting with the database via Entity Framework
  DataContextEF _entityFramework;

  // Dependency for mapping DTOs to models
  IMapper _mapper;

  // Constructor initializes the EF data context and AutoMapper configuration
  public UserEFController(IConfiguration config)
  {
    // Initialize EF Core context with configuration (connection string)
    _entityFramework = new DataContextEF(config);

    // Configure AutoMapper to map from UserToAddDto to User entity
    _mapper = new Mapper(new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<UserToAddDto, User>();
    }));
  }


  // GET: /UserEF/GetUsers
  // Retrieves and returns all users from the database
  [HttpGet("GetUsers")]
  public IEnumerable<User> GetUsers()
  {
    IEnumerable<User> users = _entityFramework.Users.ToList<User>();
    return users;
  }


  // GET: /UserEF/GetUsers/{userId}
  // Retrieves a single user by userId
  [HttpGet("GetUsers/{userId}")]
  public User GetSingleUser(int userId)
  {

    User? user = _entityFramework.Users
        .Where(u => u.UserId == userId)
        .FirstOrDefault<User>();
    if (user != null)
    {
      return user;
    }

    throw new Exception("Failed to Get User");
  }



  // PUT: /UserEF/EditUser
  // Updates an existing user in the database
  [HttpPut("EditUser")]
  public IActionResult EditUser(User user)
  {
    // Look for the user in the database
    User? userDb = _entityFramework.Users
        .Where(u => u.UserId == user.UserId)
        .FirstOrDefault<User>();

    if (userDb != null)
    {
      // Update fields
      userDb.Active = user.Active;
      userDb.FirstName = user.FirstName;
      userDb.LastName = user.LastName;
      userDb.Email = user.Email;
      userDb.Gender = user.Gender;

      // Save changes to the database
      if (_entityFramework.SaveChanges() > 0)
      {
        return Ok();
      }

      throw new Exception("Failed to Update User");
    }

    throw new Exception("Failed to Get User");

  }



  // POST: /UserEF/AddUser
  // Adds a new user to the database using data from a DTO
  [HttpPost("AddUser")]
  public IActionResult AddUser(UserToAddDto user)
  {

    // Map the DTO to the User entity
    User userDb = _mapper.Map<User>(user);

    // Add the new user and save changes
    _entityFramework.Add(userDb);
    if (_entityFramework.SaveChanges() > 0)
    {
      return Ok();
    }

    throw new Exception("Failed to Update User");
  }



  // DELETE: /UserEF/DeleteUser/{userId}
  // Deletes a user from the database by userId
  [HttpDelete("DeleteUser/{userId}")]
  public IActionResult DeleteUser(int userId)
  {
    // Find the user
    User? userDb = _entityFramework.Users
        .Where(u => u.UserId == userId)
        .FirstOrDefault<User>();

    if (userDb != null)
    {
      // Remove and save changes
      _entityFramework.Remove(userDb);
      if (_entityFramework.SaveChanges() > 0)
      {
        return Ok();
      }

      throw new Exception("Failed to Delete User");
    }
    throw new Exception("Failed to Get User");
  }
}