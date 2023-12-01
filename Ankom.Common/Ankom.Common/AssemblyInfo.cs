using System;
using System.Reflection;

namespace Ankom.Common
{
    public static class AnkomInfo
    {
#if ANKOM
        public const string AssemblyCompany = "ООО АНКОМ+";
        public const string AssemblyCopyright = "Права копирования (c) 2017-2022 ООО АНКОМ+";
        public const string SupportEmailInfo = "support@ankomplus.ru";
        public const string LinkBuyNow = "https://go.ankomplus.ru/BuyNow.aspx";
        public const string LinkCompare = "https://go.ankomplus.ru/CompareSubscriptions.aspx";
        public const string LinkCompetitiveDiscounts = "https://go.ankomplus.ru/Competitive_Discounts.aspx";
        public const string LinkEmailInfo = "mailto:info@ankomplus.ru";
        public const string LinkGetStarted = "https://go.ankomplus.ru/GetStartedOverall.aspx";
        public const string LinkGetSupport = "https://go.ankomplus.ru/GetSupport.aspx";
        public const string LinkHelp = "Https://go.ankomplus.ru/Help.aspx";
        public const string RegisterEmailInfo = "register@ankomplus.ru";
        public const string LinkRegister = "https://go.ankomplus.ru/Register.aspx";
        public const string LinkRegisterTrial = "https://go.ankomplus.ru/RegisterTrial.aspx";
        public const string LinkDownloadTrial = "https://go.ankomplus.ru/DownloadTrial.aspx";
        public const string LinkWhatsNew = "https://go.ankomplus.ru/WhatsNew.aspx";
        public const string ProductName = "АСКУЭ Политариф 2";
        public const string FileVersion = "0.1.4.0";
        public const string FullAssemblyVersionExtension = ", Version=0.1.4.0, Culture=neutral";
        public const string InstallationRegistryKeyBase = "SOFTWARE\\ANKOM+\\Политариф 2\\";
        public const string InstallationRegistryKey = "SOFTWARE\\ANKOM+\\Политариф 2\\v0.1";
        public const string MarketingVersion = "v2020 vol 1";
#else
        public const string AssemblyCompany = @"ООО ""СПб ЗИП""";
        public const string AssemblyCopyright = "Права копирования (c) 2017-2022, " + AssemblyCompany;
        public const string SupportEmailInfo = "support@spbzip.ru";
        public const string RegisterEmailInfo = "register@spbzip.ru";
        public const string ProductName = "Конфигуратор приборов учёта";
        public const string FileVersion = "1.0.2022.0";
#endif
        /*
        public const string PublicKey = "0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2";
        /// <summary>
        /// Токен открытого ключа представляет собой последние 8 байтов 
        ///  хэша SHA-1 открытого ключа, которым подписано приложение.
        /// </summary>
        public const string PublicKeyToken = "b88d1754d700e49a";
        */
    }
    public class AnkomAssemblyInfo
    {
#region Методы доступа к атрибутам сборки

        private Assembly _assembly;

        public AnkomAssemblyInfo(Assembly assembly)
        {
            _assembly = assembly;
        }
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(_assembly.CodeBase);
            }
        }

        public string AssemblyLocation
        {
            get
            {
                return _assembly.Location;
                //                return System.IO.Path.GetFileNameWithoutExtension(_assembly.CodeBase);
            }
        }

        private AssemblyName _assemblyName { get => _assembly?.GetName(); }
        public string AssemblyVersion
        {
            get
            {
                return _assemblyName?.Version.ToString();
            }
        }

        public string AssemblyVersionMajor
        {
            get
            {
                return _assemblyName?.Version.Major.ToString();
            }
        }

        public string AssemblyVersionMinor
        {
            get
            {
                return _assemblyName?.Version.Minor.ToString();
            }
        }

        public string AssemblyVersionBuild
        {
            get
            {
                return _assemblyName?.Version.Build.ToString();
            }
        }

        public string AssemblyVersionRevisionHex
        {
            get
            {
                return (_assemblyName != null) ? HexUtils.BytesToHexStr(
                    HexUtils.FromUint16((UInt16)_assemblyName.Version.Revision)) : string.Empty;
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = _assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
#endregion
    }
}
