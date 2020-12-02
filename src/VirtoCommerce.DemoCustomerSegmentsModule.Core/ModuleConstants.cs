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
    }
}
