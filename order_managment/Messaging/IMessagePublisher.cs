namespace order_managment.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishMessage<T>(T message);
    }
}
