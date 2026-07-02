using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Settings;

namespace MyEcommerce.Emailing
{
    public class EmailSettingProvider : SettingDefinitionProvider
    {
        private readonly ISettingEncryptionService encryptionService;
        private IConfiguration _configuration { get; }

        public EmailSettingProvider(ISettingEncryptionService encryptionService, IConfiguration configuration)
        {
            this.encryptionService = encryptionService;
            _configuration = configuration;
        }

        public override void Define(ISettingDefinitionContext context)
        {
            var passSetting = context.GetOrNull("Abp.Mailing.Smtp.Password");
            if (passSetting != null)
            {
                string debug = encryptionService.Encrypt(passSetting, _configuration["Settings:Abp.Mailing.Smtp.Password"]) ?? "";
            }
        }
    }
}