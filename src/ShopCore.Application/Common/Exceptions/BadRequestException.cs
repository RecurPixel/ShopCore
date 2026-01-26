namespace ShopCore.Application.Common.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException()
        : base("The request was invalid.")
    {
    }

    public BadRequestException(string message)
        : base(message)
    {
    }
}