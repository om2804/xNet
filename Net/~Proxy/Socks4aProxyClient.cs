using System.Net.Sockets;
using System.Text;

namespace xNet.Net
{
    /// <summary>
    /// ������������ ������ ��� Socks4a ������-�������.
    /// </summary>
    public class Socks4AProxyClient : Socks4ProxyClient 
    {
        #region ������������ (��������)

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4AProxyClient"/>.
        /// </summary>
        public Socks4AProxyClient()
            : this(null) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4AProxyClient"/> �������� ������ ������-�������, � ������������� ���� ������ - 1080.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        public Socks4AProxyClient(string host)
            : this(host, DefaultPort) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4AProxyClient"/> ��������� ������� � ������-�������.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        /// <param name="port">���� ������-�������.</param>
        public Socks4AProxyClient(string host, int port)
            : this(host, port, string.Empty) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4AProxyClient"/> ��������� ������� � ������-�������.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        /// <param name="port">���� ������-�������.</param>
        /// <param name="username">��� ������������ ��� ����������� �� ������-�������.</param>
        public Socks4AProxyClient(string host, int port, string username)
            : base(host, port, username)
        {
            _type = ProxyType.Socks4A;
        }

        #endregion


        #region ������ (��������)

        /// <summary>
        /// ����������� ������ � ��������� ������ <see cref="Socks4AProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">������ ���� - ����:����:���_������������:������. ��� ��������� ��������� �������� ���������������.</param>
        /// <returns>��������� ������ <see cref="Socks4AProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">�������� ��������� <paramref name="proxyAddress"/> ����� <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">�������� ��������� <paramref name="proxyAddress"/> �������� ������ �������.</exception>
        /// <exception cref="System.FormatException">������ ����� �������� ������������.</exception>
        public static Socks4AProxyClient Parse(string proxyAddress)
        {
            return ProxyClient.Parse(ProxyType.Socks4A, proxyAddress) as Socks4AProxyClient;
        }

        /// <summary>
        /// ����������� ������ � ��������� ������ <see cref="Socks4AProxyClient"/>. ���������� ��������, �����������, ������� �� ��������� ��������������.
        /// </summary>
        /// <param name="proxyAddress">������ ���� - ����:����:���_������������:������. ��� ��������� ��������� �������� ���������������.</param>
        /// <param name="result">���� �������������� ��������� �������, �� �������� ��������� ������ <see cref="Socks4AProxyClient"/>, ����� <see langword="null"/>.</param>
        /// <returns>�������� <see langword="true"/>, ���� �������� <paramref name="proxyAddress"/> ������������ �������, ����� <see langword="false"/>.</returns>
        public static bool TryParse(string proxyAddress, out Socks4AProxyClient result)
        {
            ProxyClient proxy;

            if (TryParse(ProxyType.Socks4A, proxyAddress, out proxy))
            {
                result = proxy as Socks4AProxyClient;
                return true;
            }
            result = null;
            return false;
        }

        #endregion


        internal protected override void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            var dstPort = GetPortBytes(destinationPort);
            byte[] dstIp = { 0, 0, 0, 1 };

            var userId = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            var dstAddr = Encoding.ASCII.GetBytes(destinationHost);

            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL| DSTADDR      |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            //    1    1      2              4           variable       1    variable        1 
            var request = new byte[10 + userId.Length + dstAddr.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstPort.CopyTo(request, 2);
            dstIp.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;
            dstAddr.CopyTo(request, 9 + userId.Length);
            request[9 + userId.Length + dstAddr.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //    1    1      2              4
            var response = new byte[8];

            nStream.Read(response, 0, 8);

            var reply = response[1];

            // ���� ������ �� ��������.
            if (reply != CommandReplyRequestGranted)
            {
                HandleCommandError(reply);
            }
        }
    }
}