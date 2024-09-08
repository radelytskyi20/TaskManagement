using Microsoft.Extensions.Options;
using Moq;
using TaskManagement.Domain.Constants;
using TaskManagement.Domain.Contracts.Auth;
using TaskManagement.Persistence.Helpers;

namespace TaskManagement.Persistence.Tests
{
    [TestFixture]
    public class PasswordValidatorTests
    {
        private Mock<IOptions<PasswordOptions>> _passwordOptionsMock;

        [SetUp]
        public void Setup()
        {
            _passwordOptionsMock = new Mock<IOptions<PasswordOptions>>();
        }

        [Test, TestCaseSource(nameof(Data))]
        public void ValidateTest(string password, PasswordOptions passwordOptions, bool expectedSucceeded, List<string> expectedErrors)
        {
            //Arrange
            _passwordOptionsMock.Setup(p => p.Value).Returns(passwordOptions);
            var passwordValidator = new PasswordValidator(_passwordOptionsMock.Object);

            //Act
            var result = passwordValidator.Validate(password);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.EqualTo(expectedSucceeded));
                Assert.That(result.Errors, Is.EquivalentTo(expectedErrors));
            });
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[]
                {
                    "abc", //password
                    new PasswordOptions //passwordOptions
                    {
                        MinimumLength = 6
                    },
                    false, //expectedSucceeded
                    new List<string> { PasswordValidatonErrors.PasswordTooShort } //expectedErrors
                },
                new object[]
                {
                    "password",
                    new PasswordOptions
                    {
                        RequireUppercase = true
                    },
                    false,
                    new List<string> { PasswordValidatonErrors.PasswordRequiresUppercase }
                },
                new object[]
                {
                    "PASSWORD",
                    new PasswordOptions
                    {
                        RequireLowercase = true
                    },
                    false,
                    new List<string> { PasswordValidatonErrors.PasswordRequiresLowercase }
                },
                new object[]
                {
                    "Password123",
                    new PasswordOptions
                    {
                        RequireSpecialCharacter = true
                    },
                    false,
                    new List<string> { PasswordValidatonErrors.PasswordRequiresSpecialCharacter }
                },
                new object[]
                {
                    "Password123/$/",
                    new PasswordOptions
                    {
                        MinimumLength = 6,
                        RequireUppercase = true,
                        RequireLowercase = true,
                        RequireDigit = true,
                        RequireSpecialCharacter = true
                    },
                    true,
                    new List<string>()
                }
            };
    }
}
