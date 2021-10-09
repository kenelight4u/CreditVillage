using System.ComponentModel.DataAnnotations;

namespace CreditVillageBackend
{
    public class State
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int NationalityId { get; set; }

        public Nationality Nationality { get; set; }
    }
}