using AutoMapper;
using JwtManagerHandler.Models;
using System.Security.Cryptography;
using System.Text;
using user_managment.Model;

namespace user_managment.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public async Task<AuthenticationResponse> Login(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrWhiteSpace(authenticationRequest.Username) || string.IsNullOrWhiteSpace(authenticationRequest.Password))
            {
                return null!;
            }

            authenticationRequest.Password = GetHashString(authenticationRequest.Password);

            return await _userRepository.Login(authenticationRequest);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User> GetById(int id)
        {
            var user = await _userRepository.GetById(id);

            return user == null ? throw new KeyNotFoundException("User not found") : user;
        }

        public async Task Create(UserRequest model)
        {
            // validate
            if (await _userRepository.GetByEmail(model.Email!) != null)
            {
                throw new Exception("User with the email '" + model.Email + "' already exists");
            }

            // map model to new user object
            var user = _mapper.Map<User>(model);

            // hash password
            user.PasswordHash = GetHashString(model.Password);

            // save user
            await _userRepository.Create(user);
        }

        public static byte[] GetHash(string inputString)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public async Task Update(int id, UserUpdate model)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var emailChanged = !string.IsNullOrEmpty(model.Email) && user.Email != model.Email;
            if (emailChanged && await _userRepository.GetByEmail(model.Email!) != null)
            {
                throw new Exception("User with the email '" + model.Email + "' already exists");
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = GetHashString(model.Password);
            }

            _mapper.Map(model, user);

            await _userRepository.Update(user);
        }

        public async Task Delete(int id)
        {
            await _userRepository.Delete(id);
        }
    }
}
