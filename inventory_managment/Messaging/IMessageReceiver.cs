namespace inventory_managment.Messaging
{
    public interface IMessageReceiver
    {
        Task ReceiveMessage();
    }
}
