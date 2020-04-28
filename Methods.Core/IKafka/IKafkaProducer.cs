using System;
using Methods.Core.Messages;

namespace Methods.Core.IKafka
{
    public interface IKafkaProducer
    {
        void Produce(Message message, string topicName);
    }
}
