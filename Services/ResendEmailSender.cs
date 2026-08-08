using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace Gaby.io.Services;

public class ResendEmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;
    private readonly ResendOptions _options;
    private readonly ILogger<ResendEmailSender> _logger;

    public ResendEmailSender(HttpClient httpClient, IOptions<ResendOptions> options, ILogger<ResendEmailSender> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.FromEmail))
        {
            _logger.LogError("Resend não está configurado (ApiKey/FromEmail ausentes). E-mail para {ToEmail} não foi enviado.", toEmail);
            throw new InvalidOperationException("Serviço de e-mail não configurado.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = JsonContent.Create(new
        {
            from = _options.FromEmail,
            to = new[] { toEmail },
            subject,
            html = htmlBody
        });

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Falha ao enviar e-mail via Resend para {ToEmail}. Status: {StatusCode}. Resposta: {Body}", toEmail, response.StatusCode, body);
            throw new HttpRequestException($"Falha ao enviar e-mail via Resend (status {response.StatusCode}).");
        }
    }
}
