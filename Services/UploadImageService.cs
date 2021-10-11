using CreditVillageBackend.Interfaces;
using CreditVillageBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CreditVillageBackend.Services
{
    public class UploadImageService : IUploadImage
    {
        private readonly ApplicationDbContext _dbContext;

        public UploadImageService(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Guid> UploadImageToDatabase(IFormFile file, Guid id)
        {
            //checks if user have a proile pic before now
            var existingImage = await _dbContext.ProfileImage.SingleOrDefaultAsync(p => p.Id == id);

            //if true, it Updates
            if (existingImage != null)
            {
               var updatedId = UpdateImageInDatabase(existingImage, file);

                return updatedId.Result;
            }
            // Else, it Creates
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);

            var uploadImage = new UploadModel
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                Extension = extension,
                FileType = file.ContentType,
                Name = fileName,
            };
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                uploadImage.LogoBase64 = stream.ToArray();
            }
            _dbContext.ProfileImage.Add(uploadImage);
            await _dbContext.SaveChangesAsync();

            return uploadImage.Id;
        }

        public async Task<DetailsConfirm> GetFileFromDatabase(string id)
        {
            var file = await _dbContext.ProfileImage.Where(x => x.Id.ToString() == id).FirstOrDefaultAsync();
            if (file == null)
                return null;

            var data = new DetailsConfirm()
            {
                File = Convert.ToBase64String(file.LogoBase64),
                FullName = file.Name+file.Extension
            };

            return data;
        }

        public async Task<Guid> UpdateImageInDatabase(UploadModel existingImage, IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);

            existingImage.FileType = file.ContentType;
            existingImage.ModifiedOn = DateTime.Now;
            existingImage.Extension = extension;
            existingImage.Name = fileName;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                existingImage.LogoBase64 = stream.ToArray();
            }

            _dbContext.ProfileImage.Update(existingImage);
            await _dbContext.SaveChangesAsync();

            return existingImage.Id;
        }
    }
}
