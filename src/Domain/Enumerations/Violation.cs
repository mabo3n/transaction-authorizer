namespace Authorizer.Domain.Enumerations
{
    public enum Violation
    {
        AccountAlreadyInitialized,
        AccountNotInitialized,
        CardNotActive,
        InsufficientLimit,
        HighFrequencySmallInterval,
        DoubledTransaction,
    }
}
