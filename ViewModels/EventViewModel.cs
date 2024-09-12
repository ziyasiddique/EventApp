using System.ComponentModel.DataAnnotations;

namespace EventApp.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public IFormFile? Image { get; set; }
    }
}