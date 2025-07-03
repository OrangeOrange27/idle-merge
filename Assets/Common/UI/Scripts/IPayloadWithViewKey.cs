namespace Common.UI
{
    public interface IPayloadWithViewKey<out TKey>
    {
        TKey ViewKey { get; }
    }
}