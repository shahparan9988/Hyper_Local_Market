using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Categories
{
    public sealed class CategoryAlias : Entity
    {
        public const int NameMaxLength = 150;
        public const int LanguageCodeMaxLength = 20;

        private CategoryAlias()
        {
        }

        internal CategoryAlias(
            Guid categoryId,
            string name,
            string languageCode)
        {
            Guard.NotEmpty(categoryId, nameof(categoryId));

            CategoryId = categoryId;
            Name = Guard.RequiredText(name, nameof(name), NameMaxLength);
            LanguageCode = Guard.RequiredText(
                languageCode,
                nameof(languageCode),
                LanguageCodeMaxLength);
        }

        public Guid CategoryId { get; private set; }

        public string Name { get; private set; } = default!;

        /// <summary>
        /// BCP 47-style language tag, for example "en-AU", "en-BD", or "bn-BD".
        /// </summary>
        public string LanguageCode { get; private set; } = default!;

    }
}
