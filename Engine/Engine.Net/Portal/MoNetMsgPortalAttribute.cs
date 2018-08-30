//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace MotionEngine.Net
{
	[AttributeUsage(AttributeTargets.Class)]
	public class MoNetMsgPortalAttribute : Attribute
	{
		public short Portal;

		public MoNetMsgPortalAttribute(short portal)
		{
			Portal = portal;
		}
	}
}