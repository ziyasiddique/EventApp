// EventDetailsViewModel.cs
using EventApp.Models;
using System.Collections.Generic;

namespace EventApp.ViewModels
{
    public class EventDetailsViewModel
    {
        public Event Event { get; set; }
        public List<OptionalImage> OptionalImages { get; set; }
    }
}