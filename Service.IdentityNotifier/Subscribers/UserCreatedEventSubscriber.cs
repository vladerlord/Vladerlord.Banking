using MassTransit;
using Serilog;
using Service.IdentityNotifier.RazorTemplates;
using Shared.Abstractions.MessageBus.Identity;

namespace Service.IdentityNotifier.Subscribers;

public class UserCreatedEventSubscriber : IConsumer<UserCreatedEvent>
{
	private readonly IMailSender _mailSenderService;
	private readonly RazorTemplateRenderer _razorTemplateRenderer;
	private readonly ILogger _logger;

	public UserCreatedEventSubscriber(IMailSender mailSenderService, RazorTemplateRenderer razorTemplateRenderer,
		ILogger logger)
	{
		_mailSenderService = mailSenderService;
		_razorTemplateRenderer = razorTemplateRenderer;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<UserCreatedEvent> context)
	{
		var message = context.Message;

		var viewModel = new UserConfirmation("Registration confirmation", message.ConfirmationLink);
		var body = await _razorTemplateRenderer.Render(viewModel);

		_logger.Information("Sending user registration confirmation letter for {Email}", message.Email);

		await _mailSenderService.SendLetter(message.Email, "Registration confirmation", body);
	}
}

public class UserCreatedEventSubscriberDefinition : ConsumerDefinition<UserCreatedEventSubscriber>
{
	protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
		IConsumerConfigurator<UserCreatedEventSubscriber> consumerConfigurator)
	{
		consumerConfigurator.UseMessageRetry(config =>
		{
			config.Incremental(5, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
		});
	}
}