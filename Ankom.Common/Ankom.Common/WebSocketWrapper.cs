/*
Microsoft .NET 4.5 ������������� ��������� �������� ������������� WebSocket.

�� ������� ������� �� ����� ���������� ��� ������ WebSocket ����� �� ��������� ���� ��������:

    1. � ������� HttpContext.AcceptWebSocketRequest
    2. ������ WCF-������ � CallbackContract � ����� netHttpBinding
    3. � ������� WebSocketHandler ��� WebSocketHost �� Microsoft.WebSockets.dll

�� ������� web-������� API WebSocket ������������� HTML 5. jQuery ����������� ��� API ��� ����� ������� �������������.
������� ���������� ���������� ����� ����� �� ��������� ��������:

   1. � �������������� ������ ClientWebSocket (System.Net.WebSockets). ������������ �������� ������������ ���������� ����� �������
   2. ��������� ������� WCF � ��� ��������� � WCF-������, �������������� ���������� WebSocket.

����������: 
   ��� ����������� ����������������� ���������� �� ������� ����������� ������ IIS 8 ��� ����, ������� ������ �� ������������ �������� ���-�������. 
   �������, ��� �������� WebSocket �������� ������ � ������������ �������� Windows 8 � Windows Server 2012, 
   � ����� � ����� ����� ������� (Windows 8.1 � Windows Server 2012 R2), ��������� ���������� � ���� ��������������� �������������� ������ IIS 8. 
   �� ���� ����������, ��� ������������ ������ ������� � ����������� � ���������� ������� Windows �� ���������.

   HTML 5 WebSocket API �� ����� ����������� �� ��������� OS. ��� ������ ������� ����� ����������� �������. 
   ��� �������� ��������� �� Microsoft, �� ��� ����� ������� IE 10 ��� ����, ��� ����������� ��������� HTML 5 WebSocket. 
*/
#if !Net40

using System;

