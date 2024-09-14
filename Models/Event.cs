using System.ComponentModel.DataAnnotations;

namespace EventApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; }
        public List<OptionalImage> OptionalImages { get; set; } = new List<OptionalImage>(); // Navigation property
    }

    public class OptionalImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } // Path to the optional image

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}