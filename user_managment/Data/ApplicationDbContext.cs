using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace user_managment.Data
{
    public class ApplicationDbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("SqlServer");
            return new SqlConnection(connectionString);
        }

        public async Task Init()
        {
            await _initDatabase();
            await _initTables();
        }

        private async Task _initDatabase()
        {
            var connectionString = _configuration.GetConnectionString("SqlServer");
            using var connection = new SqlConnection(connectionString);
            var sql = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'wims-um') CREATE DATABASE [wims-um];";
            await connection.ExecuteAsync(sql);
        }

        private async Task _initTables()
        {
            using var connection = CreateConnection();
            await _initUsers();

            async Task _initUsers()
            {
                var sql = """
                IF OBJECT_ID('Users', 'U') IS NULL
                CREATE TABLE Users (
                    Id INT NOT NULL PRIMARY KEY IDENTITY,
                    Name NVARCHAR(MAX),
                    Username NVARCHAR(MAX),
                    Surname NVARCHAR(MAX),
                    Email NVARCHAR(MAX),
                    Role INT,
                    PasswordHash NVARCHAR(MAX)
                );
            """;
                await connection.ExecuteAsync(sql);
            }
        }

    }
}
