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

        [Test]
        public void GetAccessToken_ReturnsToken()
        {
            var User = UserTestUtil.CreateMockUser();

            var Token = _jWTService.GetAccessToken(User);

            Assert.That(Token, Is.Not.Null);
            Assert.That(Token, Is.Not.Empty);
        }

        [Test]
        public void GenerateRefreshToken_ReturnsToken()
        {
            var Token = _jWTService.GenerateRefreshToken();

            Assert.That(Token, Is.Not.Null);
            Assert.That(Token, Is.Not.Empty);
        }

        [Test]
        public void GenerateRefreshToken_ReturnsUniqueToken()
        {
            var Token1 = _jWTService.GenerateRefreshToken();
            var Token2 = _jWTService.GenerateRefreshToken();

            Assert.That(Token1, Is.Not.EqualTo(Token2));
        }
    }
}
