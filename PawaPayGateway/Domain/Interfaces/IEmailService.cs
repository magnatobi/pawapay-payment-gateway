namespace PawaPayGateway.Domain.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendSystemErrorEmail(MailRequest mailRequest);

        Task<bool> SendInternalSupportEmail(MailRequest mailRequest);

        Task<bool> SendEmail(MailRequest mailRequest);

        Task<bool> SendSupportEmail(MailRequest mailRequest);

        public Task ProcessSendingEmailUsingForm(string receiver, string subject, string bodyHtml, string bodyText, IReadOnlyCollection<string> attachments = null);
    }
}