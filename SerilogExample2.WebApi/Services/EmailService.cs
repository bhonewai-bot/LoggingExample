using Serilog;

namespace SerilogExample2.WebApi.Services;

public static class EmailService
{
    public static void SendEmail(string to, string subject)
    {
        Log.Information("Sending email to {Recipient} with subject: {EmailSubject}", to, subject);

        try
        {
            Log.Debug("Building email message");
            
            Log.Information("Email successfully sent to {Recipient}", to);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send email to {Recipient}", to);
            throw;
        }
    }
}