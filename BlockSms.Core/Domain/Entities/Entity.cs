using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace BlockSms.Core.Domain.Entities
{
    [Serializable]
    public abstract class Entity : IEntity
    {
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}]";
        }
    }
    [Serializable]
    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        [Key]
        public virtual TKey KeyId { get; set; }

        protected Entity()
        {

        }

        protected Entity(TKey id)
        {
            KeyId = id;
        }

        private List<INotification> _domainEvents;
        [NotMapped]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TKey>))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            var other = (Entity<TKey>)obj;
            if (EntityHelper.HasDefaultId(this) && EntityHelper.HasDefaultId(other))
            {
                return false;
            }

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType().GetTypeInfo();
            var typeOfOther = other.GetType().GetTypeInfo();
            if (!typeOfThis.IsAssignableFrom(typeOfOther) && !typeOfOther.IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return KeyId.Equals(other.KeyId);
        }

        public override int GetHashCode()
        {
            if (KeyId == null)
            {
                return 0;
            }

            return KeyId.GetHashCode();

        }
        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (Equals(left, null))
                return Equals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}] KeyId = {KeyId}";
        }
    }
}
