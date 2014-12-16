using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace xNet.Net
{
    /// <summary>
    /// ����������, ������� �������������, � ������ ������������� ������ ��� ������ � ������.
    /// </summary>
    public sealed class ProxyException : NetException
    {
        /// <summary>
        /// ���������� ������-������, � ������� ��������� ������.
        /// </summary>
        public ProxyClient ProxyClient { get; private set; }

        
        #region ������������ (��������)

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="ProxyException"/>.
        /// </summary>
        public ProxyException() : this(Resources.ProxyException_Default) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="ProxyException"/> �������� ���������� �� ������.
        /// </summary>
        /// <param name="message">��������� �� ������ � ����������� ������� ����������.</param>
        /// <param name="innerException">����������, ��������� ������� ����������, ��� �������� <see langword="null"/>.</param>
        public ProxyException(string message, Exception innerException = null)
            : base(message, innerException) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="xNet.Net.ProxyException"/> �������� ���������� �� ������ � ������-��������.
        /// </summary>
        /// <param name="message">��������� �� ������ � ����������� ������� ����������.</param>
        /// <param name="proxyClient">������-������, � ������� ��������� ������.</param>
        /// <param name="innerException">����������, ��������� ������� ����������, ��� �������� <see langword="null"/>.</param>
        public ProxyException(string message, ProxyClient proxyClient, Exception innerException = null)
            : base(message, innerException)
        {
            ProxyClient = proxyClient;
        }

        #endregion


        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="ProxyException"/> ��������� ������������ <see cref="SerializationInfo"/> � <see cref="StreamingContext"/>.
        /// </summary>
        /// <param name="serializationInfo">��������� ������ <see cref="SerializationInfo"/>, ������� �������� ��������, ��������� ��� ������������ ������ ���������� ������ <see cref="ProxyException"/>.</param>
        /// <param name="streamingContext">��������� ������ <see cref="StreamingContext"/>, ���������� �������� ���������������� ������, ���������� � ����� ����������� ������ <see cref="ProxyException"/>.</param>
        protected ProxyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }


        /// <summary>
        /// ��������� ��������� <see cref="SerializationInfo"/> �������, ������������ ��� ������������ ���������� <see cref="ProxyException"/>.
        /// </summary>
        /// <param name="serializationInfo">������ � ������������, <see cref="SerializationInfo"/>, ������� ������ ��������������.</param>
        /// <param name="streamingContext">������ � ������������, <see cref="StreamingContext"/>, ������� ������ ��������������.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }
    }
}