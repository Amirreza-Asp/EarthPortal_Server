using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Contact.Enums
{
    public enum ParticipationType
    {
        [Display(Name = "نظر")]
        Suggestion = 10,
        [Display(Name = "انتقاد")]
        Criticism = 20,
        [Display(Name = "شکایت")]
        Complaint = 30,
        [Display(Name = "گزار")]
        Report = 40
    }
}
