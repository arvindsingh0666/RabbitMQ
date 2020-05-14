using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace ConsumerApp2
{
    class Program
    {
        private static IConnection _connection;
        private static IModel _channel;
        static void Main(string[] args)
        {
            Console.WriteLine("CONSUMER APP - 2 !");
            var factory = new ConnectionFactory { HostName = "localhost" };

            // create connection
            _connection = factory.CreateConnection();

            // create channel
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("Booking.Orders", ExchangeType.Topic, true);
            _channel.QueueDeclare("Booking.Modified", true, false, false, null);
            _channel.QueueBind("Booking.Modified", "Booking.Orders", "Booking.Modified.*", null);
            _channel.BasicQos(0, 1, false);



            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                //When JSON message
                //var content = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(ea.Body));

                // handle the received message
                Console.WriteLine("CONSUMER APP - 2  === " + content.ToString());
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("Booking.Modified", false, consumer);
        }
    }
}
