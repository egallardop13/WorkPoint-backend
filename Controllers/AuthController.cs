using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using DotnetAPI.Helpers;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config){
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]

        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if(userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM UsersSchema.Auth WHERE Email = '" +
                 userForRegistration.Email + "'";
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if(existingUsers.Count() == 0)
                {
                    byte [] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = "INSERT INTO UsersSchema.Auth([Email], [PasswordHash], [PasswordSalt]) VALUES ('" + userForRegistration.Email + "', @PasswordHash, @PasswordSalt)";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParamater = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParamater.Value = passwordSalt;

                    SqlParameter passwordHashParamater = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParamater.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParamater);
                    sqlParameters.Add(passwordHashParamater);


                    if(_dapper.ExecuteSqlWithParameter(sqlAddAuth, sqlParameters))
                    {
                       string sqlAddUser = @"INSERT INTO UsersSchema.Users(
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active]    
) VALUES (" +
        "'" + userForRegistration.FirstName + 
        "', '" + userForRegistration.LastName + 
        "', '" + userForRegistration.Email + 
        "', '" + userForRegistration.Gender +
        "', 1)";
                    if(_dapper.ExecuteSql(sqlAddUser))
                    {
                return Ok();
                    }
                    }
                    throw new Exception("Failed to add user");
                }   
                    throw new Exception("User with this email already exists");
            }
            throw new Exception("Passwords do not match");
        }
        

        [AllowAnonymous]
        [HttpPost("Login")]

        
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = "SELECT [PasswordHash], [PasswordSalt] FROM UsersSchema.Auth WHERE Email = '" + userForLogin.Email + "'";
            UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);
            
            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);
            
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if(passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Password is incorrect");
                }
            }

            string sqlForUserId = @"
            SELECT UserId FROM UsersSchema.Users WHERE Email = '" +
                userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(sqlForUserId);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId)}
                });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";
            string userIdSql = "SELECT UserId FROM UsersSchema.Users WHERE UserId = " + userId;

            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);
            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userIdFromDB)}
            });
        }


    }
}