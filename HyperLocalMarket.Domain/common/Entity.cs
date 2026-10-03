using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.common
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        protected Entity(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(
                    "Entity ID cannot be empty.",
                    nameof(id));

            Id = id;
        }
        public Guid Id { get; protected set; }
    }
}
