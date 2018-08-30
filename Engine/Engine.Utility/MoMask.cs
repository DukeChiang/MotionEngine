//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;

namespace MotionEngine.Utility
{
    public struct MoMask32
    {
        private int _mask;

        public static implicit operator int(MoMask32 mask) { return mask._mask; }
        public static implicit operator MoMask32(int mask) { return new MoMask32(mask); }

        public MoMask32(int mask)
        {
            _mask = mask;
        }

        /// <summary>
        /// 打开位
        /// </summary>
        public void Open(int bit)
        {
			if (bit < 0 || bit > 31)
				throw new ArgumentOutOfRangeException();
            else
                _mask |= 1 << bit;
        }

        /// <summary>
        /// 关闭位
        /// </summary>
        public void Close(int bit)
        {
            if (bit < 0 || bit > 31)
				throw new ArgumentOutOfRangeException();
			else
                _mask &= ~(1 << bit);
        }

        /// <summary>
        /// 位取反
        /// </summary>
        public void Reverse(int bit)
        {
            if (bit < 0 || bit > 31)
				throw new ArgumentOutOfRangeException();
			else
                _mask ^= 1 << bit;
        }

		/// <summary>
		/// 所有位取反
		/// </summary>
		public void Inverse()
		{
			_mask = ~_mask;
		}

		/// <summary>
		/// 比对位值
		/// </summary>
		public bool Test(int bit)
        {
            if (bit < 0 || bit > 31)
				throw new ArgumentOutOfRangeException();
			else
				return (_mask & (1 << bit)) == 0 ? false : true;
        }
    }
}