using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TechnicalChallenge.BusinessService;
using TechnicalChallenge.BusinessService.DataTransferObjects;
using TechnicalChallenge.Common;
using TechnicalChallenge.Data.Domain;
using TechnicalChallenge.Data.Respository;

namespace TechnicalChallenge.Tests.BusinessService
{
    public class CustomerServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private Mock<IRepository<Customer>> _mockCustomerRepository;
        private Mock<IRepository<Account>> _mockAccountRepository;
        private Mock<ILogger<CustomerService>> _mockLogger;
        private CustomerService _customerService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockCustomerRepository = new Mock<IRepository<Customer>>();
            _mockAccountRepository = new Mock<IRepository<Account>>();
            _mockLogger = new Mock<ILogger<CustomerService>>();

            _mockUnitOfWork.Setup(u => u.GetRepository<Customer>()).Returns(_mockCustomerRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<Account>()).Returns(_mockAccountRepository.Object);

            _customerService = new CustomerService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        #region CONSTRUCTOR TESTS

        [Test]
        public void Constructor_WithValidParameters_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new CustomerService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object));
        }

        [Test]
        public void Constructor_WithNullUnitOfWork_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CustomerService(null, _mockMapper.Object, _mockLogger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("unitOfWork"));
        }

        [Test]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CustomerService(_mockUnitOfWork.Object, null, _mockLogger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CustomerService(_mockUnitOfWork.Object, _mockMapper.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        #endregion

        #region GetByNameAsync TESTS

        [Test]
        public async Task GetByNameAsync_CustomerExists_ReturnsCustomerDto()
        {
            // Arrange
            var customerName = "John";
            var customer = new Customer { FirstName = customerName, LastName = "Doe" };
            var customerDto = new CustomerDto { FirstName = customerName, LastName = "Doe" };

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerDto>(customer)).Returns(customerDto);

            // Act
            var result = await _customerService.GetByNameAsync(customerName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(customerDto);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting customer by name: {customerName}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Customer with name {customerName} found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetByNameAsync_CustomerDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var customerName = "John";

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.GetByNameAsync(customerName);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ResultError.RecordNotFound);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting customer by name: {customerName}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Customer with name {customerName} not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetByNameAsync_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var customerName = "John";

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerService.GetByNameAsync(customerName);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Error.GetByName");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting customer by name: {customerName}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while getting customer by name: {customerName}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region GetAccountsAsync TESTS

        [Test]
        public async Task GetAccountsAsync_AccountsExist_ReturnsAccountDtoList()
        {
            // Arrange
            var customerNumber = Guid.NewGuid();
            var accounts = new List<Account>
            {
                new Account { AccountNumber="12345678", AccountId = Guid.NewGuid(), Balance = 1000, AccountType = new Data.Domain.AccountType { Name = "Savings" } },
                new Account { AccountNumber="23456789", AccountId = Guid.NewGuid(), Balance = 2000, AccountType = new Data.Domain.AccountType { Name = "Current" } }
            };
            var accountDtos = new List<AccountDto>
            {
                new AccountDto { AccountNumber="12345678", AccountId = accounts[0].AccountId, Balance = 1000, AccountType = new AccountTypeDto { Name = "Savings" } },
                new AccountDto { AccountNumber="23456789", AccountId = accounts[1].AccountId, Balance = 2000, AccountType = new AccountTypeDto { Name = "Current" } }
            };

            _mockAccountRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Expression<Func<Account, object>>[]>()))
                .ReturnsAsync(accounts);
            _mockMapper.Setup(m => m.Map<List<AccountDto>>(accounts)).Returns(accountDtos);

            // Act
            var result = await _customerService.GetAccountsAsync(customerNumber);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(accountDtos);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting accounts for customer number: {customerNumber}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Accounts for customer number {customerNumber} found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetAccountsAsync_AccountsDoNotExist_ReturnsFailure()
        {
            // Arrange
            var customerNumber = Guid.NewGuid();

            _mockAccountRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Expression<Func<Account, object>>[]>()))
                .ReturnsAsync((IEnumerable<Account>)null);

            // Act
            var result = await _customerService.GetAccountsAsync(customerNumber);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ResultError.RecordNotFound);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting accounts for customer number: {customerNumber}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Accounts for customer number {customerNumber} not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetAccountsAsync_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var customerNumber = Guid.NewGuid();

            _mockAccountRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Expression<Func<Account, object>>[]>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerService.GetAccountsAsync(customerNumber);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Error.GetAccounts");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting accounts for customer number: {customerNumber}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while getting accounts for customer number: {customerNumber}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region TransferFunds TESTS

        [Test]
        public async Task TransferFunds_SuccessfulTransfer_ReturnsSuccess()
        {
            // Arrange
            var sourceAccountId = Guid.NewGuid();
            var targetAccountId = Guid.NewGuid();
            var amount = 100m;
            var sourceAccount = new Account { AccountId = sourceAccountId, Balance = 200m };
            var targetAccount = new Account { AccountId = targetAccountId, Balance = 50m };

            _mockAccountRepository.SetupSequence(r => r.FirstAsync(It.IsAny<Expression<Func<Account, bool>>>(), null))
                .ReturnsAsync(sourceAccount)
                .ReturnsAsync(targetAccount);

            // Act
            var result = await _customerService.TransferFunds(sourceAccountId, targetAccountId, amount);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.SourceAccountBalance.Should().Be(100m);
            result.Value.TargetAccountBalance.Should().Be(150m);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Transferring {amount} from account {sourceAccountId} to account {targetAccountId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Transferred {amount} from account {sourceAccountId} to account {targetAccountId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task TransferFunds_SourceOrTargetAccountNotFound_ReturnsFailure()
        {
            // Arrange
            var sourceAccountId = Guid.NewGuid();
            var targetAccountId = Guid.NewGuid();
            var amount = 100m;

            _mockAccountRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Account, bool>>>(), null))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _customerService.TransferFunds(sourceAccountId, targetAccountId, amount);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("AccountNotFound");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Transferring {amount} from account {sourceAccountId} to account {targetAccountId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Source or target account not found for transfer")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task TransferFunds_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var sourceAccountId = Guid.NewGuid();
            var targetAccountId = Guid.NewGuid();
            var amount = 100m;

            _mockAccountRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Account, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerService.TransferFunds(sourceAccountId, targetAccountId, amount);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Error.TransferFunds");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Transferring {amount} from account {sourceAccountId} to account {targetAccountId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while transferring {amount} from account {sourceAccountId} to account {targetAccountId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion
    }
}