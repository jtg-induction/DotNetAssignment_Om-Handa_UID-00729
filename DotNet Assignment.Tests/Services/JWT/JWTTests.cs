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
        public void GetRefreshToken_ReturnsToken()
        {
            var Token = _jWTService.GetRefreshToken();

            Assert.That(Token, Is.Not.Null);
            Assert.That(Token, Is.Not.Empty);
        }

        [Test]
        public void GetRefreshToken_ReturnsUniqueToken()
        {
            var Token1 = _jWTService.GetRefreshToken();
            var Token2 = _jWTService.GetRefreshToken();

            Assert.That(Token1, Is.Not.EqualTo(Token2));
        }
    }
}
