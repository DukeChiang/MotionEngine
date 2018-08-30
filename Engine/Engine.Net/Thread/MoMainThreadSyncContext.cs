//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MotionEngine.Net
{
	/// <summary>
	/// 注意：Unity3D中需要设置Scripting Runtime Version为.NET4.6
	/// </summary>
	public sealed class MoMainThreadSyncContext : SynchronizationContext
	{
		public static MoMainThreadSyncContext Instance = new MoMainThreadSyncContext();

		/// <summary>
		/// 线程同步队列
		/// </summary>
		private readonly ConcurrentQueue<Action> _safeQueue = new ConcurrentQueue<Action>();

		public void Update()
		{
			while (true)
			{
				Action action = null;
				if (_safeQueue.TryDequeue(out action) == false)
					return;
				action.Invoke();
			}
		}

		public override void Post(SendOrPostCallback callback, object state)
		{
			Action action = new Action(() => { callback(state); });
			_safeQueue.Enqueue(action);
		}
	}
}