using Helpers.Models.SharedModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;

namespace Helpers
{    
    /// <summary>
    ///  Generic RabbitMQ utility class
    /// </summary>
    public class RabbitMQ
    {
        public IModel channel;
        public IConnection connection;
        public EventingBasicConsumer consumer;
        public List<KeyValuePair<string, string>> bindings = new List<KeyValuePair<string, string>>();

        /// <summary>
        ///  Initialize RabbitMQ connection
        /// </summary>
        /// <param name="url">RabbitMQ server url</param>
        /// <param name="username">RabbitMQ server username</param>
        /// <param name="password">RabbitMQ server password</param>
        /// <returns>ApplicationStartResult</returns>
        public ApplicationStartResult Initialize(string url, string username, string password)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = url, UserName = username, Password = password };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                return new ApplicationStartResult() { Success = true };
            }
            catch (Exception ex)
            {
                return new ApplicationStartResult() { Success = false, Exception = ex };
            }

        }

        #region Consumer

        /// <summary>
        ///  Declare RabbitMQ exchange
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="type">Exchange type (direct,fanout,topic)</param>
        /// <param name="durable">Is durable</param>
        /// <param name="autoDelete">Auto delete</param>
        /// <returns></returns>
        public bool ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete)
        {
            try
            {
                channel.ExchangeDeclare(exchange: exchange, type: type, durable: durable, autoDelete: autoDelete);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///  Bind to an exhange with exchange name and routing key
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="onReceive">Message receive event</param>
        /// <returns></returns>
        public bool QueueBind(string exchange, string routingKey, Action<string, byte[]> onReceive)
        {
            try
            {
                if (bindings.Count(x => x.Value == exchange + "|" + routingKey) == 0)
                {
                    string name = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: name, exchange: exchange, routingKey: routingKey);

                    EventingBasicConsumer cons = new EventingBasicConsumer(channel);

                    cons.Received += (model, ea) =>
                    {
                        onReceive(ea.RoutingKey, ea.Body.ToArray());
                    };

                    consumer = cons;

                    channel.BasicConsume(queue: name, autoAck: true, consumer: cons);

                    bindings.Add(new KeyValuePair<string, string>(name, exchange + "|" + routingKey));
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        ///  Deletes binding
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Routing key</param>
        /// <returns></returns>
        public bool QueueDelete(string exchange, string routingKey)
        {
            try
            {
                string key = exchange + "|" + routingKey;

                foreach (var item in bindings.Where(x => x.Value == key).ToList())
                {
                    if (item.Value == key)
                    {
                        channel.QueueDelete(item.Key);
                        bindings.Remove(item);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Publisher

        /// <summary>
        ///  Publish an object to an exchange
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="data">Data object</param>
        /// <param name="props">Properties</param>
        public void Publish(string exchange, string routingKey, object data, IBasicProperties props = null)
        {
            try
            {  
                channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: props, body: Serializers.Serialize(data));
            }
            catch
            {

            }
        }

        /// <summary>
        ///  Publish binary serialized data to an exchange
        /// </summary>
        /// <param name="exchange">Exchange name</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="data">Serialized data</param>
        /// <param name="props">Properties</param>        
        public bool Publish(string exchange, string routingKey, byte[] data, IBasicProperties props = null)
        {
            try
            {
                channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: props, body: data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
