namespace Dorbit.Framework.Utils.Queries;

public class SkipQueryOption
{
    public int Value { get; set; }

    public SkipQueryOption Clone()
    {
        return new SkipQueryOption()
        {
            Value = Value
        };
    }
}