using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;

namespace AppPubDeployUtility
{
    public class AuthConfig
    {
        public string Instance { get; set; }
        public string TenantID { get; set; }
        public string ClientID { get; set; }

        public string Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, Instance, TenantID);
            }
        }

        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string ResourceID { get; set; }

        public static AuthConfig ReadJsonFromFile(string path)
        {
            IConfiguration Configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

            Configuration = builder.Build();

            return Configuration.Get<AuthConfig>();
        }
    }
}