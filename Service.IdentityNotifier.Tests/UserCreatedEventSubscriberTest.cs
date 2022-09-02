using MassTransit;
using Moq;
using Serilog;
using Service.IdentityNotifier.Subscribers;
using Shared.Abstractions.MessageBus.Identity;

namespace Service.IdentityNotifier.Tests;

public class UserCreatedEventSubscriberTest
{
    private Mock<IMailSender> _mailSenderService = null!;
    private Mock<RazorTemplateRenderer> _razorTemplateRenderer = null!;
    private Mock<ILogger> _logger = null!;
    private UserCreatedEventSubscriber _subscriber = null!;

    [SetUp]
    public void Setup()
    {
        _mailSenderService = new Mock<IMailSender>();
        _razorTemplateRenderer = new Mock<RazorTemplateRenderer>();
        _logger = new Mock<ILogger>();

        _subscriber = new UserCreatedEventSubscriber(
            _mailSenderService.Object,
            _razorTemplateRenderer.Object,
            _logger.Object);
    }

    [Test]
    public async Task LetterIsSent()
    {
        // Arrange
        const string email = "test@gmail.com";
        const string confirmationLink = "http://somesite.com/confirm/qqweqweqweqwesd";

        var message = new UserCreatedEvent
        {
            Id = Guid.NewGuid(),
            Email = email,
            ConfirmationLink = confirmationLink
        };
        var context = Mock.Of<ConsumeContext<UserCreatedEvent>>(_ => _.Message == message);

        // Act
        await _subscriber.Consume(context);

        // Assert
        _mailSenderService.Verify(s => s.SendLetter(
            email,
            It.IsAny<string>(),
            It.Is<string>(body => body.Contains(confirmationLink))
        ), Times.Once);
    }
}
