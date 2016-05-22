using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Configuration
{
    public sealed class GreenspotConfiguration : ConfigurationSection
    {
        private GreenspotConfiguration()
        {

        }

        static GreenspotConfiguration()
        {
            _instance = (GreenspotConfiguration)System.Configuration.ConfigurationManager.GetSection("greenspot");
        }

        private static GreenspotConfiguration _instance;

        [ConfigurationProperty("payment")]
        private PaymentElement _payment
        {
            get
            {
                return (PaymentElement)this["payment"];
            }
        }
        public static PaymentElement Payment
        {
            get
            {
                return _instance._payment;
            }
        }

        [ConfigurationProperty("accessAccounts")]
        [ConfigurationCollection(typeof(AccessAccountElementCollection),
          AddItemName = "add",
          ClearItemsName = "clear",
          RemoveItemName = "remove")]
        private AccessAccountElementCollection _accessAccounts
        {
            get
            {
                return (AccessAccountElementCollection)this["accessAccounts"];
            }
        }
        public static AccessAccountElementCollection AccessAccounts
        {
            get
            {
                return _instance._accessAccounts;
            }
        }


        [ConfigurationProperty("directories", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(NameValueConfigurationCollection),
          AddItemName = "add",
          ClearItemsName = "clear",
          RemoveItemName = "remove")]
        private NameValueConfigurationCollection _directories
        {
            get
            {
                return (NameValueConfigurationCollection)this["directories"];
            }
        }

        public static NameValueConfigurationCollection Directories
        {
            get
            {
                return _instance._directories;
            }
        }

        [ConfigurationProperty("appSetttings", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection),
        AddItemName = "add",
        ClearItemsName = "clear",
        RemoveItemName = "remove")]
        private KeyValueConfigurationCollection _appSetttings
        {
            get
            {
                return (KeyValueConfigurationCollection)this["appSetttings"];
            }
        }
        public static KeyValueConfigurationCollection AppSettings
        {
            get
            {
                return _instance._appSetttings;
            }
        }
        //
        public static string TemporaryDirectory
        {
            get
            {
                return Directories["temp"].Value;
            }
        }
        public static string TesseractDataDirectory
        {
            get
            {
                return Directories["tesseractData"].Value;
            }
        }
        public static bool FakeLogin
        {
            get
            {
                return bool.Parse(AppSettings["fakeLogin"].Value);
            }
        }
        public static string RootUrl
        {
            get
            {
                return AppSettings["rootURL"].Value;
            }
        }
    }

    /// <summary>
    /// payment element
    /// </summary>
    public class PaymentElement : ConfigurationElement
    {
        [ConfigurationProperty("fullCharge", DefaultValue = "false", IsRequired = true)]
        public Boolean IsFullCharge
        {
            get
            {
                return (Boolean)this["fullCharge"];
            }
        }

        [ConfigurationProperty("exchangeRateCNY", IsRequired = true)]
        public decimal ExchangeRateCNY
        {
            get
            {
                return (decimal)this["exchangeRateCNY"];
            }
        }

        public decimal ExchangeRateNZD
        {
            get
            {
                return 1.0M;
            }
        }

        [ConfigurationProperty("discount", IsRequired = true)]
        public decimal Discount
        {
            get
            {
                return (decimal)this["discount"];
            }
        }
    }

    /// <summary>
    /// access account element
    /// </summary>
    public class AccessAccountElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return this["type"] as string;
            }
        }
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get
            {
                return this["id"] as string;
            }
        }
        [ConfigurationProperty("secret", IsRequired = true)]
        public string Secret
        {
            get
            {
                return this["secret"] as string;
            }
        }
    }

    /// <summary>
    /// directory collection
    /// </summary>
    public class AccessAccountElementCollection : ConfigurationElementCollection
    {

        new public AccessAccountElement this[string appType]
        {
            get { return (AccessAccountElement)BaseGet(appType); }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new AccessAccountElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AccessAccountElement)element).Type;
        }
    }
}
