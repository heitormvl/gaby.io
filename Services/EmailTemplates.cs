using System.Net;

namespace Gaby.io.Services;

public static class EmailTemplates
{
    private const string BrandColor = "#6b10f3";
    private const string FontStack = "-apple-system, BlinkMacSystemFont, 'SF Pro Text', 'Helvetica Neue', Arial, sans-serif";

    public static string PasswordReset(string displayName, string resetLink)
    {
        var safeName = WebUtility.HtmlEncode(displayName);
        var safeLink = WebUtility.HtmlEncode(resetLink);

        return $@"
<!doctype html>
<html lang=""pt-BR"">
<head>
<meta charset=""utf-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1"">
<title>Redefinir senha</title>
</head>
<body style=""margin:0; padding:0; background-color:#f2f2f7; font-family:{FontStack};"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f2f2f7; padding:40px 16px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width:480px; background-color:#ffffff; border-radius:20px; overflow:hidden; box-shadow:0 1px 3px rgba(0,0,0,0.06);"">
          <tr>
            <td style=""padding:36px 40px 8px 40px; text-align:center;"">
              <div style=""font-size:22px; font-weight:700; letter-spacing:-0.02em; color:{BrandColor};"">
                📖&nbsp;&nbsp;Gaby.io
              </div>
            </td>
          </tr>
          <tr>
            <td style=""padding:24px 40px 8px 40px;"">
              <h1 style=""margin:0 0 12px 0; font-size:20px; line-height:1.3; font-weight:700; letter-spacing:-0.01em; color:#1a1a1a; text-align:center;"">
                Redefinir sua senha
              </h1>
              <p style=""margin:0 0 24px 0; font-size:15px; line-height:1.5; color:#6c6c70; text-align:center;"">
                Olá, {safeName}! Recebemos uma solicitação para redefinir a senha da sua conta no Gaby.io. Clique no botão abaixo para escolher uma nova senha.
              </p>
            </td>
          </tr>
          <tr>
            <td style=""padding:0 40px 8px 40px; text-align:center;"">
              <a href=""{safeLink}"" style=""display:inline-block; background-color:{BrandColor}; color:#ffffff; text-decoration:none; font-size:15px; font-weight:600; padding:14px 32px; border-radius:12px;"">
                Redefinir senha
              </a>
            </td>
          </tr>
          <tr>
            <td style=""padding:20px 40px 0 40px; text-align:center;"">
              <p style=""margin:0; font-size:12px; line-height:1.5; color:#8e8e93;"">
                Se o botão não funcionar, copie e cole este link no navegador:<br>
                <a href=""{safeLink}"" style=""color:{BrandColor}; word-break:break-all;"">{safeLink}</a>
              </p>
            </td>
          </tr>
          <tr>
            <td style=""padding:28px 40px 32px 40px;"">
              <hr style=""border:none; border-top:1px solid rgba(60,60,67,0.15); margin:0 0 20px 0;"">
              <p style=""margin:0; font-size:12px; line-height:1.5; color:#8e8e93; text-align:center;"">
                Se você não solicitou essa alteração, pode ignorar este e-mail com segurança — sua senha continuará a mesma.
              </p>
            </td>
          </tr>
        </table>
        <p style=""margin:20px 0 0 0; font-size:12px; color:#a1a1a6; text-align:center;"">
          © {DateTime.UtcNow.Year} Gaby.io
        </p>
      </td>
    </tr>
  </table>
</body>
</html>";
    }
}
