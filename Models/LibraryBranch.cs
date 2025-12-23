using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace LibrarySystem.Models
{
    public class LibraryBranch
    {
        [Key]
        public int Id { get; set; }

        // "= null!;" ekleyerek sarı çizgiyi susturuyoruz
        public string Name { get; set; } = null!; 

        public string Address { get; set; } = null!;

        public Point Location { get; set; } = null!;
    }
}