using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ankom.Common.WSWrapper
{
    using System.Net.WebSockets;
    public class WebSocketWrapper : IDisposable
    {
        private const int ReceiveChunkSize = 1024;
        private const int SendChunkSize = 1024;

        private readonly ClientWebSocket _ws;
        private readonly Uri _uri;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _cancellationToken;

        public CancellationTokenSource WebSocketCancellation
        {
            get
            {
                return _cancellationTokenSource;
            }
        }

        private Action<WebSocketWrapper> _onConnected;
        private Action<string, WebSocketWrapper> _onMessage;
        private Action<Exception, WebSocketWrapper> _onException;
        private Action<WebSocketWrapper> _onDisconnected;

        protected WebSocketWrapper(string uri)
        {
            _ws = new ClientWebSocket();
            _ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
            _uri = new Uri(uri);
            _cancellationToken = _cancellationTokenSource.Token;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="uri">The URI of the WebSocket server.</param>
        /// <returns></returns>
        public static WebSocketWrapper Create(string uri)
        {
            return new WebSocketWrapper(uri);
        }

        /// <summary>
        /// Connects to the WebSocket server.
        /// </summary>
        /// <returns></returns>
        public WebSocketWrapper Connect()
        {
            ConnectAsync();
            return this;
        }

        /// <summary>
        /// Connects to the WebSocket server.
        /// </summary>
        /// <returns></returns>
        public async void Close()
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", _cancellationToken);
        }

        /// <summary>
        /// Set the Action to call when the connection has been established.
        /// </summary>
        /// <param name="onConnect">The Action to call.</param>
        /// <returns></returns>
        public WebSocketWrapper OnConnect(Action<WebSocketWrapper> onConnect)
        {
            _onConnected = onConnect;
            return this;
        }

        /// <summary>
        /// Set the Action to call when the connection has been terminated.
        /// </summary>
        /// <param name="onDisconnect">The Action to call</param>
        /// <returns></returns>
        public WebSocketWrapper OnDisconnect(Action<WebSocketWrapper> onDisconnect)
        {
            _onDisconnected = onDisconnect;
            return this;
        }

        /// <summary>
        /// Set the Action to call when a messages has been received.
        /// </summary>
        /// <param name="onMessage">The Action to call.</param>
        /// <returns></returns>
        public WebSocketWrapper OnMessage(Action<string, WebSocketWrapper> onMessage)
        {
            _onMessage = onMessage;
            return this;
        }

        public WebSocketWrapper OnException(Action<Exception, WebSocketWrapper> onException)
        {
            _onException = onException;
            return this;
        }
        /// <summary>
        /// Send a message to the WebSocket server.
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendMessage(string message)
        {
            SendMessageAsync(message);
        }

        private async void SendMessageAsync(string message)
        {
            if (_ws.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
                try
                {
                    var offset = (SendChunkSize * i);
                    var count = SendChunkSize;
                    var lastMessage = ((i + 1) == messagesCount);

                    if ((count * (i + 1)) > messageBuffer.Length)
                    {
                        count = messageBuffer.Length - offset;
                    }

                    await _ws.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, _cancellationToken);
                }
                catch
                {
                    throw new Exception(string.Format("Connection state is {0}.", _ws.State));
                }
        }

        private async void ConnectAsync()
        {
            try
            {
                await _ws.ConnectAsync(_uri, _cancellationToken);
            }
            catch (Exception ex)
            {
                CallOnException(ex);
                throw ex;
            }
            CallOnConnected();
            StartListen();
        }

        private async void StartListen()
        {
            var buffer = new byte[ReceiveChunkSize];

            try
            {
                while (_ws.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();


                    WebSocketReceiveResult result;
                    if (_ws.State != WebSocketState.Aborted)
                    {
                        do
                        {
                            result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await
                                    _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                                CallOnDisconnected();
                            }
                            else
                            {
                                var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                                stringResult.Append(str);
                            }

                        } while (!result.EndOfMessage);

                        CallOnMessage(stringResult);
                    }
                }
            }
            catch (Exception)
            {
                CallOnDisconnected();
            }
            finally
            {
                _ws.Dispose();
            }
        }

        private void CallOnMessage(StringBuilder stringResult)
        {
            if (_onMessage != null)
                RunInTask(() => _onMessage(stringResult.ToString(), this));
        }

        private void CallOnDisconnected()
        {
            if (_onDisconnected != null)
                RunInTask(() => _onDisconnected(this));
        }

        private void CallOnConnected()
        {
            if (_onConnected != null)
                RunInTask(() => _onConnected(this));
        }

        private void CallOnException(Exception ex)
        {
            if (_onException != null)
                RunInTask(() => _onException(ex, this));
        }

        private static void RunInTask(Action action)
        {
            Task.Factory.StartNew(action);
        }

        #region IDisposable Support
        private bool disposedValue = false; // ��� ����������� ���������� �������

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: ���������� ����������� ��������� (����������� �������).
                }

                // ���������� ������������� ������� (������������� �������) � �������������� ���� ����� ����������.
                if (_ws != null)
                    ((IDisposable)_ws).Dispose();
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Dispose();

                // TODO: ������ ������� ����� �������� NULL.

                disposedValue = true;
            }
        }

        // ��������������� ������ ����������, �.�. Dispose(bool disposing) ���� �������� ��� ��� ������������ ������������� ��������.
        ~WebSocketWrapper()
        {
            // �� ��������� ���� ���. K�� ������� �������� ���� � ������ Dispose(bool disposing).
            Dispose(false);
        }

        // ���� ��� �������� ��� ���������� ���������� ������� ��������������� ������.
        public void Dispose()
        {
            // �� ��������� ���� ���. K�� ������� �������� ���� � ������ Dispose(bool disposing).
            Dispose(true);
            // TODO: ���������������� ��������� ������, ���� ����� ���������� ���� �� ������������� .
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
#endif
