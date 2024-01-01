namespace Domain.Shared
{
    public abstract class BaseValueObject<TValueObject> : IEquatable<TValueObject> where TValueObject : BaseValueObject<TValueObject>
    {

        public abstract bool ObjectIsEqual(TValueObject other);
        public abstract int ObjectGetHashCode();

        public bool Equals(TValueObject? other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return ObjectIsEqual(other);
        }
        public override bool Equals(object? obj)
        {
            var otherObjects = obj as TValueObject;
            return Equals(otherObjects);
        }

        public override int GetHashCode()
        {
            return ObjectGetHashCode();
        }

        public static bool operator ==(BaseValueObject<TValueObject> left, BaseValueObject<TValueObject> right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }
        public static bool operator !=(BaseValueObject<TValueObject> left, BaseValueObject<TValueObject> right)
        {
            return !(left == right);
        }

    }
}
