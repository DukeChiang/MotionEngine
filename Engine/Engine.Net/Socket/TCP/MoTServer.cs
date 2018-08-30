//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MotionEngine.Net
{
	/// <summary>
	/// 异步IOCP SOCKET服务器
	/// </summary>
	public class MoTServer : IDisposable
	{
		#region Fields
		/// <summary>
		/// 监听Socket，用于接受客户端的连接请求
		/// </summary>
		private Socket _listenSocket;

		/// <summary>
		/// 信号量
		/// This Semaphore is used to keep from going over max connection.
		/// </summary>
		private Semaphore _maxAcceptedSemaphore;

		/// <summary>
		/// 通信频道列表
		/// </summary>
		private readonly List<MoTChannel> _allChannels;
		#endregion

		#region Properties
		/// <summary>
		/// 服务器程序允许的最大客户端连接数
		/// </summary>
		public int MaxClient { get; }

		/// <summary>
		/// 服务器是否正在运行
		/// </summary>
		public bool IsRunning { get; private set; }

		/// <summary>
		/// 监听的IP地址
		/// </summary>
		public IPAddress Address { get; private set; }

		/// <summary>
		/// 监听的端口
		/// </summary>
		public int Port { get; private set; }
		#endregion

		#region Ctors
		/// <summary>
		/// 异步Socket TCP服务器
		/// </summary>
		/// <param name="listenPort">监听的端口</param>
		/// <param name="maxClient">最大的客户端数量</param>
		public MoTServer(int listenPort, int maxClient)
				: this(IPAddress.Any, listenPort, maxClient)
		{
		}

		/// <summary>
		/// 异步Socket TCP服务器
		/// </summary>
		/// <param name="localEP">监听的终结点</param>
		/// <param name="maxClient">最大客户端数量</param>
		public MoTServer(IPEndPoint localEP, int maxClient)
			: this(localEP.Address, localEP.Port, maxClient)
		{
		}

		private MoTServer(IPAddress address, int port, int maxClient)
		{
			Address = address;
			Port = port;
			MaxClient = maxClient;
			_allChannels = new List<MoTChannel>(MaxClient);
		}
		#endregion

		/// <summary>
		/// 开始服务
		/// </summary>
		public void Start()
		{
			if (IsRunning)
				return;

			IsRunning = true;

			//最大连接数信号
			_maxAcceptedSemaphore = new Semaphore(MaxClient, MaxClient);

			//创建监听socket
			IPEndPoint localEndPoint = new IPEndPoint(Address, Port);
			_listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_listenSocket.Bind(localEndPoint);

			//开始监听
			_listenSocket.Listen(1000);

			// 在监听Socket上投递一个接受请求
			StartAccept(null);
		}

		/// <summary>
		/// 停止服务
		/// </summary>
		public void Stop()
		{
			if (IsRunning)
			{
				IsRunning = false;
				Dispose();
			}
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			if (_listenSocket != null)
			{
				_listenSocket.Close();
				_listenSocket = null;
			}

			if (_maxAcceptedSemaphore != null)
			{
				_maxAcceptedSemaphore.Close();
				_maxAcceptedSemaphore = null;
			}

			for (int i=0; i< _allChannels.Count; i++)
			{
				_allChannels[i].Dispose();
			}
			_allChannels.Clear();
		}

		/// <summary>
		/// 操作完成时回调函数
		/// </summary>
		private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
		{
			switch (e.LastOperation)
			{
				case SocketAsyncOperation.Accept:
					MoMainThreadSyncContext.Instance.Post(ProcessAccept, e);
					break;
				case SocketAsyncOperation.Connect:
					MoMainThreadSyncContext.Instance.Post(ProcessConnected, e);
					break;
				default:
					throw new ArgumentException("The last operation completed on the socket was not a accept or connect");
			}
		}

		#region 监听连接
		/// <summary>
		/// 从客户端开始接受一个连接操作
		/// Begins an operation to accept a connection request from the client 
		/// </summary>
		private void StartAccept(SocketAsyncEventArgs acceptEventArg)
		{
			if (acceptEventArg == null)
			{
				acceptEventArg = new SocketAsyncEventArgs();
				acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
			}
			else
			{
				// socket must be cleared since the context object is being reused
				acceptEventArg.AcceptSocket = null;
			}

			// 查询等待是否有空闲socket资源
			if(_maxAcceptedSemaphore != null)
				_maxAcceptedSemaphore.WaitOne();

			// 开始处理请求
			bool willRaiseEvent = _listenSocket.AcceptAsync(acceptEventArg);
			if (!willRaiseEvent)
			{
				//IO没有挂起，操作同步完成
				ProcessAccept(acceptEventArg);
			}
		}

		/// <summary>
		/// 处理Accept请求
		/// </summary>
		private void ProcessAccept(object obj)
		{
			SocketAsyncEventArgs e = obj as SocketAsyncEventArgs;

			//如果失败
			if (e.SocketError != SocketError.Success)
			{
				MoLog.Log(ELogType.Error, "ProcessConnected error : {0}", e.SocketError);
				return;
			}

			//创建频道
			MoTChannel channel = new MoTChannel();
			channel.InitSocket(e.AcceptSocket);

			//添加频道
			AddChannel(channel);

			// 投递下一个接收请求
			StartAccept(e);
		}
		#endregion

		#region 主动连接
		/// <summary>
		/// 开始连接
		/// </summary>
		public void ConnectAsync(IPEndPoint local, IPEndPoint remote)
		{
			SocketAsyncEventArgs args = new SocketAsyncEventArgs();
			args.RemoteEndPoint = remote;
			args.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);

			//异步连接
			Socket clientSock = new Socket(local.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			bool willRaiseEvent = clientSock.ConnectAsync(args);
			if (!willRaiseEvent)
			{
				ProcessConnected(args);
			}
		}

		private void ProcessConnected(object obj)
		{
			SocketAsyncEventArgs e = obj as SocketAsyncEventArgs;

			//如果失败
			if (e.SocketError != SocketError.Success)
			{
				MoLog.Log(ELogType.Error, "ProcessConnected error : {0}", e.SocketError);
				return;
			}

			//创建频道
			MoTChannel channel = new MoTChannel();
			channel.InitSocket(e.ConnectSocket);

			//加入到频道列表
			lock (_allChannels)
			{
				_allChannels.Add(channel);
			}
		}
		#endregion

		private void RemoveChannel(MoTChannel channel)
		{
			channel.Dispose();

			//从频道列表里删除
			lock (_allChannels)
			{
				_allChannels.Remove(channel);
			}

			//信号减弱
			if (_maxAcceptedSemaphore != null)
				_maxAcceptedSemaphore.Release();
		}
		private void AddChannel(MoTChannel channel)
		{
			//加入到频道列表
			lock (_allChannels)
			{
				_allChannels.Add(channel);
			}
		}
	}
}