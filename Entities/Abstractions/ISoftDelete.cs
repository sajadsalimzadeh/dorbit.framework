namespace Dorbit.Framework.Entities.Abstractions;

public interface ISoftDelete : IEntity
{
    bool IsDeleted { get; set; }
}