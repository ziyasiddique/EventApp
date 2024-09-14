using System.ComponentModel.DataAnnotations;

namespace EventApp.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile[] OptionalImages { get; set; } = new IFormFile[5]; // Adjust this line
    }
}