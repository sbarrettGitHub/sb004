namespace SB004.Business
{
    public interface IPasswordBusiness
    {
        string Hash(string password);
        bool ValidatePassword(string password, string correctHash);
        void EnforcePasswordStengthRules(string password);
    }
}
