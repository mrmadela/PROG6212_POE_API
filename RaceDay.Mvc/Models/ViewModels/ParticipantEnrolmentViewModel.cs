using RaceDay.Contracts;

namespace RaceDay.Mvc.Models.ViewModels
{
    // START OF PARTICIPANT ENROLMENT VIEWMODEL
    public class ParticipantEnrolmentViewModel
    {
        public EnrolmentDto Enrolment { get; set; } = new EnrolmentDto();

        public EventDto Event { get; set; } = new EventDto();

        public CategoryDto Category { get; set; } = new CategoryDto();
    }
    // END OF PARTICIPANT ENROLMENT VIEWMODEL
}