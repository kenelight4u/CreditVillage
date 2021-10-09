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
        Task<Guid> UploadToDatabase(IFormFile file, Guid id);

        Task<DetailsConfirm> GetFileFromDatabase(string id);
    }
}
