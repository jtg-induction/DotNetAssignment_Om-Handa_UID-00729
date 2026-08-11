using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Tests.Utils;
using NUnit.Framework;

namespace DotNet_Assignment.Tests.Services.JWT
{
    [TestFixture]
    public class JWTTests
    {
        private JWTService _jWTService;

        [SetUp]
        public void Setup()
        {
            _jWTService = new JWTService();
        }

        /// <summary>
        /// GetAccessToken function - returns new access token
        /// </summary>
        [Test]
        public void GetAccessToken_ReturnsToken()
        {
            var user = UserTestUtil.CreateMockUser();

            var token = _jWTService.GetAccessToken(user);

            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        }

        /// <summary>
        /// GenerateRefreshToken function - returns new refresh token
        /// </summary>
        [Test]
        public void GenerateRefreshToken_ReturnsToken()
        {
            var token = _jWTService.GenerateRefreshToken();

            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        }

        /// <summary>
        /// GenerateRefreshToken function - validates refresh tokens are unique
        /// </summary>
        [Test]
        public void GenerateRefreshToken_ReturnsUniqueToken()
        {
            var token1 = _jWTService.GenerateRefreshToken();
            var token2 = _jWTService.GenerateRefreshToken();

            Assert.That(token1, Is.Not.EqualTo(token2));
        }
    }
}
