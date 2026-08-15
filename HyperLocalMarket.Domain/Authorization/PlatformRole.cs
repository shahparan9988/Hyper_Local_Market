using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Authorization
{
    public sealed class PlatformRole : Entity
    {
        public const int CodeMaxLength = 100;
        public const int NameMaxLength = 150;
        public const int DescriptionMaxLength = 500;

        private PlatformRole()
        {
        }

        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public bool IsSystem { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
