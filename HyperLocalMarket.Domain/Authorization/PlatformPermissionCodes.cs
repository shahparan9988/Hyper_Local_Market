using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Authorization
{
    public static class PlatformPermissionCodes
    {
        public const string RolesAssign = "roles.assign";
        public const string UsersView = "users.view";
        public const string UsersSuspend = "users.suspend";
        public const string StoresSuspend = "stores.suspend";
        public const string CategoriesReview = "categories.review";
        public const string CategoriesManage = "categories.manage";
    }
}
