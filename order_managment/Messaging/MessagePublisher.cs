using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Collections;
using System.Text;

namespace order_managment.Messaging
{
    public class MessagePublisher(ITopicClient topicClient) : IMessagePublisher
    {
        private readonly ITopicClient _client = topicClient;

        public Task PublishMessage<T>(T message)
        {
            string jsonMessage = JsonConvert.SerializeObject(message);
            Message publishMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage));

            ////string messageType = GetMessageType(typeof(T));
            ////publishMessage.UserProperties["messageType"] = messageType;

            return _client.SendAsync(publishMessage);
        }

        private string GetMessageType(Type type)
        {
            string messageType = type.Name;
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                Type? innerType = type.GetGenericArguments().FirstOrDefault();
                if (innerType is not null)
                    messageType = innerType.Name;
            }

            return messageType;
        }
    }
}
