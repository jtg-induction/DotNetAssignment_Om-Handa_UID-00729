using DotNet_Assignment.Services.PasswordService;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace DotNet_Assignment.Tests.Services.Password
{
    [TestFixture]
    public class PasswordTests
    {
        private PasswordService _passwordService;

        [SetUp]
        public void Setup()
        {
            _passwordService = new PasswordService();
        }

        [Test]
        public void HashPassword_ReturnsHashedPassword()
        {
            var password = "Password";

            var Hash = _passwordService.HashPassword(password);

            Assert.That(Hash, Is.Not.Null);
            Assert.That(Hash, Is.Not.Empty);
            Assert.That(Hash, Is.Not.EqualTo(password));
        }

        [Test]
        public void HashPassword_ReturnsDifferentHashesForSamePassword()
        {
            var password = "Password";

            var Hash1 = _passwordService.HashPassword(password);
            var Hash2 = _passwordService.HashPassword(password);

            Assert.That(Hash1, Is.Not.EqualTo(Hash2));
        }

        [Test]
        public void VerifyPassword_ValidPassword_ReturnsTrue()
        {
            var password = "Password";

            var Hash = _passwordService.HashPassword(password);

            var result = _passwordService.VerifyPassword(password, Hash);

            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyPassword_InvalidPassword_ReturnsFalse()
        {
            var Hash = _passwordService.HashPassword("Password");

            var result = _passwordService.VerifyPassword("WrongPassword", Hash);

            Assert.That(result, Is.False);
        }

    }
}
