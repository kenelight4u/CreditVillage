using System.ComponentModel.DataAnnotations;

namespace CreditVillageBackend
{
    public class Nationality
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}