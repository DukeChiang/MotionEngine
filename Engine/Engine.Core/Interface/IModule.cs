//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;

namespace MotionEngine
{
	public interface IModule
    {
		void Awake();
		void Start();
		void Update();
    }
}