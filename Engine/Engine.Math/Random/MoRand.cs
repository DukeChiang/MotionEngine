//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;

namespace MotionEngine.Math
{
	/// <summary>
	/// 效率最高的随机算法
	/// Xorshift 128 implementation
	/// Wikipedia: http://en.wikipedia.org/wiki/Xorshift
	/// </summary>
	public class MoRand
	{
		private UInt32 x, y, z, w;


		public MoRand(UInt32 seed = 0)
		{
			SetSeed(seed);
		}

		public UInt32 GetSeed()
		{
			return x;
		}
		public void SetSeed(UInt32 seed)
		{
			x = seed;
			y = x * 1812433253U + 1;
			z = y * 1812433253U + 1;
			w = z * 1812433253U + 1;
		}

		public UInt32 Get()
		{
			UInt32 t;
			t = x ^ (x << 11);
			x = y; y = z; z = w;
			return w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
		}

		/// <summary>
		/// random number between 0.0 and 1.0
		/// </summary>
		public float GetFloat()
		{
			return GetFloatFromInt(Get());
		}

		/// <summary>
		/// random number between -1.0 and 1.0
		/// </summary>
		public float GetSignedFloat()
		{
			return GetFloat() * 2.0f - 1.0f;
		}

		private static float GetFloatFromInt(UInt32 value)
		{
			// take 23 bits of integer, and divide by 2^23-1
			return (value & 0x007FFFFF) * (1.0f / 8388607.0f);
		}
		private static Byte GetByteFromInt(UInt32 value)
		{
			// take the most significant byte from the 23-bit value
			return (Byte)(value >> (23 - 8));
		}
	}
}
