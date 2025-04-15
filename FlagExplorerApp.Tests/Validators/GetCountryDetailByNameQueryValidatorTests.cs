
using FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;
using FluentAssertions;
using NUnit.Framework;

namespace FlagExplorerApp.Tests.Validators
{
    [TestFixture]
    public class GetCountryDetailByNameQueryValidatorTests
    {
        private GetCountryDetailByNameQueryValidator _validator;

        [SetUp]
        public void SetUp()
        {
            // Initialize the validator before each test
            _validator = new GetCountryDetailByNameQueryValidator();
        }

        [Test]
        public void Validator_ShouldHaveValidationError_ForNullName()
        {
            // Arrange
            var query = new GetCountryDetailByNameQuery(null);

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "Country name must not be null.");
        }

        [Test]
        public void Validator_ShouldHaveValidationError_ForEmptyName()
        {
            // Arrange
            var query = new GetCountryDetailByNameQuery(string.Empty);

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "Country name must not be empty.");
        }

        [Test]
        public void Validator_ShouldHaveValidationError_ForNameShorterThanMinimumLength()
        {
            // Arrange
            var query = new GetCountryDetailByNameQuery("A");

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "The length of 'Name' must be at least 2 characters. You entered 1 characters.");
        }

        [Test]
        public void Validator_ShouldHaveValidationError_ForNameLongerThanMaximumLength()
        {
            // Arrange
            var query = new GetCountryDetailByNameQuery(new string('A', 101));

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "The length of 'Name' must be 100 characters or fewer. You entered 101 characters.");
        }

        [Test]
        public void Validator_ShouldNotHaveValidationError_ForValidName()
        {
            // Arrange
            var query = new GetCountryDetailByNameQuery("ValidCountryName");

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }

}
