using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using RoadReadyAPI.Services;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace RoadReadyTests
{
    [TestFixture]
    public class TokenServiceTests
    {
        private ITokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            // We create a fake in-memory configuration to provide our secret key for testing.
            var inMemorySettings = new Dictionary<string, string> {
                {"Tokens:JWT", "This is a test key that is long enough for the HMAC SHA256 algorithm"},
                {"Tokens:Issuer", "TestIssuer"},
                {"Tokens:Audience", "TestAudience"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Create the service instance with the fake configuration
            _tokenService = new TokenService(configuration);
        }

        [Test]
        public void GenerateToken_WhenGivenValidUser_ReturnsValidJwtTokenWithCorrectClaims()
        {
            // Arrange
            var user = new User
            {
                Id = 101,
                Email = "testuser@example.com",
                FirstName = "Test",
                UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = "Customer" } } }
            };

            // Act
            var tokenString = _tokenService.GenerateToken(user);

            // Assert
            // 1. Basic check to ensure a token was created
            Assert.IsNotNull(tokenString);
            Assert.IsNotEmpty(tokenString);

            // 2. Decode the token to verify its contents
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(tokenString);

            // 3. Check that all the claims we added are present and correct
            var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            var emailClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var roleClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            var givenNameClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "givenName")?.Value;

            Assert.AreEqual(user.Id.ToString(), userIdClaim);
            Assert.AreEqual(user.Email, emailClaim);
            Assert.AreEqual("Customer", roleClaim);
            Assert.AreEqual(user.FirstName, givenNameClaim);

            // 4. Verify Issuer and Audience are set correctly
            Assert.AreEqual("TestIssuer", decodedToken.Issuer);
            Assert.AreEqual("TestAudience", decodedToken.Audiences.FirstOrDefault());
        }
    }
}