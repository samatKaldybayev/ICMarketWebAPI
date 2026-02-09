using ICMarketWebAPI.Application.Common.Exceptions;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace ICMarketWebAPI.Application.UnitTests.Common.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void DefaultConstructor_creates_empty_error_dictionary()
    {
        // Act
        var errors = new ValidationException().Errors;

        // Assert
        errors.Should().NotBeNull();
        errors.Keys.Should().BeEmpty();
    }

    [Fact]
    public void Single_validation_failure_creates_single_error_entry()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Age", "must be over 18"),
        };

        // Act
        var errors = new ValidationException(failures).Errors;

        // Assert
        errors.Keys.Should().ContainSingle("Age");
        errors["Age"].Should().BeEquivalentTo(new[]
        {
            "must be over 18"
        });
    }

    [Fact]
    public void Multiple_validation_failures_grouped_by_property()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Age", "must be 18 or older"),
            new("Age", "must be 25 or younger"),
            new("Password", "must contain at least 8 characters"),
            new("Password", "must contain a digit"),
            new("Password", "must contain upper case letter"),
            new("Password", "must contain lower case letter"),
        };

        // Act
        var errors = new ValidationException(failures).Errors;

        // Assert
        errors.Keys.Should().BeEquivalentTo("Age", "Password");

        errors["Age"].Should().BeEquivalentTo(new[]
        {
            "must be 18 or older",
            "must be 25 or younger",
        });

        errors["Password"].Should().BeEquivalentTo(new[]
        {
            "must contain at least 8 characters",
            "must contain a digit",
            "must contain upper case letter",
            "must contain lower case letter",
        });
    }
}
