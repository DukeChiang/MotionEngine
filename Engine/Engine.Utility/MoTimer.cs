﻿//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************

namespace MotionEngine.Utility
{
	/// <summary>
	/// 计时器基类
	/// </summary>
	public abstract class MoTimer
	{
		protected readonly float _delay;

		//重置变量
		protected float _delayTimer = 0;
		protected bool _isOver = false;
		protected bool _isPause = false;

		public bool IsOver { get { return _isOver; } }
		public bool IsPause { get { return _isPause; } }

		/// <summary>
		/// 计时器
		/// </summary>
		/// <param name="delay">延迟计时时间</param>
		public MoTimer(float delay)
		{
			_delay = delay;
		}

		/// <summary>
		/// 暂停计时器
		/// </summary>
		public void Pause()
		{
			_isPause = true;
		}

		/// <summary>
		/// 恢复计时器
		/// </summary>
		public void Resume()
		{
			_isPause = false;
		}

		/// <summary>
		/// 结束计时器
		/// </summary>
		public void Kill()
		{
			_isOver = true;
		}

		/// <summary>
		/// 延迟剩余时间
		/// </summary>
		public float Remaining()
		{
			if (_isOver)
				return 0f;
			else
				return System.Math.Max(0f, _delay - _delayTimer);
		}

		/// <summary>
		/// 重置计时器
		/// </summary>
		public abstract void Reset();

		/// <summary>
		/// 更新计时器
		/// </summary>
		public abstract bool Update(float deltaTime);
	}

	/// <summary>
	/// 延迟后，执行一次
	/// </summary>
	public class MoOnceTimer : MoTimer
	{
		public MoOnceTimer(float delay) : base(delay)
		{
		}
		public override void Reset()
		{
			_delayTimer = 0;
			_isOver = false;
			_isPause = false;
		}
		public override bool Update(float deltaTime)
		{
			if (_isOver || _isPause)
				return false;

			_delayTimer += deltaTime;
			if (_delayTimer > _delay)
			{
				Kill();
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	/// <summary>
	/// 延迟后，间隔执行
	/// </summary>
	public class MoRepeatTimer : MoTimer
	{
		private readonly float _repeat;

		//重置变量
		private float _repeatTimer = 0;

		public MoRepeatTimer(float delay, float repeat) : base(delay)
		{
			_repeat = repeat;
		}
		public override void Reset()
		{
			_repeatTimer = 0;
			_delayTimer = 0;
			_isOver = false;
			_isPause = false;
		}
		public override bool Update(float deltaTime)
		{
			if (_isOver || _isPause)
				return false;

			_delayTimer += deltaTime;
			if (_delayTimer > _delay)
			{
				_repeatTimer += deltaTime;
				if (_repeatTimer > _repeat)
				{
					_repeatTimer = 0;
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}

	/// <summary>
	/// 延迟后，执行一段时间
	/// </summary>
	public class MoDurationTimer : MoTimer
	{
		private readonly float _duration;

		//重置变量
		private float _durationTimer = 0;

		public MoDurationTimer(float delay, float duration) : base(delay)
		{
			_duration = duration;
		}
		public override void Reset()
		{
			_durationTimer = 0;
			_delayTimer = 0;
			_isOver = false;
			_isPause = false;
		}
		public override bool Update(float deltaTime)
		{
			if (_isOver || _isPause)
				return false;

			_delayTimer += deltaTime;
			if (_delayTimer > _delay)
			{
				_durationTimer += deltaTime;
				if (_durationTimer > _duration)
				{
					Kill();
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}
	}

	/// <summary>
	/// 延迟后，永久执行
	/// </summary>
	public class MoForeverTimer : MoTimer
	{
		public MoForeverTimer(float delay) : base(delay)
		{
		}
		public override void Reset()
		{
			_delayTimer = 0;
			_isOver = false;
			_isPause = false;
		}
		public override bool Update(float deltaTime)
		{
			if (_isOver || _isPause)
				return false;

			_delayTimer += deltaTime;
			if (_delayTimer > _delay)
				return true;
			else
				return false;
		}
	}
}