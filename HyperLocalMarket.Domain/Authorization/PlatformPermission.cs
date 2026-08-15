using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Authorization
{
    public sealed class PlatformPermission : Entity
    {
        public const int CodeMaxLength = 150;
        public const int DescriptionMaxLength = 500;

        private PlatformPermission()
        {
        }

        public string Code { get; private set; } = null!;
        public string? Description { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
