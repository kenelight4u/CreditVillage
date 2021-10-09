using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditVillageBackend.Models
{
    public class UploadModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string FileType { get; set; }

        public string Extension { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public byte[] LogoBase64 { get; set; }
    }
}
