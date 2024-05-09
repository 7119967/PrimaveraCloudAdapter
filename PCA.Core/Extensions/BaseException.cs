namespace PCA.Core.Extensions;

public class BaseException : Exception
{
    public int Code { get; }
    public override string Message { get; }

    public BaseException(string message, int code = 400)
    {
        Code = code;
        Message = message;
    }
}