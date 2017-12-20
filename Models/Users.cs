using System.ComponentModel.DataAnnotations.Schema;

namespace mvcproj.Models
{
    [Table("User")]
    public class Users
    {
        public int id { get; set; }
        public string name { get; set; }
        public string pass { get; set; }
    }
}