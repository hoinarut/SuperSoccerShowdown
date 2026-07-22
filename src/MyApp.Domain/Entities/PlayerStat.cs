namespace MyApp.Domain.Entities;

public abstract class PlayerStat : BaseEntity, IEquatable<PlayerStat>, IComparable<PlayerStat>
{
    protected PlayerStat(double value) => Value = value;

    // Required by EF Core
    protected PlayerStat()
    {
    }

    public double Value { get; private set; }

    public bool IsValid => Value != 0;

    public int CompareTo(PlayerStat? other) =>
        other is null ? 1 : Value.CompareTo(other.Value);

    public bool Equals(PlayerStat? other) =>
        other is not null
        && GetType() == other.GetType()
        && Value.Equals(other.Value);

    public override bool Equals(object? obj) => Equals(obj as PlayerStat);

    public override int GetHashCode() => HashCode.Combine(GetType(), Value);

    public override string ToString() => Value.ToString();

    public static bool operator ==(PlayerStat? left, PlayerStat? right) => Equals(left, right);

    public static bool operator !=(PlayerStat? left, PlayerStat? right) => !Equals(left, right);

    public static implicit operator double(PlayerStat stat) => stat.Value;
}
