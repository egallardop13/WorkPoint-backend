using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
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

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString =
                _config.GetSection("AppSettings:PasswordKey").Value
                + Convert.ToBase64String(passwordSalt);
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[] { new Claim("userId", userId.ToString()) };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

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
                Expires = DateTime.Now.AddDays(1),
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
                @"EXEC UsersSchema.spRegistration_Upsert
                        @Email = @EmailParam,
                        @PasswordHash = @PasswordHashParam, 
                        @PasswordSalt = @PasswordSaltParam";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            SqlParameter emailParamater = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            emailParamater.Value = userForSetPassword.Email;
            sqlParameters.Add(emailParamater);

            SqlParameter passwordSaltParamater = new SqlParameter(
                "@PasswordSaltParam",
                SqlDbType.VarBinary
            );
            passwordSaltParamater.Value = passwordSalt;
            sqlParameters.Add(passwordSaltParamater);

            SqlParameter passwordHashParamater = new SqlParameter(
                "@PasswordHashParam",
                SqlDbType.VarBinary
            );
            passwordHashParamater.Value = passwordHash;
            sqlParameters.Add(passwordHashParamater);

            return _dapper.ExecuteSqlWithParameter(sqlAddAuth, sqlParameters);
        }
    }
}
