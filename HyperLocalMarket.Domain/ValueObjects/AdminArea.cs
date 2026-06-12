using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.ValueObjects
{
    public sealed class AdminArea
    {

        public int? Level1Id { get; }   // DivisionId (BD) / StateId (AU optional)
        public int? Level2Id { get; }   // DistrictId (BD) / LGAId (AU optional)
        public int? Level3Id { get; }   // ThanaId (BD) / SuburbId (AU optional)

        public string? Level1 { get; } // BD: Division | AU: State
        public string? Level2 { get; } // BD: District  | AU: LGA/Region (optional)
        public string? Level3 { get; } // BD: Thana/Upazila | AU: optional
        public string? Level4 { get; } // reserved

        private AdminArea() { } // EF Core

        public AdminArea(int? level1Id, int? level2Id, int? level3Id, string? level1, string? level2, string? level3, string? level4 = null)
        {
            Level1Id = level1Id;
            Level2Id = level2Id;
            Level3Id = level3Id;

            Level1 = string.IsNullOrWhiteSpace(level1) ? null : level1.Trim();
            Level2 = string.IsNullOrWhiteSpace(level2) ? null : level2.Trim();
            Level3 = string.IsNullOrWhiteSpace(level3) ? null : level3.Trim();
            Level4 = string.IsNullOrWhiteSpace(level4) ? null : level4.Trim();
        }
    }
}
