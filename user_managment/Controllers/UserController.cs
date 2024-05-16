using JwtManagerHandler;
using JwtManagerHandler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_managment.Model;
using user_managment.Services;

namespace user_managment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            var response = await _userService.Login(authenticationRequest);

            if (response is null)
            {
                return Unauthorized();
            }

            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> Create([FromBody] UserRequest model)
        {
            await _userService.Create(model);
            return Ok(new { message = "User created" });
        }

        [HttpPut("updateUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdate model)
        {
            await _userService.Update(id, model);
            return Ok(new { message = "User updated" });
        }

        [HttpDelete("deleteUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return Ok(new { message = "User deleted" });
        }

    }
}
