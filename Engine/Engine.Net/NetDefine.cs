//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************

namespace MotionEngine.Net
{
	public class NetDefine
	{
		public const int BufferSize = 128 * 1024; // 数据缓冲区大小
		public const int PackageMaxSize = 32 * 1024; // 网络包最大长度
		public const int PackageHeadSize = 22; //包头长度
		public const short PackageHeadMark = 0x55AA; //包头标记
	}
}