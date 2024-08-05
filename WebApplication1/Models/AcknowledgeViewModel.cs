namespace WebApplication1.Models;

public class AcknowledgeViewModel
{
    public List<string>? Messages { get; set; } = [];
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }


    public static AcknowledgeViewModel Error(List<string>? messages = default)
    {
        return new AcknowledgeViewModel()
        {
            IsSuccess = false,
            StatusCode = 500,
            Messages = messages
        };
    }

    public static AcknowledgeViewModel Success(List<string>? messages = default)
    {
        return new AcknowledgeViewModel()
        {
            IsSuccess = true,
            StatusCode = 200,
            Messages = messages
        };
    }
}