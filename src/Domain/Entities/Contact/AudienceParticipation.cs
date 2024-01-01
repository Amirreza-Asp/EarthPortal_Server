using Domain.Entities.Contact.Enums;
using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class AudienceParticipation : BaseEntity
    {
        public AudienceParticipation(Guid userId, ParticipationType type)
        {
            UserId = userId;
            Type = type;
        }

        private AudienceParticipation() { }

        public Guid UserId { get; set; }
        public ParticipationType Type { get; set; }
    }
}
