using System.Net.Mail;
using System.Net.Sockets;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Polly;

namespace WebApplication1.ProxyServices.EmailService;

public class EmailService(IFluentEmail fluentEmail) : IDisposable
{
    public async Task<SendResponse> Send(string to, string subject, string body)
    {
        return await Policy
            .Handle<SocketException>()
            .Or<TimeoutException>()
            .Or<SmtpException>()
            .RetryAsync(3)
            .ExecuteAsync(async () => await SendTask(to, subject, body));
           
    }

    private async Task<SendResponse> SendTask(string to, string subject, string body)
    {
        var response = await fluentEmail
            .To(to)
            .Subject(subject)
            .Body(body)
            .SendAsync();

        return response;
    }

        

    #region IDisposable Support
    public void Dispose(bool dispose)
    {
        if (dispose)
        {
            Dispose();

        }
    }

    public void Dispose()
    {

        GC.SuppressFinalize(this);
        GC.Collect();
    }





    #endregion
}