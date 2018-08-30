//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System.Collections.Generic;

namespace MotionEngine.AI
{
	/// <summary>
	/// 神经细胞
	/// </summary>
	public class MoNeuron
	{
		public readonly List<float> Weights = new List<float>();
		public readonly List<float> LastDelta = new List<float>();
		public float Output;
		public float Error;

		public MoNeuron(int inputs)
		{
			// each neuron has inputs+1 weights, extra one is the bias value
			for (int i = 0; i < inputs + 1; i++)
			{
				Weights.Add(RandomRange(-1.0f, 1.0f));
				LastDelta.Add(0.0f);
			}

			Output = 0.0f;
			Error = 999999.9f;
		}

		private static readonly System.Random _rand = new System.Random();
		private static float RandomRange(float min, float max)
		{
			return (float)(_rand.NextDouble() * (max - min) + min);
		}
	}
}