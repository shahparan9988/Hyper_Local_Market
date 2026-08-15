using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Authorization
{
    public sealed class PlatformRolePermission
    {
        private PlatformRolePermission()
        {
        }

        public Guid PlatformRoleId { get; private set; }
        public Guid PlatformPermissionId { get; private set; }
    }
}
