using M2Mqtt;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CoeverOpen.iOS
{
    public class CoM2Mqtt : M2Mqtt.MqttClient
    {
        private short _UsedConnMethod = -1;

        private string _UserName;
        private string _Password;
        private bool _WillRetain;
        private ushort _KeepAlivePeriod;
        public CoM2Mqtt(string brokerHostName) : base(brokerHostName) { }
        public CoM2Mqtt(
    string brokerHostName, int brokerPort, bool secure,
    X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol
) : base(brokerHostName, brokerPort, secure, caCert, clientCert, sslProtocol) { }

        public CoM2Mqtt(
            string brokerHostName, int brokerPort, bool secure, MqttSslProtocols sslProtocol,
            RemoteCertificateValidationCallback userCertificateValidationCallback,
            LocalCertificateSelectionCallback userCertificateSelectionCallback
        ) : base(brokerHostName, brokerPort, secure, sslProtocol,
            userCertificateValidationCallback, userCertificateSelectionCallback)
        { }

        public CoM2Mqtt(
            string brokerHostName, int brokerPort, bool secure,
            X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol,
            RemoteCertificateValidationCallback userCertificateValidationCallback
        ) : base(brokerHostName, brokerPort, secure, caCert, clientCert, sslProtocol, userCertificateValidationCallback) { }

        public CoM2Mqtt(
            string brokerHostName, int brokerPort, bool secure,
            X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol,
            RemoteCertificateValidationCallback userCertificateValidationCallback,
            LocalCertificateSelectionCallback userCertificateSelectionCallback
        ) : base(brokerHostName, brokerPort, secure, caCert, clientCert, sslProtocol,
            userCertificateValidationCallback, userCertificateSelectionCallback)
        { }

        public new byte Connect(string clientId, string username, string password, bool willRetain, byte willQosLevel, bool willFlag, string willTopic, string willMessage, bool cleanSession, ushort keepAlivePeriod)
        {
            ConnectionClosed -= MqttConnectionClosed;

            byte result = base.Connect(clientId, username, password, willRetain, willQosLevel,
                willFlag, willTopic, willMessage, cleanSession, keepAlivePeriod);
            _UserName = username;
            _Password = password;
            _WillRetain = willRetain;
            _KeepAlivePeriod = keepAlivePeriod;

            _UsedConnMethod = 0;
            ConnectionClosed += MqttConnectionClosed;

            return result;
        }

        public new byte Connect(string clientId, string username, string password, bool cleanSession, ushort keepAlivePeriod)
        {

            ConnectionClosed -= MqttConnectionClosed;

            byte result = base.Connect(clientId, username, password, cleanSession, keepAlivePeriod);
            _UserName = username;
            _Password = password;
            _WillRetain = false;
            _KeepAlivePeriod = keepAlivePeriod;

            _UsedConnMethod = 1;
            ConnectionClosed += MqttConnectionClosed;

            return result;


        }

        public new byte Connect(string clientId, string username, string password)
        {
            ConnectionClosed -= MqttConnectionClosed;

            byte result = base.Connect(clientId, username, password);
            _UserName = username;
            _Password = password;
            _WillRetain = false;
            _KeepAlivePeriod = 0;

            _UsedConnMethod = 2;
            ConnectionClosed += MqttConnectionClosed;

            return result;
        }

        public new byte Connect(string clientId)
        {
            ConnectionClosed -= MqttConnectionClosed;

            byte result = base.Connect(clientId);
            _UserName = null;
            _Password = null;
            _WillRetain = false;
            _KeepAlivePeriod = 0;

            _UsedConnMethod = 3;
            ConnectionClosed += MqttConnectionClosed;

            return result;
        }

        public byte ConnectRandomId(string prefix, string username, string password, ushort keepAlivePeriod)
        {
            string clientId = string.IsNullOrEmpty(prefix) ? Guid.NewGuid().ToString() : $"{prefix}-{Guid.NewGuid().ToString()}";
            return Connect(clientId, username, password, true, keepAlivePeriod);
        }

        public byte ConnectRandomId(string prefix, string username, string password)
        {
            string clientId = string.IsNullOrEmpty(prefix) ? Guid.NewGuid().ToString() : $"{prefix}-{Guid.NewGuid().ToString()}";
            return Connect(clientId, username, password);
        }

        public byte ConnectRandomId(string prefix)
        {
            string clientId = string.IsNullOrEmpty(prefix) ? Guid.NewGuid().ToString() : $"{prefix}-{Guid.NewGuid().ToString()}";
            return Connect(clientId);
        }

        public new void Disconnect()
        {
            ConnectionClosed -= MqttConnectionClosed;
            base.Disconnect();

            _UserName = null;
            _Password = null;
            _WillRetain = false;
            _KeepAlivePeriod = 0;

            _UsedConnMethod = -1;
        }

        private void MqttConnectionClosed(object sender, EventArgs e)
        {
            var client = sender as MqttClient;
            var connected = client.IsConnected;
            while (!connected)
            {
                try
                {
                    switch (_UsedConnMethod)
                    {
                        case 0:
                            {
                                client.Connect(ClientId, _UserName, _Password, _WillRetain, WillQosLevel,
                                    WillFlag, WillTopic, WillMessage, CleanSession, _KeepAlivePeriod);
                                break;
                            }
                        case 1:
                            {
                                client.Connect(ClientId, _UserName, _Password, CleanSession, _KeepAlivePeriod);
                                break;
                            }
                        case 2:
                            {
                                client.Connect(ClientId, _UserName, _Password);
                                break;
                            }
                        case 3:
                            {
                                client.Connect(ClientId);
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CoeverMqttClient: Reconnect failed{System.Environment.NewLine}{ex.Message} : {ex.StackTrace}");
                }
                connected = client.IsConnected;
            }
        }

    }
}