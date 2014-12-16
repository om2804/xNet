using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace xNet.Net
{
    /// <summary>
    /// ������������ ������ ��� Socks5 ������-�������.
    /// </summary>
    public class Socks5ProxyClient : ProxyClient
    {
        #region ��������� (��������)

        private const int DefaultPort = 1080;

        private const byte VersionNumber = 5;
        private const byte Reserved = 0x00;
        private const byte AuthMethodNoAuthenticationRequired = 0x00;
        public static byte AuthMethodGssapi { get; } = 0x01;
        private const byte AuthMethodUsernamePassword = 0x02;
        public static byte AuthMethodIanaAssignedRangeBegin { get; } = 0x03;
        public static byte AuthMethodIanaAssignedRangeEnd { get; } = 0x7f;
        public static byte AuthMethodReservedRangeBegin { get; } = 0x80;
        public static byte AuthMethodReservedRangeEnd { get; } = 0xfe;
        private const byte AuthMethodReplyNoAcceptableMethods = 0xff;
        private const byte CommandConnect = 0x01;
        public static byte CommandBind { get; } = 0x02;
        public static byte CommandUdpAssociate { get; } = 0x03;
        private const byte CommandReplySucceeded = 0x00;
        private const byte CommandReplyGeneralSocksServerFailure = 0x01;
        private const byte CommandReplyConnectionNotAllowedByRuleset = 0x02;
        private const byte CommandReplyNetworkUnreachable = 0x03;
        private const byte CommandReplyHostUnreachable = 0x04;
        private const byte CommandReplyConnectionRefused = 0x05;
        private const byte CommandReplyTtlExpired = 0x06;
        private const byte CommandReplyCommandNotSupported = 0x07;
        private const byte CommandReplyAddressTypeNotSupported = 0x08;
        private const byte AddressTypeIpv4 = 0x01;
        private const byte AddressTypeDomainName = 0x03;
        private const byte AddressTypeIpv6 = 0x04;

        #endregion


        #region ������������ (��������)

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks5ProxyClient"/>.
        /// </summary>
        public Socks5ProxyClient()
            : this(null) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks5ProxyClient"/> �������� ������ ������-�������, � ������������� ���� ������ - 1080.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        public Socks5ProxyClient(string host)
            : this(host, DefaultPort) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks5ProxyClient"/> ��������� ������� � ������-�������.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        /// <param name="port">���� ������-�������.</param>
        public Socks5ProxyClient(string host, int port)
            : this(host, port, string.Empty, string.Empty) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks5ProxyClient"/> ��������� ������� � ������-�������.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        /// <param name="port">���� ������-�������.</param>
        /// <param name="username">��� ������������ ��� ����������� �� ������-�������.</param>
        /// <param name="password">������ ��� ����������� �� ������-�������.</param>
        public Socks5ProxyClient(string host, int port, string username, string password)
            : base(ProxyType.Socks5, host, port, username, password) { }

        #endregion


        #region ����������� ������ (��������)

        /// <summary>
        /// ����������� ������ � ��������� ������ <see cref="Socks5ProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">������ ���� - ����:����:���_������������:������. ��� ��������� ��������� �������� ���������������.</param>
        /// <returns>��������� ������ <see cref="Socks5ProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">�������� ��������� <paramref name="proxyAddress"/> ����� <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">�������� ��������� <paramref name="proxyAddress"/> �������� ������ �������.</exception>
        /// <exception cref="System.FormatException">������ ����� �������� ������������.</exception>
        public static Socks5ProxyClient Parse(string proxyAddress)
        {
            return ProxyClient.Parse(ProxyType.Socks5, proxyAddress) as Socks5ProxyClient;
        }

        /// <summary>
        /// ����������� ������ � ��������� ������ <see cref="Socks5ProxyClient"/>. ���������� ��������, �����������, ������� �� ��������� ��������������.
        /// </summary>
        /// <param name="proxyAddress">������ ���� - ����:����:���_������������:������. ��� ��������� ��������� �������� ���������������.</param>
        /// <param name="result">���� �������������� ��������� �������, �� �������� ��������� ������ <see cref="Socks5ProxyClient"/>, ����� <see langword="null"/>.</param>
        /// <returns>�������� <see langword="true"/>, ���� �������� <paramref name="proxyAddress"/> ������������ �������, ����� <see langword="false"/>.</returns>
        public static bool TryParse(string proxyAddress, out Socks5ProxyClient result)
        {
            ProxyClient proxy;

            if (TryParse(ProxyType.Socks5, proxyAddress, out proxy))
            {
                result = proxy as Socks5ProxyClient;
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
                var nStream = curTcpClient.GetStream();

                InitialNegotiation(nStream);
                SendCommand(nStream, CommandConnect, destinationHost, destinationPort);
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


        #region ������ (��������)

        private void InitialNegotiation(NetworkStream nStream)
        {
            byte authMethod;

            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                authMethod = AuthMethodUsernamePassword;
            }
            else
            {
                authMethod = AuthMethodNoAuthenticationRequired;
            }

            // +----+----------+----------+
            // |VER | NMETHODS | METHODS  |
            // +----+----------+----------+
            // | 1  |    1     | 1 to 255 |
            // +----+----------+----------+
            var request = new byte[3];

            request[0] = VersionNumber;
            request[1] = 1;
            request[2] = authMethod;

            nStream.Write(request, 0, request.Length);

            // +----+--------+
            // |VER | METHOD |
            // +----+--------+
            // | 1  |   1    |
            // +----+--------+
            var response = new byte[2];

            nStream.Read(response, 0, response.Length);

            var reply = response[1];

            if (authMethod == AuthMethodUsernamePassword && reply == AuthMethodUsernamePassword)
            {
                SendUsernameAndPassword(nStream);
            }
            else if (reply != CommandReplySucceeded)
            {
                HandleCommandError(reply);
            }
        }

        private void SendUsernameAndPassword(Stream nStream)
        {
            var uname = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            var passwd = string.IsNullOrEmpty(_password) ?
                new byte[0] : Encoding.ASCII.GetBytes(_password);

            // +----+------+----------+------+----------+
            // |VER | ULEN |  UNAME   | PLEN |  PASSWD  |
            // +----+------+----------+------+----------+
            // | 1  |  1   | 1 to 255 |  1   | 1 to 255 |
            // +----+------+----------+------+----------+
            var request = new byte[uname.Length + passwd.Length + 3];

            request[0] = 1;
            request[1] = (byte)uname.Length;
            uname.CopyTo(request, 2);
            request[2 + uname.Length] = (byte)passwd.Length;
            passwd.CopyTo(request, 3 + uname.Length);

            nStream.Write(request, 0, request.Length);

            // +----+--------+
            // |VER | STATUS |
            // +----+--------+
            // | 1  |   1    |
            // +----+--------+
            var response = new byte[2];

            nStream.Read(response, 0, response.Length);

            var reply = response[1];

            if (reply != CommandReplySucceeded)
            {
                throw NewProxyException(Resources.ProxyException_Socks5_FailedAuthOn);
            }
        }

        private void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            var aTyp = GetAddressType(destinationHost);
            var dstAddr = GetAddressBytes(aTyp, destinationHost);
            var dstPort = GetPortBytes(destinationPort);

            // +----+-----+-------+------+----------+----------+
            // |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
            // +----+-----+-------+------+----------+----------+
            // | 1  |  1  | X'00' |  1   | Variable |    2     |
            // +----+-----+-------+------+----------+----------+
            var request = new byte[4 + dstAddr.Length + 2];

            request[0] = VersionNumber;
            request[1] = command;
            request[2] = Reserved;
            request[3] = aTyp;
            dstAddr.CopyTo(request, 4);
            dstPort.CopyTo(request, 4 + dstAddr.Length);

            nStream.Write(request, 0, request.Length);

            // +----+-----+-------+------+----------+----------+
            // |VER | REP |  RSV  | ATYP | BND.ADDR | BND.PORT |
            // +----+-----+-------+------+----------+----------+
            // | 1  |  1  | X'00' |  1   | Variable |    2     |
            // +----+-----+-------+------+----------+----------+
            var response = new byte[255];

            nStream.Read(response, 0, response.Length);

            var reply = response[1];

            // ���� ������ �� ��������.
            if (reply != CommandReplySucceeded)
            {
                HandleCommandError(reply);
            }
        }

        private byte GetAddressType(string host)
        {
            IPAddress ipAddr;

            if (!IPAddress.TryParse(host, out ipAddr))
            {
                return AddressTypeDomainName;
            }

            switch (ipAddr.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    return AddressTypeIpv4;

                case AddressFamily.InterNetworkV6:
                    return AddressTypeIpv6;

                default:
                    throw new ProxyException(string.Format(Resources.ProxyException_NotSupportedAddressType,
                        host, Enum.GetName(typeof(AddressFamily), ipAddr.AddressFamily), ToString()), this);
            }

        }

        private byte[] GetAddressBytes(byte addressType, string host)
        {
            switch (addressType)
            {
                case AddressTypeIpv4:
                case AddressTypeIpv6:
                    return IPAddress.Parse(host).GetAddressBytes();

                case AddressTypeDomainName:
                    var bytes = new byte[host.Length + 1];

                    bytes[0] = (byte)host.Length;
                    Encoding.ASCII.GetBytes(host).CopyTo(bytes, 1);

                    return bytes;

                default:
                    return null;
            }
        }

        private static byte[] GetPortBytes(int port)
        {
            var array = new byte[2];

            array[0] = (byte)(port / 256);
            array[1] = (byte)(port % 256);

            return array;
        }

        private void HandleCommandError(byte command)
        {
            string errorMessage;

            switch (command)
            {
                case AuthMethodReplyNoAcceptableMethods:
                    errorMessage = Resources.Socks5_AuthMethodReplyNoAcceptableMethods;
                    break;

                case CommandReplyGeneralSocksServerFailure:
                    errorMessage = Resources.Socks5_CommandReplyGeneralSocksServerFailure;
                    break;

                case CommandReplyConnectionNotAllowedByRuleset:
                    errorMessage = Resources.Socks5_CommandReplyConnectionNotAllowedByRuleset;
                    break;

                case CommandReplyNetworkUnreachable:
                    errorMessage = Resources.Socks5_CommandReplyNetworkUnreachable;
                    break;

                case CommandReplyHostUnreachable:
                    errorMessage = Resources.Socks5_CommandReplyHostUnreachable;
                    break;

                case CommandReplyConnectionRefused:
                    errorMessage = Resources.Socks5_CommandReplyConnectionRefused;
                    break;

                case CommandReplyTtlExpired:
                    errorMessage = Resources.Socks5_CommandReplyTTLExpired;
                    break;

                case CommandReplyCommandNotSupported:
                    errorMessage = Resources.Socks5_CommandReplyCommandNotSupported;
                    break;

                case CommandReplyAddressTypeNotSupported:
                    errorMessage = Resources.Socks5_CommandReplyAddressTypeNotSupported;
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