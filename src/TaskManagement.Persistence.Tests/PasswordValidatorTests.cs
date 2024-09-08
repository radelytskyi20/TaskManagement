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
        private Mock<IOptions<PasswordComplexityOptions>> _passwordOptionsMock;

        [SetUp]
        public void Setup()
        {
            _passwordOptionsMock = new Mock<IOptions<PasswordComplexityOptions>>();
        }

        [Test, TestCaseSource(nameof(Data))]
        public void ValidateTest(string password, PasswordComplexityOptions passwordOptions, bool expectedSucceeded, List<string> expectedErrors)
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
                    new PasswordComplexityOptions //passwordOptions
                    {
                        MinimumLength = 6
                    },
                    false, //expectedSucceeded
                    new List<string> { PasswordValidatonErrors.PasswordTooShort } //expectedErrors
                },
                new object[]
                {
                    "password",
                    new PasswordComplexityOptions
                    {
                        RequireUppercase = true
                    },
                    false,
                    new List<string> { PasswordValidatonErrors.PasswordRequiresUppercase }
                },
                new object[]
                {
                    "PASSWORD",
                    new PasswordComplexityOptions
                    {
                        RequireLowercase = true
                    },
                    false,
                    new List<string> { PasswordValidatonErrors.PasswordRequiresLowercase }
                },
                new object[]
                {
                    "Password123",
                    new PasswordComplexityOptions
                    {
                        RequireSpecialCharacter = true
                    },
                    false,
                    new List<string> { PasswordValidatonErrors.PasswordRequiresSpecialCharacter }
                },
                new object[]
                {
                    "Password123/$/",
                    new PasswordComplexityOptions
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
