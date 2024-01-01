using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Regulation.Enums
{
    public enum LawImageType
    {
        [Display(Name = "ابلاغی")]
        Announucement = 10,
        [Display(Name = "روزنامه")]
        Newspaper = 20
    }
}
