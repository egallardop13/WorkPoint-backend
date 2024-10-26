using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DotnetAPI.Controllers
{


    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        private readonly IConfiguration _config;

        public AuthController(IConfiguration config){
            _dapper = new DataContextDapper(config);
            _config = config;
        }


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

                    byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

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

                return Ok();
                    }
                    throw new Exception("Failed to add user");
                }   
                    throw new Exception("User with this email already exists");
            }
            throw new Exception("Passwords do not match");
        }
        


        [HttpPost("Login")]

        
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = "SELECT [PasswordHash], [PasswordSalt] FROM UsersSchema.Auth WHERE Email = '" + userForLogin.Email + "'";
            UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);
            
            byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);
            
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if(passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Password is incorrect");
                }
            }
            return Ok();
        }
        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
          string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);
          return KeyDerivation.Pbkdf2(
                        password: password,
                        salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8
                    );
        }


    }
}