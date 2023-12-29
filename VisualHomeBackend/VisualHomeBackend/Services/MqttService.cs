using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Server;

namespace VisualHomeBackend.Services
{
    public class MqttService
    {
        MqttServer _server;

        public MqttService() 
        {
            MqttServerOptions options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()                
                .Build();

            MqttNetLogger logger = new MqttNetLogger();            

            MqttFactory mqttFactory = new MqttFactory();
            _server = mqttFactory.CreateMqttServer(options, logger);

            MyLogger myLogger = new MyLogger();

            _server.StartedAsync += (context) =>
            {
                myLogger.Publish("MQTT server started.");
                return Task.CompletedTask;
            };

            _server.ValidatingConnectionAsync += (context) =>
            {
                if (context.UserName == null)
                {
                    context.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.NotAuthorized;
                    myLogger.Publish($"Rejected unauthorized user. Username: {context.UserName ?? "null"}. ClientId: {context.ClientId}");
                }
                return Task.CompletedTask;
            };

            _server.ClientConnectedAsync += (context) =>
            {
                myLogger.Publish($"Client with client id: {context.ClientId} connected.");                
                return Task.CompletedTask;
            };

            _server.ClientSubscribedTopicAsync += (context) =>
            {
                myLogger.Publish($"Client with client id '{context.ClientId}' subscribed to topic '{context.TopicFilter.Topic}'.");
                return Task.CompletedTask;
            };

            HashSet<string> allowedTopics = new HashSet<string>()
            {
                "#",
                "$SYS/#",
                "my/topic"
            };

            _server.InterceptingSubscriptionAsync += (context) =>
            {
                string topicName = context.TopicFilter.Topic;

                if (!allowedTopics.Contains(topicName))
                {
                    context.Response.ReasonCode = MQTTnet.Protocol.MqttSubscribeReasonCode.TopicFilterInvalid;
                    context.Response.ReasonString = $"Topic '{topicName}' not allowed.";
                    //context.CloseConnection = true; // Not necessary as long as we are sure any message published to this topic will not be published
                    
                    myLogger.Publish($"Rejected subscription to topic '{topicName}'. ClientId: {context.ClientId}");
                }
                
                return Task.CompletedTask;
            };

            _server.InterceptingPublishAsync += (context) =>
            {
                string topicName = context.ApplicationMessage.Topic;

                if (!allowedTopics.Contains(topicName))
                {
                    context.Response.ReasonCode = MQTTnet.Protocol.MqttPubAckReasonCode.TopicNameInvalid;
                    context.Response.ReasonString = $"Topic '{topicName}' not allowed.";
                    //context.CloseConnection = true; // Not necessary as long as we are sure any message published to this topic will not be published

                    myLogger.Publish($"Rejected publishing to topic '{topicName}'. ClientId: {context.ClientId}");
                }

                return Task.CompletedTask;
            };



        }

        public async Task StartAsync()
        {
            await _server.StartAsync();
        }

        private class MqttNetLogger : IMqttNetLogger
        {
            public bool IsEnabled => true;

            public void Publish(MqttNetLogLevel logLevel, string source, string message, object[] parameters, Exception exception)
            {
                if (logLevel > MqttNetLogLevel.Verbose) 
                {
                    Console.WriteLine($"{source}: {message}");                        
                }

                if (!string.IsNullOrWhiteSpace(exception?.Message))
                {
                    Console.WriteLine("Exception: " + exception.Message);
                }

            }
        }

        private class MyLogger
        {
            public void Publish(string message)
            {
                Console.WriteLine($"{message}");
            }
        }
    }
}
