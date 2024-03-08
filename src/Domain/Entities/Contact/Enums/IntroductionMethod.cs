using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Contact.Enums
{
    public enum IntroductionMethod
    {
        [Display(Name = "جست و جو در اینترنت")]
        Search = 10,
        [Display(Name = "حضور در دستگاه های حوزه زمین")]
        Presence = 20,
        [Display(Name = "شبکه های اجتماعی")]
        SocialMedia = 30,
        [Display(Name = "معرفی توسط یک آشنا")]
        Invite = 40,
        [Display(Name = "دفاتر پیشخوان دولت")]
        CounterOffices = 50
    }
}
