using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Contact.Enums
{
    public enum Pages
    {
        [Display(Name = "کتاب ها، نشریات و مقالات")]
        Resources = 10,
        [Display(Name = "چند رسانه ای")]
        Multimedia = 20,
        [Display(Name = "اخبار و اطلاعیه ها")]
        News = 30,
        [Display(Name = "آمار عملکری")]
        FunctionalStatistics = 40,
        [Display(Name = "گزارشات مردمی")]
        PeopleReport = 50,
        [Display(Name = "راهنما و ویدیوهای آموزشی")]
        EducationVideos = 60,
        [Display(Name = "قوانین و مقررات حوزه زمین")]
        Law = 70
    }
}
