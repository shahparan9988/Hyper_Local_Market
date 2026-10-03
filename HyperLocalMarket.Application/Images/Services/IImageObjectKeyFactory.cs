using HyperLocalMarket.Domain.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Services
{
    public interface IImageObjectKeyFactory
    {
        string CreateIncomingProductImageKey(
            Guid storeId,
            Guid assetId,
            string extension);

        string CreateProcessedProductImageKey(
            Guid storeId,
            Guid assetId);

        string CreateIncomingStoreBrandingKey(
            Guid storeId,
            StoreBrandingImageKind kind,
            Guid uploadToken,
            string extension);

        string CreateProcessedStoreBrandingKey(
            Guid storeId,
            StoreBrandingImageKind kind,
            Guid uploadToken,
            string extension);

        bool IsIncomingProductImageKey(string key);

        bool IsProcessedProductImageKey(string key);

        string CreateProcessedStoreLogo(
            Guid storeId);

        string CreateProcessedStoreCover(
            Guid storeId);
    }
}
