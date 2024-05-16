using Dapper;
using JwtManagerHandler;
using JwtManagerHandler.Models;
using user_managment.Data;
using user_managment.Model;

namespace user_managment.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context, JwtTokenHandler jwtTokenHandler)
        {
            _context = context;
            _jwtTokenHandler = jwtTokenHandler;
        }

        public async Task<AuthenticationResponse> Login(AuthenticationRequest authenticationRequest)
        {
            using var connection = _context.CreateConnection();

            var sql = """
                SELECT * FROM Users
                WHERE Username = @username AND PasswordHash = @password
                """;
            User? user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { authenticationRequest.Username, authenticationRequest.Password });         

            if (user is null) 
                return null!;

            authenticationRequest.Role = (JwtManagerHandler.Models.Role)user.Role;

            return _jwtTokenHandler.GenerateJwtToken(authenticationRequest)!;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var sql = """
            SELECT * FROM Users
        """;
            return await connection.QueryAsync<User>(sql);
        }

        public async Task<User> GetById(int id)
        {
            using var connection = _context.CreateConnection();
            var sql = """
            SELECT * FROM Users 
            WHERE Id = @id
        """;
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { id });
        }

        public async Task<User> GetByEmail(string email)
        {
            using var connection = _context.CreateConnection();
            var sql = """
            SELECT * FROM Users 
            WHERE Email = @email
        """;
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { email });

        }

        public async Task Create(User user)
        {
            using var connection = _context.CreateConnection();
            var sql = """
            INSERT INTO Users (Username, Name, Surname, Email, Role, PasswordHash)
            VALUES (@Username, @Name, @Surname, @Email, @Role, @PasswordHash)
        """;
            await connection.ExecuteAsync(sql, user);
        }

        public async Task Update(User user)
        {
            using var connection = _context.CreateConnection();
            var sql = """
            UPDATE Users 
            SET Title = @Title,
                FirstName = @FirstName,
                LastName = @LastName, 
                Email = @Email, 
                Role = @Role, 
                PasswordHash = @PasswordHash
            WHERE Id = @Id
        """;
            await connection.ExecuteAsync(sql, user);
        }

        public async Task Delete(int id)
        {
            using var connection = _context.CreateConnection();
            var sql = """
            DELETE FROM Users 
            WHERE Id = @id
        """;
            await connection.ExecuteAsync(sql, new { id });
        }
    }
}
