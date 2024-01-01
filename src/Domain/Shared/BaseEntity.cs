using System.ComponentModel.DataAnnotations;

namespace Domain.Shared
{
    public abstract class BaseEntity
    {
        protected BaseEntity(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }


        [Key]
        public Guid Id { get; private set; }

        public DateTime CreatedAt { get; private set; }
    }
}
