//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MotionEngine.Res
{
	public abstract class MoAssetByte
	{
		protected string _path = null;
		protected byte[] _bytes = null;

		public void Load(string path)
		{
			_path = path;

			//检测文件是否存在
			if (File.Exists(path) == false)
			{
				throw new FileNotFoundException(path);
			}

			//加载数据
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				_bytes = new byte[fs.Length];
				fs.Read(_bytes);
			}

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