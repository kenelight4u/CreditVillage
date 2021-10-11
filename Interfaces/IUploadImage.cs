using CreditVillageBackend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditVillageBackend.Interfaces
{
    public interface IUploadImage
    {
        Task<Guid> UploadImageToDatabase(IFormFile file, Guid id);

        Task<Guid> UpdateImageInDatabase(UploadModel existingImage, IFormFile file);

        Task<DetailsConfirm> GetFileFromDatabase(string id);
    }
}
