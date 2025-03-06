using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using TechnicalChallenge.BusinessService;
using TechnicalChallenge.BusinessService.DataTransferObjects;
using TechnicalChallenge.Common;
using TechnicalChallenge.Data.Domain;
using TechnicalChallenge.Data.Respository;

namespace TechnicalChallenge.Tests.BusinessService
{
    public class LoanServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private Mock<IRepository<LoanRate>> _mockLoanRateRepository;
        private Mock<IRepository<Customer>> _mockCustomerRepository;
        private Mock<IRepository<Account>> _mockAccountRepository;
        private Mock<ILogger<LoanService>> _mockLogger;
        private LoanService _loanService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLoanRateRepository = new Mock<IRepository<LoanRate>>();
            _mockCustomerRepository = new Mock<IRepository<Customer>>();
            _mockAccountRepository = new Mock<IRepository<Account>>();
            _mockLogger = new Mock<ILogger<LoanService>>();

            _mockUnitOfWork.Setup(u => u.GetRepository<LoanRate>()).Returns(_mockLoanRateRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<Customer>()).Returns(_mockCustomerRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<Account>()).Returns(_mockAccountRepository.Object);

            _loanService = new LoanService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        #region CONSTRUCTOR TESTS

        [Test]
        public void Constructor_WithValidParameters_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new LoanService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object));
        }

        [Test]
        public void Constructor_WithNullUnitOfWork_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new LoanService(null, _mockMapper.Object, _mockLogger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("unitOfWork"));
        }

        [Test]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new LoanService(_mockUnitOfWork.Object, null, _mockLogger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new LoanService(_mockUnitOfWork.Object, _mockMapper.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        #endregion

        #region GetLoanDurations TESTS

        [Test]
        public async Task GetLoanDurations_RatesExist_ReturnsUniqueDurations()
        {
            // Arrange
            var loanRates = new List<LoanRate>
            {
                new LoanRate { Duration = 12 },
                new LoanRate { Duration = 24 },
                new LoanRate { Duration = 12 }
            };

            _mockLoanRateRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync(loanRates);

            // Act
            var result = await _loanService.GetLoanDurations();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(new List<byte> { 12, 24 });
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Getting loan durations")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Loan durations retrieved successfully")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetLoanDurations_NoRatesFound_ReturnsFailure()
        {
            // Arrange
            _mockLoanRateRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync((IEnumerable<LoanRate>)null);

            // Act
            var result = await _loanService.GetLoanDurations();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ResultError.RecordNotFound);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Getting loan durations")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No loan rates found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetLoanDurations_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockLoanRateRepository.Setup(r => r.GetAllAsync(null)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _loanService.GetLoanDurations();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Error.GetLoanDurations");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Getting loan durations")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while getting loan durations")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region GetLoanRate TESTS

        [Test]
        public async Task GetLoanRate_CustomerAndRateExist_ReturnsLoanRateDto()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var duration = 12;
            var customer = new Customer { CustomerNumber = customerId, CreditScore = 4 };
            var loanRate = new LoanRate { Duration = (byte)duration, RatingFrom = 20, RatingTo = 50, Rate = 5 };
            var loanRateDto = new LoanRateDto { Duratiion = (byte)duration, RatingFrom = 20, RatingTo = 50, Rate = 5 };

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ReturnsAsync(customer);
            _mockLoanRateRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<LoanRate, bool>>>(), null))
                .ReturnsAsync(loanRate);
            _mockMapper.Setup(m => m.Map<LoanRateDto>(loanRate)).Returns(loanRateDto);

            // Act
            var result = await _loanService.GetLoanRate(customerId, duration);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(loanRateDto);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting loan rate for customer {customerId} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Loan rate retrieved successfully for customer {customerId} with duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetLoanRate_CustomerNotFound_ReturnsFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var duration = 12;

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _loanService.GetLoanRate(customerId, duration);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ResultError.RecordNotFound);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting loan rate for customer {customerId} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Customer with ID {customerId} not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetLoanRate_RateNotFound_ReturnsFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var duration = 12;
            var customer = new Customer { CustomerNumber = customerId, CreditScore = 50 };

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ReturnsAsync(customer);
            _mockLoanRateRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<LoanRate, bool>>>(), null))
                .ReturnsAsync((LoanRate)null);

            // Act
            var result = await _loanService.GetLoanRate(customerId, duration);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ResultError.RecordNotFound);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting loan rate for customer {customerId} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Loan rate not found for customer {customerId} with duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetLoanRate_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var duration = 12;

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _loanService.GetLoanRate(customerId, duration);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Error.GetLoanRate");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Getting loan rate for customer {customerId} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while getting loan rate for customer {customerId} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion


        #region ProcessLoan TESTS

        [Test]
        public async Task ProcessLoan_SuccessfulLoanProcessing_ReturnsSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var targetAccountId = Guid.NewGuid();
            var amount = 1000m;
            var duration = 12;
            var customer = new Customer { Id = 1, CustomerNumber = customerId, CreditScore = 50 };
            var account = new Account { AccountId = targetAccountId, CustomerId = 1, Balance = 500m };
            var loanRate = new LoanRate { Duration = (byte)duration, RatingFrom = 20, RatingTo = 50, Rate = 5 };

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ReturnsAsync(customer);
            _mockAccountRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Account, bool>>>(), null))
                .ReturnsAsync(account);
            _mockLoanRateRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<LoanRate, bool>>>(), null))
                .ReturnsAsync(loanRate);

            // Act
            var result = await _loanService.ProcessLoan(amount, customerId, duration, targetAccountId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            account.Balance.Should().Be(1500m);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Processing loan for customer {customerId} with amount {amount} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Loan processed successfully for customer {customerId} with amount {amount} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task ProcessLoan_InvalidAccount_ReturnsFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var targetAccountId = Guid.NewGuid();
            var amount = 1000m;
            var duration = 12;
            var customer = new Customer { Id = 1, CustomerNumber = customerId, CreditScore = 45 };

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ReturnsAsync(customer);
            _mockAccountRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Account, bool>>>(), null))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _loanService.ProcessLoan(amount, customerId, duration, targetAccountId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("InvalidAccount");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Processing loan for customer {customerId} with amount {amount} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Invalid account for customer {customerId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task ProcessLoan_BadRating_ReturnsFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var targetAccountId = Guid.NewGuid();
            var amount = 1000m;
            var duration = 12;
            var customer = new Customer { Id = 1, CustomerNumber = customerId, CreditScore = 45 };
            var account = new Account { AccountId = targetAccountId, CustomerId = 1, Balance = 500m };

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ReturnsAsync(customer);
            _mockAccountRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Account, bool>>>(), null))
                .ReturnsAsync(account);
            _mockLoanRateRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<LoanRate, bool>>>(), null))
                .ReturnsAsync((LoanRate)null);

            // Act
            var result = await _loanService.ProcessLoan(amount, customerId, duration, targetAccountId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("BadRating");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Processing loan for customer {customerId} with amount {amount} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Customer {customerId} does not have adequate credit rating for duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task ProcessLoan_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var targetAccountId = Guid.NewGuid();
            var amount = 1000m;
            var duration = 12;

            _mockCustomerRepository.Setup(r => r.FirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), null))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _loanService.ProcessLoan(amount, customerId, duration, targetAccountId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Error.ProcessLoan");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Processing loan for customer {customerId} with amount {amount} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while processing loan for customer {customerId} with amount {amount} and duration {duration}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

    }
}


