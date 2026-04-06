using System.Data;
using System.Security.Cryptography;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        private readonly ReusableSql _reusableSql;

        private readonly IMapper _mapper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _reusableSql = new ReusableSql(config);
            _mapper = new Mapper(
                new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserForRegistrationDto, UserComplete>();
                })
            );
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists =
                    "SELECT Email FROM WorkPointSchema.Auth WHERE Email = @EmailParam";
                DynamicParameters checkParams = new DynamicParameters();
                checkParams.Add("@EmailParam", userForRegistration.Email, DbType.String);
                IEnumerable<string> existingUsers = _dapper.LoadDataWithParameters<string>(
                    sqlCheckUserExists,
                    checkParams
                );
                if (existingUsers.Count() == 0)
                {
                    UserForLoginDto userForSetPassword = new UserForLoginDto
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password,
                    };
                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
                        userComplete.Active = true;

                        if (_reusableSql.UpsertUser(userComplete).Response == 1)
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

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to reset password");
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt =
                @"EXEC WorkPointSchema.spLoginConfirmation_Get
                @Email = @EmailParam";
            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            UserForLoginConfirmationDto userForLoginConfirmation =
                _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(
                    sqlForHashAndSalt,
                    sqlParameters
                );

            byte[] passwordHash = _authHelper.GetPasswordHash(
                userForLogin.Password,
                userForLoginConfirmation.PasswordSalt
            );

            bool passwordValid = CryptographicOperations.FixedTimeEquals(
                passwordHash,
                userForLoginConfirmation.PasswordHash
            );

            if (!passwordValid)
            {
                byte[] legacyHash = _authHelper.GetPasswordHash(
                    userForLogin.Password,
                    userForLoginConfirmation.PasswordSalt,
                    100000
                );
                passwordValid = CryptographicOperations.FixedTimeEquals(
                    legacyHash,
                    userForLoginConfirmation.PasswordHash
                );

                if (passwordValid)
                {
                    _authHelper.SetPassword(
                        new UserForLoginDto
                        {
                            Email = userForLogin.Email,
                            Password = userForLogin.Password,
                        }
                    );
                }
            }

            if (!passwordValid)
            {
                return StatusCode(401, "Password is incorrect");
            }

            string sqlForUserId =
                "SELECT UserId FROM WorkPointSchema.Users WHERE Email = @EmailParam";
            DynamicParameters userIdParams = new DynamicParameters();
            userIdParams.Add("@EmailParam", userForLogin.Email, DbType.String);
            int userId = _dapper.LoadDataSingleWithParameters<int>(sqlForUserId, userIdParams);

            string accessToken = _authHelper.CreateToken(userId);
            string refreshToken = _authHelper.GenerateRefreshToken();
            _authHelper.SaveRefreshToken(userId, refreshToken, DateTime.UtcNow.AddDays(7));

            return Ok(
                new Dictionary<string, string>
                {
                    { "token", accessToken },
                    { "refreshToken", refreshToken },
                }
            );
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public IActionResult Refresh([FromBody] Dictionary<string, string> body)
        {
            if (!body.TryGetValue("refreshToken", out string? token) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Refresh token is required");
            }

            var storedToken = _authHelper.GetValidRefreshToken(token);
            if (storedToken == null)
            {
                return StatusCode(401, "Invalid or expired refresh token");
            }

            _authHelper.RevokeRefreshToken(token);

            string newAccessToken = _authHelper.CreateToken(storedToken.UserId);
            string newRefreshToken = _authHelper.GenerateRefreshToken();
            _authHelper.SaveRefreshToken(
                storedToken.UserId,
                newRefreshToken,
                DateTime.UtcNow.AddDays(7)
            );

            return Ok(
                new Dictionary<string, string>
                {
                    { "token", newAccessToken },
                    { "refreshToken", newRefreshToken },
                }
            );
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";
            string userIdSql =
                "SELECT UserId FROM WorkPointSchema.Users WHERE UserId = @UserIdParam";
            DynamicParameters refreshParams = new DynamicParameters();
            refreshParams.Add("@UserIdParam", userId, DbType.Int32);
            int userIdFromDB = _dapper.LoadDataSingleWithParameters<int>(
                userIdSql,
                refreshParams
            );
            return Ok(
                new Dictionary<string, string>
                {
                    { "token", _authHelper.CreateToken(userIdFromDB) },
                }
            );
        }
    }
}
