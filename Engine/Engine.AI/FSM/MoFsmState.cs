//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System.Collections.Generic;

namespace MotionEngine.AI
{
	public abstract class MoFsmState
	{
		public int Type { get; private set; }
		private readonly List<int> _changeToStates = new List<int>();
		
		public MoFsmState(int type)
		{
			Type = type;
		}
		public abstract void Enter();
		public abstract void Execute();
		public abstract void Exit();
		public abstract bool OnMessage(object msg);

		/// <summary>
		/// 添加可转换状态类型
		/// </summary>
		public void AddChangeToState(int stateType)
		{
			if(_changeToStates.Contains(stateType) == false)
			{
				_changeToStates.Add(stateType);
			}
		}

		/// <summary>
		/// 检测是否可以转换到该状态
		/// </summary>
		public bool CanChangeTo(int stateType)
		{
			return _changeToStates.Contains(stateType);
		}
	}
}