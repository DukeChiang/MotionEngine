//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System.Numerics;

namespace MotionEngine.Math
{
    public class MoTransform
    {
        private Vector3 _position = Vector3.Zero;
        private Quaternion _rotation = Quaternion.Identity;


        public MoTransform()
        {
        }

        public Vector3 Position
        {
            get { return _position; }
            set
            {
#if DEBUG
                Vector3Helper.CheckInvalid(value);
#endif
                _position = value;
            }
        }
        public Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
#if DEBUG
                QuaternionHelper.CheckInvalid(value);
#endif
                _rotation = QuaternionHelper.NormalizeSafe(value);
            }
        }
        public Vector3 EulerAngle
        {
            get
            {
                return QuaternionHelper.QuaternionToEuler(_rotation);
            }
            set
            {
#if DEBUG
                Vector3Helper.CheckInvalid(value);
#endif
                Rotation = QuaternionHelper.EulerToQuaternion(value);
            }
        }

        public Vector3 Right
        {
            get { return TransformDirection(Vector3.UnitX); }
        }
        public Vector3 Up
        {
            get { return TransformDirection(Vector3.UnitY); }
        }
        public Vector3 Forward
        {
            get { return TransformDirection(Vector3.UnitZ); }
        }

        /// <summary>
        /// transforms a point from localspace to worldspace
        /// </summary>
        public Vector3 TransformPoint(Vector3 inPoint)
        {
            inPoint = QuaternionHelper.RotateVectorByQuat(_rotation, inPoint);
            inPoint += _position;
            return inPoint;
        }

        /// <summary>
        /// Transforms a direction from localspace to worldspace
        /// (Ignores scale)
        /// </summary>
        public Vector3 TransformDirection(Vector3 inDirection)
        {
            return QuaternionHelper.RotateVectorByQuat(_rotation, inDirection);
        }

        /// <summary>
        /// Transforms a point from worldspace to localspace
        /// </summary>
        public Vector3 InverseTransformPoint(Vector3 inPoint)
        {
            Vector3 newPosition, localPosition;
            localPosition = inPoint;
            localPosition -= _position;
            newPosition = QuaternionHelper.RotateVectorByQuat(Quaternion.Conjugate(_rotation), localPosition);
            return newPosition;
        }

        /// <summary>
        /// Transforms a direction from worldspace to localspace
        /// (Ignores scale)
        /// </summary>
        public Vector3 InverseTransformDirection(Vector3 inDirection)
        {
            return QuaternionHelper.RotateVectorByQuat(Quaternion.Conjugate(_rotation), inDirection);
        }

        /// <summary>
        /// Translate world position
        /// </summary>
        public void Translate(Vector3 translation)
        {
            Position += translation;
        }

        /// <summary>
        /// Rotates the transform around axis by angle
        /// </summary>
        public void Rotate(Vector3 worldPoint, Vector3 worldAxis, float degreeAngle)
        {
            // 先移动位置
            Quaternion q = QuaternionHelper.AxisAngleToQuaternionSafe(worldAxis, MoMath.Deg2Rad(degreeAngle));
            Vector3 dif = _position - worldPoint;
            dif = QuaternionHelper.RotateVectorByQuat(q, dif);
            Position = worldPoint + dif;

            // 再移动朝向
            Rotate(worldAxis, degreeAngle);
        }

        /// <summary>
        /// Rotates the transform around axis by angle
        /// </summary>
        public void Rotate(Vector3 worldAxis, float degreeAngle)
        {
            float radianAngle = MoMath.Deg2Rad(degreeAngle);
            Vector3 localAxis = InverseTransformDirection(worldAxis);
            if (localAxis.LengthSquared() > Vector3Helper.Epsilon)
            {
                localAxis = Vector3Helper.NormalizeSafe(localAxis);
                Quaternion q = QuaternionHelper.AxisAngleToQuaternionSafe(localAxis, radianAngle);
                Rotation = QuaternionHelper.NormalizeSafe(_rotation * q);
            }
        }

        /*
		/// <summary>
		/// Look at the world position
		/// </summary>
		public void LookAt(Vector3 worldPosition, Vector3 worldUp)
		{
			Vector3 forward = worldPosition - _position;
			Quaternion q = Quaternion.Identity;
			if (Quaternion.LookRotationToQuaternion(forward, worldUp, out q))
				Rotation = q;
			else
			{
				// 通过矩阵计算四元数
			}
		}
		*/
    }
}