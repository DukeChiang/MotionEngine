//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************

namespace MotionEngine
{
	public static class MoTime
	{
		/// <summary>
		/// The time in seconds it took to complete the last frame.
		/// </summary>
		public static float DeltaTime { get; private set; }

		/// <summary>
		/// The real time in seconds since the server started.
		/// </summary>
		public static double RealtimeStartup { get; private set; }

		/// <summary>
		/// The total number of frames that have passed.
		/// </summary>
		public static long FrameCount { get; private set; }

		internal static void SyncFrame(float deltaTime, double realtimeStartup)
		{
			DeltaTime = deltaTime;
			RealtimeStartup = realtimeStartup;
			FrameCount++;
		}
	}
}