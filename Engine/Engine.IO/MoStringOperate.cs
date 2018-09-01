//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MotionEngine.IO
{
	/// </summary>
	/// 字符串操作类
	/// 注意：Span<T>需要C# 7.2支持
	/// </summary>
	public static class MoStringOperate
	{
		private static string _operateStr;
		private static int _operateIndex = 0;

		/// <summary>
		/// 设置操作对象
		/// </summary>
		public static void SetOperateString(string str)
		{
			_operateStr = str;
			_operateIndex = 0;
		}

		public static bool NextFloat(char separator, out float value)
		{
			value = 0;

#if UNITY_ENGINE
			string span = MoveNext(separator);
#else
			ReadOnlySpan<char> span = MoveNext(separator);
#endif

			if (span == null)
			{
				return false;
			}
			else
			{
				value = Single.Parse(span);
				return true;
			}
		}
		public static bool NextDouble(char separator, out double value)
		{
			value = 0;

#if UNITY_ENGINE
			string span = MoveNext(separator);
#else
			ReadOnlySpan<char> span = MoveNext(separator);
#endif

			if (span == null)
			{
				return false;
			}
			else
			{
				value = Double.Parse(span);
				return true;
			}
		}
		public static bool NextInt(char separator, out int value)
		{
			value = 0;

#if UNITY_ENGINE
			string span = MoveNext(separator);
#else
			ReadOnlySpan<char> span = MoveNext(separator);
#endif

			if (span == null)
			{
				return false;
			}
			else
			{
				value = Int32.Parse(span);
				return true;
			}
		}
		public static bool NextLong(char separator, out long value)
		{
			value = 0;

#if UNITY_ENGINE
			string span = MoveNext(separator);
#else
			ReadOnlySpan<char> span = MoveNext(separator);
#endif

			if (span == null)
			{
				return false;
			}
			else
			{
				value = Int64.Parse(span);
				return true;
			}
		}
		public static bool NextString(char separator, out string value)
		{
			value = null;

#if UNITY_ENGINE
			string span = MoveNext(separator);
#else
			ReadOnlySpan<char> span = MoveNext(separator);
#endif

			if (span == null)
			{
				return false;
			}
			else
			{
				value = span.ToString();
				return true;
			}
		}


#if UNITY_ENGINE
		private static string MoveNext(char separator)
#else
		private static ReadOnlySpan<char> MoveNext(char separator)
#endif
		{
			int beginIndex = _operateIndex;

			for (int i = _operateIndex; i < _operateStr.Length; i++)
			{
				bool isLastChar = _operateIndex == _operateStr.Length - 1;
				bool isSeparatorChar = _operateStr[i] == separator;

				if (isSeparatorChar || isLastChar)
				{
					if (isLastChar && isSeparatorChar == false)
						_operateIndex++;

					int charCount = _operateIndex - beginIndex;
					if (charCount == 0)
					{
						string message = string.Format("Invalid operate string : {0}", _operateStr);
						throw new InvalidOperationException(message);
					}

					_operateIndex++;

#if UNITY_ENGINE
					return _operateStr.Substring(beginIndex, charCount);
#else
					return _operateStr.AsSpan(beginIndex, charCount);
#endif
				}
				else
				{
					_operateIndex++;
				}
			}

			return null; //移动失败返回NULL
		}
	}
}
