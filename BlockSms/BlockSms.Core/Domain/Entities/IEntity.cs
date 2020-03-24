namespace BlockSms.Core.Domain.Entities
{
    public interface IEntity
    {

    }
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        TKey KeyId { get; set; }
    }
}
