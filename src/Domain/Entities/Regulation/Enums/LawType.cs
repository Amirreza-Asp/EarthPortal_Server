using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Regulation.Enums
{
    public enum LawType
    {
        [Display(Name = "آیین نامه")]
        Rule = 10,

        [Display(Name = "قانون")]
        Regulation = 20
    }
}
