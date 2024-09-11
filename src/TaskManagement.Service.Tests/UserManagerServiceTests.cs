using Moq;
using TaskManagement.Domain.Constants.Helpers;
using TaskManagement.Domain.Constants.User;
using TaskManagement.Domain.Contracts.Common;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Persistence.Interfaces;
using TaskManagement.Service.Implementations;
using TaskManagement.Service.Interfaces;
using User = TaskManagement.Domain.Entities.User;

namespace TaskManagement.Service.Tests
{
    public class UserManagerServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IJwtProvider> _jwtProviderMock;
        private Mock<IPasswordValidator> _passwordValidatorMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private IUserManagerService _userManagerService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _passwordValidatorMock = new Mock<IPasswordValidator>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _userManagerService = new UserManagerService(
                _userRepositoryMock.Object,
                _jwtProviderMock.Object,
                _passwordValidatorMock.Object,
                _passwordHasherMock.Object
            );
        }

        [Test]
        public async Task GIVEN_User_Manager_Service_WHEN_I_register_user_THEN_user_is_being_added_to_database()
        {
            //Arrange
            var username = "TestUser1";
            var email = "test-user@gmail.com";
            var password = "testuser12345";
            var hashedPassword = "hashedPassword";

            var expectedUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, default)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email, default)).ReturnsAsync((User?)null);
            _passwordValidatorMock.Setup(validator => validator.Validate(password)).Returns(Result.Success());
            _passwordHasherMock.Setup(hasher => hasher.HashPassword(password)).Returns(hashedPassword);

            //Act
            var result = await _userManagerService.RegisterAsync(username, email, password);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.Is<User>(u =>
                u.Username == username &&
                u.Email == email &&
                u.PasswordHash == hashedPassword),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GIVEN_User_Manager_Service_WHEN_I_register_user_with_existing_username_THEN_registration_fails()
        {
            //Arrange
            var username = "TestUser1";
            var email = "test-user@gmail.com";
            var passwordHash = "testuser12345";

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, default)).ReturnsAsync(user);

            //Act
            var result = await _userManagerService.RegisterAsync(username, email, passwordHash);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Errors, Contains.Item(UserManagerServiceErrors.UsernameAlreadyExists));
            });
        }

        [Test]
        public async Task GIVEN_User_Manager_Service_WHEN_I_register_user_with_existing_email_THEN_registrations_fails()
        {
            //Arrange
            var username = "TestUser1";
            var email = "test-user@gmail.com";
            var passwordHash = "testuser12345";

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, default)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email, default)).ReturnsAsync(user);

            //Act
            var result = await _userManagerService.RegisterAsync(username, email, passwordHash);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Errors, Contains.Item(UserManagerServiceErrors.EmailAlreadyExists));
            });
        }

        [Test]
        public async Task GIVEN_User_Manager_Service_WHEN_I_register_user_with_invalid_password_THEN_registration_fails()
        {
            //Arrange
            var username = "TestUser1";
            var email = "test-user@gmail.com";
            var passwordHash = "testuser12345";

            var expectedFailureValidationResult = Result.Failure(
                PasswordValidatonErrors.PasswordTooShort,
                PasswordValidatonErrors.PasswordRequiresUppercase,
                PasswordValidatonErrors.PasswordRequiresLowercase,
                PasswordValidatonErrors.PasswordRequiresDigit,
                PasswordValidatonErrors.PasswordRequiresSpecialCharacter
            );

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, default)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email, default)).ReturnsAsync((User?)null);
            _passwordValidatorMock.Setup(validator => validator.Validate(passwordHash)).Returns(expectedFailureValidationResult);

            //Act
            var result = await _userManagerService.RegisterAsync(username, email, passwordHash);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Errors, Is.EquivalentTo(expectedFailureValidationResult.Errors));
            });
        }

        [Test]
        public async Task GIVEN_User_Manager_Service_WHEN_I_login_user_with_valid_credentials_THEN_token_is_generated()
        {
            //Arrange
            var identifier = "TestUser1";
            var email = "test-user@gmail.com";
            var password = "testuser12345";
            var hashedPassword = "hashedPassword";
            var expectedJwtToken = "jwtToken";

            var user = new User
            {
                Username = identifier,
                Email = email,
                PasswordHash = hashedPassword
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(identifier, default)).ReturnsAsync(user);
            _passwordHasherMock.Setup(hasher => hasher.VerifyPassword(password, hashedPassword)).Returns(true);
            _jwtProviderMock.Setup(provider => provider.GenerateJwtToken(user)).Returns(expectedJwtToken);

            //Act
            var result = await _userManagerService.LoginAsync(identifier, password);
            Assert.That(result.Succeeded, Is.True);
            var tokenResponse = result.Payload;

            //Assert
            Assert.That(tokenResponse, Is.Not.Null);
            Assert.That(tokenResponse.accessToken, Is.EqualTo(expectedJwtToken));
        }

        [Test]
        public async Task GIVEN_User_Manager_Service_WHEN_I_login_with_invalid_credentials_THEN_login_fails()
        {
            //Assert
            var identifier = "test-user@gmail.com"; //use email as identifier => service accepts both username and email
            var password = "testuser12345";

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(identifier, default)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(identifier, default)).ReturnsAsync((User?)null);

            //Act
            var result = await _userManagerService.LoginAsync(identifier, password);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Errors, Contains.Item(UserManagerServiceErrors.UserNotFound));
            });
        }

        [Test]
        public async Task GIVEN_User_Manager_Service_WHEN_I_try_login_with_invalid_password_hash_THEN_login_fails()
        {
            //Arrange
            var username = "TestUser1";
            var email = "test-user@gmail.com";
            var password = "testuser12345";
            var hashedPassword = "hashedPassword";

            var user = new User()
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, default)).ReturnsAsync(user);
            _passwordHasherMock.Setup(hasher => hasher.VerifyPassword(password, hashedPassword)).Returns(false);

            //Act
            var result = await _userManagerService.LoginAsync(username, password);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Errors, Contains.Item(UserManagerServiceErrors.InvalidPassword));
            });
        }
    }
}
