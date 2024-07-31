using Domain.Entities.Regulation.Enums;
using Domain.Shared;

namespace Domain.Entities.Regulation
{
    public class LawImage : BaseEntity
    {
        public LawImage(string path, int order, Guid lawId, LawImageType type) : base(Guid.NewGuid())
        {
            Path = path;
            Order = order;
            LawId = lawId;
            Type = type;
        }

        private LawImage() { }

        public String Path { get; set; }
        public LawImageType Type { get; set; }
        public int Order { get; set; }
        public Guid LawId { get; set; }

        public Law? Law { get; set; }
    }


}
