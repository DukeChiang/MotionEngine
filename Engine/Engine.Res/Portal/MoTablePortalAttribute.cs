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
	public class MoTablePortalAttribute : Attribute
	{
		public int Portal;

		public MoTablePortalAttribute(int portal)
		{
			Portal = portal;
		}
	}
}