//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace MotionEngine
{
	public enum ELogType
	{
		Log,	
		Error,
		Assert,
		Warning,
		Exception,
	}

	public static class MoLog
	{
		public delegate void LogCallback(ELogType logType, string message);
		private static LogCallback _callback;

		public static void RegisterCallback(LogCallback callback)
		{
			_callback += callback;
		}
		public static void Log(ELogType logType, string format, params object[] args)
		{
			if (_callback != null)
			{
				string message = string.Format(format, args);
				_callback.Invoke(logType, message);
			}
		}
		public static void Log(ELogType logType, string message)
		{
			if (_callback != null)
			{
				_callback.Invoke(logType, message);
			}
		}
	}
}