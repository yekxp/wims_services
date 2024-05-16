using JwtManagerHandler.Models;
using user_managment.Model;

namespace user_managment.Services
{
    public interface IUserService
    {
        Task<AuthenticationResponse> Login(AuthenticationRequest authenticationRequest);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task Create(UserRequest model);
        Task Update(int id, UserUpdate model);
        Task Delete(int id);
    }
}
