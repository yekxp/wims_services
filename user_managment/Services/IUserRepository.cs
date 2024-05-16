using JwtManagerHandler.Models;
using user_managment.Model;

namespace user_managment.Services
{
    public interface IUserRepository
    {
        Task<AuthenticationResponse> Login(AuthenticationRequest authenticationRequest);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByEmail(string email);
        Task Create(User user);
        Task Update(User user);
        Task Delete(int id);
    }
}
