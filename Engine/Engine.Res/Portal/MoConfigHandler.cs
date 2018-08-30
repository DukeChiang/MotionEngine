//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace MotionEngine.Res
{
	internal class MoConfigHandler
	{
		private static Dictionary<int, Type> _portals = new Dictionary<int, Type>();

		static MoConfigHandler()
		{
			Type[] types = typeof(MoConfigHandler).Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];

				if (Attribute.IsDefined(type, typeof(MoConfigPortalAttribute)))
				{
					//判断继承关系
					if (!typeof(MoAssetConfig).IsAssignableFrom(type))
					{
						string message = string.Format("class {0} does not inherit from MoAssetConfig.", type);
						throw new Exception(message);
					}

					//判断是否重复
					MoConfigPortalAttribute attribute = (MoConfigPortalAttribute)Attribute.GetCustomAttribute(type, typeof(MoConfigPortalAttribute));
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

		public static MoAssetConfig Handle(int configType)
		{
			MoAssetConfig config = null;
			Type type;
			if (_portals.TryGetValue(configType, out type))
			{
				config = (MoAssetConfig)Activator.CreateInstance(type);
			}

			if (config == null)
			{
				string message = string.Format("MoAssetConfig {0} is not define.", configType);
				throw new KeyNotFoundException(message);
			}

			return config;
		}
	}
}