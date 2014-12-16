using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace xNet.Net
{
    /// <summary>
    /// ������������ ������ ��� Socks4 ������-�������.
    /// </summary>
    public class Socks4ProxyClient : ProxyClient
    {
        #region ��������� (����������)

        internal protected const int DefaultPort = 1080;

        internal protected const byte VersionNumber = 4;
        internal protected const byte CommandConnect = 0x01;
        internal protected const byte CommandBind = 0x02;
        internal protected const byte CommandReplyRequestGranted = 0x5a;
        internal protected const byte CommandReplyRequestRejectedOrFailed = 0x5b;
        internal protected const byte CommandReplyRequestRejectedCannotConnectToIdentd = 0x5c;
        internal protected const byte CommandReplyRequestRejectedDifferentIdentd = 0x5d;

        #endregion


        #region ������������ (��������)

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4ProxyClient"/>.
        /// </summary>
        public Socks4ProxyClient()
            : this(null) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4ProxyClient"/> �������� ������ ������-�������, � ������������� ���� ������ - 1080.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        public Socks4ProxyClient(string host)
            : this(host, DefaultPort) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4ProxyClient"/> ��������� ������� � ������-�������.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        /// <param name="port">���� ������-�������.</param>
        public Socks4ProxyClient(string host, int port)
            : this(host, port, string.Empty) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4ProxyClient"/> ��������� ������� � ������-�������.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        /// <param name="port">���� ������-�������.</param>
        /// <param name="username">��� ������������ ��� ����������� �� ������-�������.</param>
        public Socks4ProxyClient(string host, int port, string username)
            : base(ProxyType.Socks4, host, port, username, null) { }

        #endregion


        #region ����������� ������ (��������)

        /// <summary>
        /// ����������� ������ � ��������� ������ <see cref="Socks4ProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">������ ���� - ����:����:���_������������:������. ��� ��������� ��������� �������� ���������������.</param>
        /// <returns>��������� ������ <see cref="Socks4ProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">�������� ��������� <paramref name="proxyAddress"/> ����� <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">�������� ��������� <paramref name="proxyAddress"/> �������� ������ �������.</exception>
        /// <exception cref="System.FormatException">������ ����� �������� ������������.</exception>
        public static Socks4ProxyClient Parse(string proxyAddress)
        {
            return Parse(ProxyType.Socks4, proxyAddress) as Socks4ProxyClient;
        }

        /// <summary>
        /// ����������� ������ � ��������� ������ <see cref="Socks4ProxyClient"/>. ���������� ��������, �����������, ������� �� ��������� ��������������.
        /// </summary>
        /// <param name="proxyAddress">������ ���� - ����:����:���_������������:������. ��� ��������� ��������� �������� ���������������.</param>
        /// <param name="result">���� �������������� ��������� �������, �� �������� ��������� ������ <see cref="Socks4ProxyClient"/>, ����� <see langword="null"/>.</param>
        /// <returns>�������� <see langword="true"/>, ���� �������� <paramref name="proxyAddress"/> ������������ �������, ����� <see langword="false"/>.</returns>
        public static bool TryParse(string proxyAddress, out Socks4ProxyClient result)
        {
            ProxyClient proxy;

            if (TryParse(ProxyType.Socks4, proxyAddress, out proxy))
            {
                result = proxy as Socks4ProxyClient;
                return true;
            }
            result = null;
            return false;
        }

        #endregion


        /// <summary>
        /// ������ ���������� � �������� ����� ������-������.
        /// </summary>
        /// <param name="destinationHost">���� �������, � ������� ����� ��������� ����� ������-������.</param>
        /// <param name="destinationPort">���� �������, � ������� ����� ��������� ����� ������-������.</param>
        /// <param name="tcpClient">����������, ����� ������� ����� ��������, ��� �������� <see langword="null"/>.</param>
        /// <returns>���������� � �������� ����� ������-������.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// �������� �������� <see cref="Host"/> ����� <see langword="null"/> ��� ����� ������� �����.
        /// -���-
        /// �������� �������� <see cref="Port"/> ������ 1 ��� ������ 65535.
        /// -���-
        /// �������� �������� <see cref="Username"/> ����� ����� ����� 255 ��������.
        /// -���-
        /// �������� �������� <see cref="Password"/> ����� ����� ����� 255 ��������.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">�������� ��������� <paramref name="destinationHost"/> ����� <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">�������� ��������� <paramref name="destinationHost"/> �������� ������ �������.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">�������� ��������� <paramref name="destinationPort"/> ������ 1 ��� ������ 65535.</exception>
        /// <exception cref="xNet.Net.ProxyException">������ ��� ������ � ������-��������.</exception>
        public override TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null)
        {
            CheckState();

            #region �������� ����������

            if (destinationHost == null)
            {
                throw new ArgumentNullException("destinationHost");
            }

            if (destinationHost.Length == 0)
            {
                throw ExceptionHelper.EmptyString("destinationHost");
            }

            if (!ExceptionHelper.ValidateTcpPort(destinationPort))
            {
                throw ExceptionHelper.WrongTcpPort("destinationPort");
            }

            #endregion

            var curTcpClient = tcpClient ?? CreateConnectionToProxy();

            try
            {
                SendCommand(curTcpClient.GetStream(), CommandConnect, destinationHost, destinationPort);
            }
            catch (Exception ex)
            {
                curTcpClient.Close();

                if (ex is IOException || ex is SocketException)
                {
                    throw NewProxyException(Resources.ProxyException_Error, ex);
                }

                throw;
            }

            return curTcpClient;
        }


        #region ������ (���������� ����������)

        internal protected virtual void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            var dstPort = GetIPAddressBytes(destinationHost);
            var dstIp = GetPortBytes(destinationPort);

            var userId = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            // +----+----+----+----+----+----+----+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+
            //    1    1      2              4           variable       1
            var request = new byte[9 + userId.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstIp.CopyTo(request, 2);
            dstPort.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //   1    1       2              4
            var response = new byte[8];

            nStream.Read(response, 0, response.Length);

            var reply = response[1];

            // ���� ������ �� ��������.
            if (reply != CommandReplyRequestGranted)
            {
                HandleCommandError(reply);
            }
        }

        internal protected byte[] GetIPAddressBytes(string destinationHost)
        {
            IPAddress ipAddr;

            if (IPAddress.TryParse(destinationHost, out ipAddr)) return ipAddr.GetAddressBytes();
            try
            {
                var ips = Dns.GetHostAddresses(destinationHost);

                if (ips.Length > 0)
                {
                    ipAddr = ips[0];
                }
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is ArgumentException)
                {
                    throw new ProxyException(string.Format(
                        Resources.ProxyException_FailedGetHostAddresses, destinationHost), this, ex);
                }

                throw;
            }

            return ipAddr.GetAddressBytes();
        }

        internal protected byte[] GetPortBytes(int port)
        {
            var array = new byte[2];

            array[0] = (byte)(port / 256);
            array[1] = (byte)(port % 256);

            return array;
        }

        internal protected void HandleCommandError(byte command)
        {
            string errorMessage;

            switch (command)
            {
                case CommandReplyRequestRejectedOrFailed:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedOrFailed;
                    break;

                case CommandReplyRequestRejectedCannotConnectToIdentd:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedCannotConnectToIdentd;
                    break;

                case CommandReplyRequestRejectedDifferentIdentd:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedDifferentIdentd;
                    break;

                default:
                    errorMessage = Resources.Socks_UnknownError;
                    break;
            }

            var exceptionMsg = string.Format(
                Resources.ProxyException_CommandError, errorMessage, ToString());

            throw new ProxyException(exceptionMsg, this);
        }

        #endregion
    }
}