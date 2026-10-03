using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Exceptions
{
    public sealed class ImageRejectedException : Exception
    {
        public ImageRejectedException(string message)
            : base(message)
        {
        }
    }
}
