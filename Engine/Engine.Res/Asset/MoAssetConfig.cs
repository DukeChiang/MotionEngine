//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Collections.Generic;
using System.Text;
using MotionEngine.IO;

namespace MotionEngine.Res
{
	/// <summary>
	/// 数据基类
	/// </summary>
	public abstract class MoCfgTab
	{
		public int Id { get; protected set; }
		public abstract void ReadByte(MoByteBuffer byteBuf);
	}

	/// <summary>
	/// 配表基类
	/// </summary>
	public abstract class MoAssetConfig : MoAssetByte
	{
		protected readonly Dictionary<int, MoCfgTab> _tabs = new Dictionary<int, MoCfgTab>();

		/// <summary>
		/// 解析数据
		/// </summary>
		protected override void ParseData()
		{
			MoByteBuffer bb = new MoByteBuffer(_bytes);

			int tabLine = 0;
			const int headMarkAndSize = 6;
			while (bb.IsReadable(headMarkAndSize))
			{
				//检测行标记
				short tabHead = bb.ReadShort();
				if (tabHead != ResDefine.TabStreamHead)
				{
					string message = string.Format("Table stream head is invalid. File is {0} , tab line is {1}", _path, tabLine);
					throw new Exception(message);
				}

				//检测行大小
				int tabSize = bb.ReadInt();
				if (!bb.IsReadable(tabSize) || tabSize > ResDefine.TabStreamMaxLen)
				{
					string message = string.Format("Table stream size is invalid. File is {0}, tab line {1}", _path, tabLine);
					throw new Exception(message);
				}

				//读取行内容
				MoCfgTab tab = null;
				try
				{
					tab = ReadTab(bb);
				}
				catch (Exception ex)
				{
					string message = string.Format("ReadTab falied. File is {0}, tab line {1}. Error : ", _path, tabLine, ex.ToString());
					throw new Exception(message);
				}

				++tabLine;

				//检测是否重复
				if (_tabs.ContainsKey(tab.Id))
				{
					string message = string.Format("The tab key is already exist. Type is {0}, file is {1}, key is {2}", this.GetType(), _path, tab.Id);
					throw new Exception(message);
				}
				else
				{
					_tabs.Add(tab.Id, tab);
				}
			}
		}

		protected abstract MoCfgTab ReadTab(MoByteBuffer byteBuffer);

		public MoCfgTab GetTab(int key)
		{
			if (_tabs.ContainsKey(key))
			{
				return _tabs[key];
			}
			else
			{
				MoLog.Log(ELogType.Warning, "Faild to get tab. File is {0}, key is {1}", _path, key);
				return null;
			}
		}
		public bool TryGetTab(int key, out MoCfgTab tab)
		{
			return _tabs.TryGetValue(key, out tab);
		}
		public bool ContainsKey(int key)
		{
			return _tabs.ContainsKey(key);
		}
		public List<int> GetTabKeys()
		{
			List<int> keys = new List<int>();
			foreach (var tab in _tabs)
			{
				keys.Add(tab.Key);
			}
			return keys;
		}
	}
}