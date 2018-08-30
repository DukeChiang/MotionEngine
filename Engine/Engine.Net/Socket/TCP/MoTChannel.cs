//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using MotionEngine.IO;

namespace MotionEngine.Net
{
	public class MoTChannel : IDisposable
	{
		#region Fields
		private readonly SocketAsyncEventArgs _receiveArgs = new SocketAsyncEventArgs();
		private readonly SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

		private readonly MoByteBuffer _sendBuffer = new MoByteBuffer(NetDefine.BufferSize);
		private readonly MoByteBuffer _tempSBuffer = new MoByteBuffer(NetDefine.PackageMaxSize);

		private readonly MoByteBuffer _receiveBuffer = new MoByteBuffer(NetDefine.BufferSize);	
		private readonly MoByteBuffer _tempRBuffer = new MoByteBuffer(NetDefine.PackageMaxSize);

		private readonly Queue<IPackage> _sendQueue = new Queue<IPackage>(10000);
		private readonly Queue<IPackage> _receiveQueue = new Queue<IPackage>(10000);

		private bool _isSending = false;
		private bool _isReceiving = false;
		#endregion

		#region Properties
		/// <summary>
		/// 通信Socket
		/// </summary>
		private Socket IOSocket { get; set; }

		/// <summary>
		/// 频道是否有效
		/// </summary>
		public bool IsValid { get { return IOSocket != null; } }
		#endregion


		public MoTChannel()
		{
			_receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
			_receiveArgs.SetBuffer(_receiveBuffer.Buf, 0, _receiveBuffer.Capacity);

			_sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
			_sendArgs.SetBuffer(_sendBuffer.Buf, 0, _sendBuffer.Capacity);
		}

		/// <summary>
		/// 初始化频道
		/// </summary>
		public void InitSocket(Socket socket)
		{
			IOSocket = socket;
			IOSocket.NoDelay = true;
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (IOSocket != null)
					IOSocket.Shutdown(SocketShutdown.Both);

				_receiveArgs.Dispose();
				_sendArgs.Dispose();

				_sendBuffer.Clear();
				_tempSBuffer.Clear();

				_receiveBuffer.Clear();
				_tempRBuffer.Clear();

				_sendQueue.Clear();
				_receiveQueue.Clear();

				_isSending = false;
				_isReceiving = false;
			}
			catch (Exception)
			{
				// throws if client process has already closed, so it is not necessary to catch.
			}
			finally
			{
				if (IOSocket != null)
				{
					IOSocket.Close();
					IOSocket = null;
				}
			}
		}

		/// <summary>
		/// 主线程内更新
		/// </summary>
		public void Update()
		{
			if (IOSocket == null || IOSocket.Connected == false)
				return;

			//接收数据
			if (_isReceiving == false)
			{
				_isReceiving = true;

				//清空缓存
				_receiveBuffer.Clear();

				//请求操作
				_receiveArgs.SetBuffer(0, _receiveBuffer.Capacity);
				bool willRaiseEvent = IOSocket.ReceiveAsync(_receiveArgs);
				if (!willRaiseEvent)
				{
					ProcessReceive(_receiveArgs);
				}
			}

			//发送数据
			if (_isSending == false && _sendQueue.Count > 0)
			{
				_isSending = true;

				//清空缓存
				_sendBuffer.Clear();

				//合并数据一起发送
				while (_sendQueue.Count > 0)
				{
					IPackage packet = _sendQueue.Dequeue();
					packet.Encode(_sendBuffer, _tempSBuffer);

					//如果已经超过一个最大包体尺寸
					//注意：发送的数据理论最大值为俩个最大包体大小
					if (_sendBuffer.WriterIndex >= NetDefine.PackageMaxSize)
						break;
				}

				//请求操作
				_sendArgs.SetBuffer(0, _sendBuffer.ReadableBytes());
				bool willRaiseEvent = IOSocket.SendAsync(_sendArgs);
				if (!willRaiseEvent)
				{
					ProcessSend(_sendArgs);
				}
			}
		}


		/// <summary>
		/// 发送消息
		/// </summary>
		public void SendMsg(IPackage packet)
		{
			_sendQueue.Enqueue(packet);
		}

		/// <summary>
		/// 获取消息
		/// </summary>
		public IPackage PickMsg()
		{
			lock (_receiveQueue)
			{
				if (_receiveQueue.Count > 0)
					return _receiveQueue.Dequeue();
			}

			return null;
		}


