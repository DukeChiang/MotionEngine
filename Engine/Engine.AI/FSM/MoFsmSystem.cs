//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;

namespace MotionEngine.AI
{
	public class MoFsmSystem
	{
		private readonly List<MoFsmState> _states = new List<MoFsmState>();

		private MoFsmState _runState;
		private MoFsmState _preState;
		private MoFsmState _globalState;

		public int RunStateType
		{
			get { return _runState != null ? _runState.Type : 0; }
		}
		public int PreStateType
		{
			get { return _preState != null ? _preState.Type : 0; }
		}

		public void Run(int runStateType, int globalStateType)
		{
			_runState = GetState(runStateType);
			_preState = GetState(runStateType);
			_globalState = GetState(globalStateType);
			_runState.Enter();
		}
		public void Update()
		{
			if (_runState != null)
				_runState.Execute();
		}

		/// <summary>
		/// 接收消息
		/// </summary>
		public bool HandleMessage(object msg)
		{
			// If current state can handle message
			if (_runState != null && _runState.OnMessage(msg))
				return true;

			// If not, send the message to global state
			if (_globalState != null && _globalState.OnMessage(msg))
				return true;

			return false;
		}

		/// <summary>
		/// 添加一个状态
		/// </summary>
		public void AddState(MoFsmState state)
		{
			if (state == null)
				throw new ArgumentNullException();

			if (_states.Contains(state) == false)
			{
				_states.Add(state);
			}
		}

		/// <summary>
		/// 改变状态
		/// </summary>
		public void ChangeState(int stateType)
		{
			MoFsmState state = GetState(stateType);
			if (state == null)
			{
				MoLog.Log(ELogType.Error, "Can not found state {0}", stateType);
				return;
			}

			//全局状态不需要检测
			if (_runState.Type != _globalState.Type && state.Type != _globalState.Type)
			{
				if (_runState.CanChangeTo(stateType) == false)
				{
					MoLog.Log(ELogType.Error, "Can not change state {0} to {1}", _runState, state);
					return;
				}
			}

			MoLog.Log(ELogType.Log, "Change state {0} to {1}", _runState, state);
			_preState = _runState;
			_runState.Exit();
			_runState = state;
			_runState.Enter();
		}

		/// <summary>
		/// 返回之前状态
		/// </summary>
		public void RevertToPreState()
		{
			int stateType = _preState != null ? _preState.Type : 0;
			ChangeState(stateType);
		}

		private bool IsContains(int stateType)
		{
			for (int i = 0; i < _states.Count; i++)
			{
				if (_states[i].Type == stateType)
					return true;
			}
			return false;
		}
		private MoFsmState GetState(int stateType)
		{
			for (int i = 0; i < _states.Count; i++)
			{
				if (_states[i].Type == stateType)
					return _states[i];
			}
			return null;
		}
	}
}
