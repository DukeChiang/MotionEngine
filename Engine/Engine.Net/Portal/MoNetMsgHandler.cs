//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace MotionEngine.Net
{
	internal class MoNetMsgHandler
	{
		private static Dictionary<short, Type> _portals = new Dictionary<short, Type>();

		static MoNetMsgHandler()
		{
			Type[] types = typeof(MoNetMsgHandler).Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];

				if (Attribute.IsDefined(type, typeof(MoNetMsgPortalAttribute)))
				{
					//判断继承关系
					if (!typeof(IPackage).IsAssignableFrom(type))
					{
						string message = string.Format("class {0} does not inherit from MoNetPacket.", type);
						throw new Exception(message);
					}

					//判断是否重复
					MoNetMsgPortalAttribute attribute = (MoNetMsgPortalAttribute)Attribute.GetCustomAttribute(type, typeof(MoNetMsgPortalAttribute));
					if (_portals.ContainsKey(attribute.Portal))
					{
						string message = string.Format("class {0} portal {1} already exist.", type, attribute.Portal);
						throw new Exception(message);
					}

					//添加到列表
					_portals.Add(attribute.Portal, type);
				}
			}
		}

		public static IPackage Handle(short msgType)
		{
			IPackage pakcet = null;
			Type type;
			if (_portals.TryGetValue(msgType, out type))
			{
				pakcet = (IPackage)Activator.CreateInstance(type);
			}

			if (pakcet == null)
			{
				string message = string.Format("Package {0} is not define.", msgType);
				throw new KeyNotFoundException(message);
			}

			return pakcet;
		}
	}
}