using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabTest.Web.Configs
{
    public static class ConfigKeys
    {
        public const string AuthCookieKey = "Auth";
        public const string UserCookieKey = "user";
        public const string InvalidTokenCookieKey = "X-Invalid-Token";
    }
}
