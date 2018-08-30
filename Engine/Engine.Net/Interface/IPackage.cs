//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;

namespace MotionEngine.Net
{
	public interface IPackage
	{
		/// <summary>
		/// 消息类型
		/// </summary>
		Int16 MsgType { get; set; }

		/// <summary>
		/// 包体大小
		/// </summary>
		Int32 MsgSize { get; set; }

		void Decode(MoByteBuffer bb, MoByteBuffer tempDecodeByteBuf);
		void Encode(MoByteBuffer bb, MoByteBuffer tempEncodeByteBuf);
	}
}