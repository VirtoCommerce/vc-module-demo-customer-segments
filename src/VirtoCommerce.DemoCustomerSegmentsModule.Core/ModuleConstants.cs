using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Access = "virtoCommerceDemoCustomerSegmentsModule:access";
                public const string Create = "virtoCommerceDemoCustomerSegmentsModule:create";
                public const string Read = "virtoCommerceDemoCustomerSegmentsModule:read";
                public const string Update = "virtoCommerceDemoCustomerSegmentsModule:update";
                public const string Delete = "virtoCommerceDemoCustomerSegmentsModule:delete";

                public static string[] AllPermissions { get; } = { Read, Create, Access, Update, Delete };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor VirtoCommerceDemoCustomerSegmentsModuleEnabled { get; } = new SettingDescriptor
                {
                    Name = "VirtoCommerceDemoCustomerSegmentsModule.VirtoCommerceDemoCustomerSegmentsModuleEnabled",
                    GroupName = "VirtoCommerceDemoCustomerSegmentsModule|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static SettingDescriptor VirtoCommerceDemoCustomerSegmentsModulePassword { get; } = new SettingDescriptor
                {
                    Name = "VirtoCommerceDemoCustomerSegmentsModule.VirtoCommerceDemoCustomerSegmentsModulePassword",
                    GroupName = "VirtoCommerceDemoCustomerSegmentsModule|Advanced",
                    ValueType = SettingValueType.SecureString,
                    DefaultValue = "qwerty"
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return VirtoCommerceDemoCustomerSegmentsModuleEnabled;
                        yield return VirtoCommerceDemoCustomerSegmentsModulePassword;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings
            {
                get
                {
                    return General.AllSettings;
                }
            }
        }
    }
}
