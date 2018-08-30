//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace MotionEngine.Res
{
	[AttributeUsage(AttributeTargets.Class)]
	public class MoConfigPortalAttribute : Attribute
	{
		public int Portal;

		public MoConfigPortalAttribute(int portal)
		{
			Portal = portal;
		}
	}
}