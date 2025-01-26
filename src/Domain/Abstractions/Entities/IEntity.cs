namespace Domain.Abstractions.Entities
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }
    }
}
