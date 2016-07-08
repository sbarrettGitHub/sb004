using System.Configuration;

namespace SB004.Domain
{
    public class Configuration : IConfiguration
    {
        public string RootUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["RootUrl"];
            }
        }
        public string SystemName
        {
            get
            {
                return ConfigurationManager.AppSettings["systemName"];
            }
        }
        public string EmailFrom
        {
            get
            {
                return ConfigurationManager.AppSettings["emailFrom"];
            }
        }

        public string EmailSMTP
        {
            get
            {
                return ConfigurationManager.AppSettings["emailSMTP"];
            }
        }

        public string EmailPort
        {
            get
            {
                return ConfigurationManager.AppSettings["emailPort"];
            }
        }

        public string EmailUserId
        {
            get
            {
                return ConfigurationManager.AppSettings["emailUserId"];
            }
        }

        public string EmailPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["emailPassword"];
            }
        }

        public int ResetTokenDaysToExpire
        {
            get
            {
                int resetTokenDaysToExpire;
                string resetTokenDaysToExpireConfig = ConfigurationManager.AppSettings["resetTokenDaysToExpire"];
                if (int.TryParse(resetTokenDaysToExpireConfig, out resetTokenDaysToExpire))
                {
                    return resetTokenDaysToExpire;
                }

                // Default to 1000 years!
                return 365 * 1000;
            }
        }   
    }
}
