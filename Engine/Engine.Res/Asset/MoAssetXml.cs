//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace MotionEngine.Res
{
	public abstract class MoAssetXml
	{
		protected string _path = null;
		protected XmlDocument _xml = null;

		public void Load(string path)
		{
			_path = path;

			//检测文件是否存在
			if (File.Exists(path) == false)
			{
				throw new FileNotFoundException(path);
			}

			//加载数据
			_xml = new XmlDocument();
			_xml.Load(path);

			//解析数据
			try
			{
				ParseData();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		protected abstract void ParseData();
	}
}