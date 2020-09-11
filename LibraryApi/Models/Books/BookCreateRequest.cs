using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Models.Books
{
    public class BookCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        public string Author { get; set; }
        [Required]
        public string Genre { get; set; }
    }
}
