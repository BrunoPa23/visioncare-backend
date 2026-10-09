using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VisionCareCore.Shared.Infraestructure.Persistences.EFC.Configuration;
using VisionCareCore.User.Application.Internal.OutboundServices;
using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Repositories;
using VisionCareCore.User.Infraestructure.Tokens.JWT.Configurations;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace VisionCareCore.User.Infraestructure.Tokens.JWT.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenSettings _tokenSettings;
        private readonly IAuthUserRefreshTokenRepository _refreshTokenRepository;
        private readonly AppDbContext _context; 

        public TokenService(
            IOptions<TokenSettings> tokenSettings,
            IAuthUserRefreshTokenRepository refreshTokenRepository,
            AppDbContext context
            ) 
        {
            _tokenSettings = tokenSettings?.Value ?? throw new ArgumentNullException(nameof(tokenSettings));
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Generate a new JWT token
        /// </summary>
        public string GenerateToken(AuthUser user)
        {
            var secret = _tokenSettings.Secret;
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // ✅ Reemplaza ClaimTypes.Sid por sub
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("visualImpairment", user.VisualImpairment.ToString())
                }),
                Issuer = _tokenSettings.Issuer,
                Audience = _tokenSettings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        
        /// <summary>
        /// Validate the JWT token
        /// </summary>
        public (Guid? UserId, string Role) ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return (null, null);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _tokenSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _tokenSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = jwtToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
                var role = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;

                return (Guid.Parse(userId), role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la validación del token: {ex.Message}");
                return (null, null);
            }
        }




        /// <summary>
        /// Generate a secure Refresh Token
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }   

        /// <summary>
        /// Validate Refresh Token
        /// </summary>
        public async Task<Guid?> ValidateRefreshToken(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(HashRefreshToken(refreshToken));

            if (storedToken == null)
            {
                Console.WriteLine(" Refresh token no encontrado.");
                return null;
            }

            if (storedToken.ExpiryDate < DateTime.UtcNow)
            {
                Console.WriteLine($" El refresh token del usuario {storedToken.UserId} ha expirado.");
                return null;
            }

            return storedToken.UserId;
        }

        /// <summary>
        /// Only the SHA-256 hash of a refresh token is stored, so a leaked database
        /// does not expose tokens that can be used against refresh-token.
        /// </summary>
        private static string HashRefreshToken(string refreshToken) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));


        /// <summary>
        /// Store Refresh Token
        /// </summary>
        public async Task StoreRefreshToken(Guid userId, string refreshToken)
        {
            var existingToken = await _refreshTokenRepository.GetByUserIdAsync(userId);

            if (existingToken != null)
            {
                existingToken.Token = HashRefreshToken(refreshToken);
                existingToken.ExpiryDate = DateTime.UtcNow.AddDays(30);
                await _refreshTokenRepository.UpdateAsync(existingToken); // Asegurar que se actualiza
            }
            else
            {
                var newRefreshToken = new AuthUserRefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Token = HashRefreshToken(refreshToken),
                    ExpiryDate = DateTime.UtcNow.AddDays(30)
                };
                await _refreshTokenRepository.AddAsync(newRefreshToken);
            }
        }






        /// <summary>
        /// Revoke Refresh Token (Logout)
        /// </summary>
        public async Task RevokeRefreshToken(string refreshToken)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(HashRefreshToken(refreshToken));

            if (existingToken != null)
            {
                await _refreshTokenRepository.RevokeAsync(existingToken);
            }
        }
    }
}
