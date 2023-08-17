using System.ComponentModel.DataAnnotations;
using lib_blazor.Models;

namespace lib_blazor.Server.Models
{
    public class Reservation
    {
        [Key] public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public Book Book { get; set; }
    }
}