		/// <summary>
		/// This method is called whenever a receive or send operation is completed on a socket 
		/// </summary>
		private void IO_Completed(object sender, SocketAsyncEventArgs e)
		{
			// determine which type of operation just completed and call the associated handler
			switch (e.LastOperation)
			{
				case SocketAsyncOperation.Receive:
					MoMainThreadSyncContext.Instance.Post(ProcessReceive, e);
					break;
				case SocketAsyncOperation.Send:
					MoMainThreadSyncContext.Instance.Post(ProcessSend, e);
					break;
				default:
					throw new ArgumentException("The last operation completed on the socket was not a receive or send");
			}
		}

		/// <summary>
		/// 数据接收完成时
		/// </summary>
		private void ProcessReceive(object obj)
		{
			SocketAsyncEventArgs e = obj as SocketAsyncEventArgs;

			// check if the remote host closed the connection	
			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				_receiveBuffer.WriterIndex += e.BytesTransferred;

				//如果数据写穿
				if(_receiveBuffer.WriterIndex > _receiveBuffer.Capacity)
				{
					HandleError(true, "The channel fatal error");
					return;
				}

				//循环解包
				while (true)
				{
					//如果数据不够一个包头
					if (_receiveBuffer.ReadableBytes() < NetDefine.PackageHeadSize)
						break;

					_receiveBuffer.MarkReaderIndex();

					//如果包头标记不正确
					Int16 headMark = _receiveBuffer.ReadShort();
					if (headMark != NetDefine.PackageHeadMark)
					{
						_receiveBuffer.ResetReaderIndex();
						_receiveBuffer.ReaderIndex++;
						continue;
					}

					//获取包体信息
					Int16 msgType = _receiveBuffer.ReadShort();
					Int32 msgSize = _receiveBuffer.ReadInt();
					int remainHeadSize = NetDefine.PackageHeadSize - 2 - 2 - 4;
					for (int i = 0; i < remainHeadSize; i++)
					{
						_receiveBuffer.ReadByte();
					}

					//如果协议大小超过最大长度
					if (msgSize > NetDefine.PackageMaxSize)
					{
						HandleError(false, "The package {0} size is exceeds max size.", msgType);
						_receiveBuffer.ResetReaderIndex();
						_receiveBuffer.ReaderIndex++;
						continue;
					}

					//如果剩余可读数据小于包体长度
					if (msgSize > 0 && _receiveBuffer.ReadableBytes() < msgSize)
					{
						_receiveBuffer.ResetReaderIndex();
						break; //需要退出读够数据再解包
					}

					//正常解包			
					try
					{
						_receiveBuffer.ResetReaderIndex();
						IPackage msg = MoNetMsgHandler.Handle(msgType);
						msg.Decode(_receiveBuffer, _tempRBuffer);
						lock (_receiveQueue)
						{
							_receiveQueue.Enqueue(msg);
						}
					}
					catch (Exception ex)
					{
						// 解包异常后继续解包
						HandleError(false, "The package {0} decode error : {1}", msgType, ex.ToString());
						_receiveBuffer.ResetReaderIndex();
						_receiveBuffer.ReaderIndex++;
					}
				} //while end

				//将剩余数据移至起始
				_receiveBuffer.DiscardReadBytes();

				//为接收下一段数据，投递接收请求
				e.SetBuffer(_receiveBuffer.WriterIndex, _receiveBuffer.WriteableBytes());
				bool willRaiseEvent = IOSocket.ReceiveAsync(e);
				if (!willRaiseEvent)
				{
					ProcessReceive(e);
				}
			}
			else
			{
				HandleError(true, "ProcessReceive error : {0}", e.SocketError);
			}
		}

		/// <summary>
		/// 数据发送完成时
		/// </summary>
		private void ProcessSend(object obj)
		{
			SocketAsyncEventArgs e = obj as SocketAsyncEventArgs;
			if (e.SocketError == SocketError.Success)
			{
				_isSending = false;
			}
			else
			{
				HandleError(true, "ProcessSend error : {0}", e.SocketError);
			}
		}

		private void HandleError(bool isDispose, string format, params object[] args)
		{
			MoLog.Log(ELogType.Error, format, args);
			if (isDispose) Dispose();
		}
	}
}