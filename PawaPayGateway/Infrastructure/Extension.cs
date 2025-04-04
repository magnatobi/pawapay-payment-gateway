using PawaPayGateway.Domain;
using PawaPayGateway.Domain.Entities;
using PawaPayGateway.Domain.Interfaces;
using PawaPayGateway.Infrastructure.Configs;
using PawaPayGateway.Infrastructure.Data;
using PawaPayGateway.Infrastructure.Services;

namespace PawaPayGateway.Infrastructure
{
    public static class Extension
    {
        /// <summary>
        /// Add Paystack instances to the service.
        /// Expects appsettings config to exist with credentials for each instance, of the form:
        /// <code>
        /// "[Gateway.CredentialKey]" {
        ///     "SecretKey": "..."
        ///  }
        /// e.g.
        /// "paystack_yourdomain" {
        ///     "SecretKey": "123"
        /// }
        /// </code>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddPaystack(this IServiceCollection services, IConfiguration configuration)
        {
            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PaystackTransferService>>();
            var mailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var gatewayManager = scope.ServiceProvider.GetRequiredService<IGatewayManager>();
            var domainAPIHost = configuration.GetValue<string>("DOMAINAPIHOST") ?? string.Empty;

            List<Gateway> paystackGateways;
            try
            {
                paystackGateways = context.Gateways.Where(g => g.GatewayType == GatewayType.Paystack).ToList();
            }
            catch (Exception e)
            {
                return services;
            }

            foreach (var gw in paystackGateways)
            {
                var config = configuration.GetSection(gw.CredentialKey)?.Get<PaystackConfig>();
                if (string.IsNullOrEmpty(config?.SecretKey))
                {
                    mailService.SendSystemErrorEmail(new MailRequest
                    {
                        BodyText = $"Error configuring Paystack gateway with id {gw.Id} and key {gw.CredentialKey}",
                        Subject = "Paystack Gateway Initialisation Error"
                    });
                    continue;
                }

                gatewayManager.AddTransactionProvider(new PaystackTransferService(config, logger, domainAPIHost, mailService, gw)
                );
            }

            return services;
        }

        /// <summary>
        /// Add Opay payment instances to the service.
        /// Expects appsettings config to exist with credentials for each instance, of the form:
        /// <code>
        /// "[Gateway.CredentialKey]" {
        ///     "SecretKey": "..."
        ///  }
        /// e.g.
        /// "opay_yourdomain" {
        ///     "SecretKey": "123"
        /// }
        /// </code>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOpayPayment(this IServiceCollection services, IConfiguration Configuration)
        {
            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var gatewayManager = scope.ServiceProvider.GetRequiredService<IGatewayManager>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<OpayService>>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            List<Gateway> gateways;
            try
            {
                gateways = context.Gateways.Where(g => g.GatewayType == GatewayType.Opay).ToList();
            }
            catch (Exception e)
            {
                return services;
            }

            foreach (var gw in gateways)
            {
                var config = Configuration.GetSection(gw.CredentialKey)?.Get<OpayConfig>();
                if (string.IsNullOrEmpty(config?.AuthToken))
                {
                    emailService.SendSystemErrorEmail(new MailRequest
                    {
                        BodyText = $"Error configuring Opay gateway with id {gw.Id} and key {gw.CredentialKey}",
                        Subject = "Opay Gateway Initialisation Error"
                    });
                    continue;
                }

                gatewayManager.AddTransactionProvider(new OpayService(logger, emailService, config, gw));
            }

            return services;
        }


        /// <summary>
        /// Add Paystack instances to the service.
        /// Expects appsettings config to exist with credentials for each instance, of the form:
        /// <code>
        /// "[Gateway.CredentialKey]" {
        ///     "SecretKey": "..."
        ///  }
        /// e.g.
        /// "paystack_tingtel" {
        ///     "SecretKey": "123"
        /// }
        /// </code>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddFlutterwave(this IServiceCollection services, IConfiguration configuration)
        {
            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PaystackTransferService>>();
            var mailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var gatewayManager = scope.ServiceProvider.GetRequiredService<IGatewayManager>();
            var domainAPIHost = configuration.GetValue<string>("IRISAPIHOST") ?? string.Empty;

            List<Gateway> paystackGateways;
            try
            {
                paystackGateways = context.Gateways.Where(g => g.GatewayType == GatewayType.FlutterWave).ToList();
            }
            catch (Exception e)
            {
                return services;
            }

            foreach (var gw in paystackGateways)
            {
                var config = configuration.GetSection(gw.CredentialKey)?.Get<PaystackConfig>();
                if (string.IsNullOrEmpty(config?.SecretKey))
                {
                    mailService.SendSystemErrorEmail(new MailRequest
                    {
                        BodyText = $"Error configuring Paystack gateway with id {gw.Id} and key {gw.CredentialKey}",
                        Subject = "Paystack Gateway Initialisation Error"
                    });
                    continue;
                }

                gatewayManager.AddTransactionProvider(new PaystackTransferService(config, logger, domainAPIHost, mailService, gw)
                );
            }

            return services;
        }
    }
}