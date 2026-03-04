namespace UsersAPI.Domain.Common
{
    public abstract class Entity
    {
        public Entity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public Guid Id { get; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public bool IsActive { get; protected set; }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive) return;

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
