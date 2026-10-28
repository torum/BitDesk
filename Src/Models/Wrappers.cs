
namespace BitDesk.Models;

// TODO: 
public class ResponseBodyWrapper
{
    public bool IsSuccess
    {
        get; set;
    }
    public string BodyText = string.Empty;
    public ApiClientError HTTPError = new();
}
