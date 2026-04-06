using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;

        public AuthHelper(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public bool SaveRefreshToken(int userId, string token, DateTime expiresAt)
        {
            string sql =
                @"INSERT INTO WorkPointSchema.RefreshTokens (UserId, Token, ExpiresAt, CreatedAt)
                VALUES (@UserIdParam, @TokenParam, @ExpiresAtParam, GETUTCDATE())";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            sqlParameters.Add("@TokenParam", token, DbType.String);
            sqlParameters.Add("@ExpiresAtParam", expiresAt, DbType.DateTime2);

            return _dapper.ExecuteSqlWithParameter(sql, sqlParameters);
        }

        public RefreshToken? GetValidRefreshToken(string token)
        {
            string sql =
                @"SELECT Id, UserId, Token, ExpiresAt, CreatedAt, RevokedAt
                FROM WorkPointSchema.RefreshTokens
                WHERE Token = @TokenParam
                    AND RevokedAt IS NULL
                    AND ExpiresAt > GETUTCDATE()";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@TokenParam", token, DbType.String);

            try
            {
                return _dapper.LoadDataSingleWithParameters<RefreshToken>(sql, sqlParameters);
            }
            catch
            {
                return null;
            }
        }

        public bool RevokeRefreshToken(string token)
        {
            string sql =
                @"UPDATE WorkPointSchema.RefreshTokens
                SET RevokedAt = GETUTCDATE()
                WHERE Token = @TokenParam AND RevokedAt IS NULL";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@TokenParam", token, DbType.String);

            return _dapper.ExecuteSqlWithParameter(sql, sqlParameters);
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt, int iterations = 600000)
        {
            string passwordSaltPlusString =
                _config.GetSection("AppSettings:PasswordKey").Value
                + Convert.ToBase64String(passwordSalt);
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: iterations,
                numBytesRequested: 256 / 8
            );
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[] { new Claim("userId", userId.ToString()) };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
            string issuer =
                _config.GetValue<string>("AppSettings:Issuer") ?? "WorkPointAPI";
            string audience =
                _config.GetValue<string>("AppSettings:Audience") ?? "WorkPointClient";

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString : "")
            );

            SigningCredentials credentials = new SigningCredentials(
                tokenKey,
                SecurityAlgorithms.HmacSha512Signature
            );

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = issuer,
                Audience = audience,
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool SetPassword(UserForLoginDto userForSetPassword)
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

            string sqlAddAuth =
                @"EXEC WorkPointSchema.spRegistration_Upsert
                        @Email = @EmailParam,
                        @PasswordHash = @PasswordHashParam, 
                        @PasswordSalt = @PasswordSaltParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@EmailParam", userForSetPassword.Email, DbType.String);
            sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

            return _dapper.ExecuteSqlWithParameter(sqlAddAuth, sqlParameters);
        }
    }
}
