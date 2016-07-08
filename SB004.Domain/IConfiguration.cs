namespace SB004.Domain
{
    public interface IConfiguration
    {
        string RootUrl { get; }

        string SystemName { get; }

        string EmailFrom { get; }

        string EmailSMTP { get; }

        string EmailPort { get; }

        string EmailUserId { get; }

        string EmailPassword { get; }

        int ResetTokenDaysToExpire { get; }

    }
}