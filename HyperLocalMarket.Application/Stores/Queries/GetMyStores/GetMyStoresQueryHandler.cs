using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetMyStores
{
    public sealed class GetMyStoresQueryHandler
        : IRequestHandler<
            GetMyStoresQuery,
            IReadOnlyList<MyStoreDto>>
    {
        private readonly IMyStoresReader _myStoresReader;

        public GetMyStoresQueryHandler(
            IMyStoresReader myStoresReader)
        {
            _myStoresReader = myStoresReader;
        }

        public Task<IReadOnlyList<MyStoreDto>> Handle(
            GetMyStoresQuery request,
            CancellationToken cancellationToken)
        {
            return _myStoresReader.GetAsync(
                request.UserId,
                cancellationToken);
        }
    }
}
