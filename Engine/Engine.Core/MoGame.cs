//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace MotionEngine
{
	public class MoGame
	{
		//模块列表
		private readonly List<IModule> _coms = new List<IModule>();

		/// <summary>
		/// 目标帧率
		/// </summary>
		public double TargetFrameRate { get; set; } = 30;

		/// <summary>
		/// 是否正在运行
		/// </summary>
		public bool IsRunning { get; private set; }


		/// <summary>
		/// 开始游戏循环
		/// </summary>
		public void RunLoop()
		{
			IsRunning = true;

			//检测硬件是否支持高精度计时器
			if (Stopwatch.IsHighResolution == false)
				MoLog.Log(ELogType.Warning, "Stopwatch not support hardware.");

			//模块Start
			StartModule();

			Stopwatch watch = Stopwatch.StartNew();
			long previousFrameTicks = 0;

			//主循环
			while (IsRunning)
			{
				double desiredFrameTime = 1000.0 / TargetFrameRate;
				long currentFrameTicks = watch.ElapsedTicks;
				double deltaMilliseconds = (currentFrameTicks - previousFrameTicks) * (1000.0 / Stopwatch.Frequency);

				//限制帧率
				while (deltaMilliseconds < desiredFrameTime)
				{
					Thread.Sleep(0);
					currentFrameTicks = watch.ElapsedTicks;
					deltaMilliseconds = (currentFrameTicks - previousFrameTicks) * (1000.0 / Stopwatch.Frequency);
				}

				previousFrameTicks = currentFrameTicks;
				double deltaSeconds = deltaMilliseconds / 1000.0;

				//同步时间
				double realtimeStartup = watch.ElapsedTicks * (1000.0 / Stopwatch.Frequency) / 1000.0;
				MoTime.SyncFrame((float)deltaSeconds, realtimeStartup);

				//模块Update
				UpdateModule();
			}

			watch.Stop();
		}

		/// <summary>
		/// 退出
		/// </summary>
		public void Exit()
		{
			IsRunning = false;
		}

		/// <summary>
		/// 注册游戏模块
		/// </summary>
		public void RegisterModule(IModule module)
		{
			if (module == null)
				throw new ArgumentNullException();

			//添加到列表
			_coms.Add(module);
			//模块Awake
			module.Awake();
		}

		private void StartModule()
		{
			for (int i = 0; i < _coms.Count; i++)
			{
				_coms[i].Start();
			}
		}
		private void UpdateModule()
		{
			for (int i = 0; i < _coms.Count; i++)
			{
				_coms[i].Update();
			}
		}
	}
}