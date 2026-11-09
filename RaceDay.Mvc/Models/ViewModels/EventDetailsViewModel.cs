
using RaceDay.Contracts;

namespace RaceDay.Mvc.Models.ViewModels
{// START OF NAMESPACE

    public class EventDetailsViewModel
    {// START OF CLASS

        // START OF EVENT INFORMATION
        public EventDto Event { get; set; } = new EventDto();
        // END OF EVENT INFORMATION

        // START OF EVENT CATEGORIES
        public List<CategoryDto> Categories { get; set; }
            = new List<CategoryDto>();
        // END OF EVENT CATEGORIES

    }// END OF CLASS

}// END OF NAMESPACE
