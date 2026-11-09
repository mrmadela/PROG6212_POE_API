using RaceDay.Contracts;

namespace RaceDay.Mvc.Models.ViewModels
{
    // START OF PARTICIPANT RESULT VIEWMODEL
    public class ParticipantResultViewModel
    {
        public ResultDto Result { get; set; } = new ResultDto();

        public EventDto Event { get; set; } = new EventDto();

        public CategoryDto Category { get; set; } = new CategoryDto();
    }
    // END OF PARTICIPANT RESULT VIEWMODEL
}