using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhysicsSim
{
    internal static class AdvancedMath
    {
        public static float Deg2Rad = ((float)Math.PI * 2) / 360;
        public static float Rad2Deg = 360 / ((float)Math.PI * 2);

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime) // Obs Denna funktionen är tagen direkt från Unity där den är inbyggd. Jag skäms men jag behöver den för att göra någon dampening
        {
            smoothTime = Math.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            float num4 = current - target;
            float num5 = target;
            float num6 = maxSpeed * smoothTime;
            num4 = Math.Clamp(num4, -num6, num6);
            target = current - num4;
            float num7 = (currentVelocity + num * num4) * deltaTime;
            currentVelocity = (currentVelocity - num * num7) * num3;
            float num8 = target + (num4 + num7) * num3;
            if (num5 - current > 0f == num8 > num5)
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }

        public static float AngleBetween(Vector2 a, Vector2 B)
        {
            float dotProd;
            float Ratio;
            dotProd = Vector2.Dot(a, B);
            Ratio = dotProd / a.Length();
            return (float)(Math.Acos(Ratio)) * Rad2Deg;
        }

        public static float Vector2Distance(Vector2 a, Vector2 b)
        {
            return Magnitude(a - b);
        }

        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static Vector2 Rotate(Vector2 v, float degrees) //Stulen från unity
        {
            float sin = (float)Math.Sin(degrees * Deg2Rad);
            float cos = (float)Math.Cos(degrees * Deg2Rad);

            float tx = v.X;
            float ty = v.Y;
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
            return v;
        }

        //public double ConvertToRadians(double angle) //Stulen från unity
        //{
        //    return (Math.PI / 180) * angle;
        //}

        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)//Stulen från unity
        {
            float sqrMagnitude = AdvancedMath.sqrMagnitude(vector);
            if (sqrMagnitude > maxLength * maxLength)
            {
                float mag = (float)Math.Sqrt(sqrMagnitude);
                //these intermediate variables force the intermediate result to be
                //of float precision. without this, the intermediate result can be of higher
                //precision, which changes behavior.
                float normalized_x = vector.X / mag;
                float normalized_y = vector.Y / mag;
                return new Vector2(normalized_x * maxLength,
                    normalized_y * maxLength);
            }
            return vector;
        }

        public static float sqrMagnitude(Vector2 temp) //Stulen från unity
        {
            return temp.X * temp.X + temp.Y * temp.Y;
        }

        public static float Magnitude(Vector2 temp) //Stulen från unity
        {
            return (float)Math.Sqrt(sqrMagnitude(temp));
        }

        public static bool isDistanceLessThan(int[] vector2, int[] vector21, double length)
        {
            vector2[0] -= vector21[0];
            vector2[1] -= vector21[1];
            int vectorLength = (vector2[0] * vector2[0] + vector2[1] * vector2[1]);
            return vectorLength < length * length;
        }
    }
}