namespace AgroControl.Domain.Common;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("O identificador não pode ser vazio.", nameof(id));
        }

        Id = id;
    }

    public Guid Id { get; private init; }
}
