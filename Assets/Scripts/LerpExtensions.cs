using UnityEngine;
using System.Collections;

public static class LerpExtensions
{
    public static Vector3 LerpEaseQuad(Vector3 start, Vector3 finish, float elapsedTime, float duration)
    {
        elapsedTime /= duration;
        if (elapsedTime < 0.5f)
        {
            return Vector3.Lerp(start, finish, elapsedTime * elapsedTime * 2);
        }
        return Vector3.Lerp(start, finish, 1 - (elapsedTime - 1) * (elapsedTime - 1) * 2);
    }

    public static Color LerpEaseQuad(Color start, Color finish, float elapsedTime, float duration)
    {
        elapsedTime /= duration;
        if (elapsedTime < 0.5f)
        {
            return Color.Lerp(start, finish, elapsedTime * elapsedTime * 2);
        }
        return Color.Lerp(start, finish, 1 - (elapsedTime - 1) * (elapsedTime - 1) * 2);
    }

    public static Quaternion LerpEaseQuad(Quaternion start, Quaternion finish, float elapsedTime, float duration)
    {
        elapsedTime /= duration;
        if (elapsedTime < 0.5f)
        {
            return Quaternion.Lerp(start, finish, elapsedTime * elapsedTime * 2);
        }
        return Quaternion.Lerp(start, finish, 1 - (elapsedTime - 1) * (elapsedTime - 1) * 2);
    }

    public static float LerpEaseQuad(float start, float finish, float elapsedTime, float duration)
    {
        elapsedTime /= duration;
        if (elapsedTime < 0.5f)
        {
            return Mathf.Lerp(start, finish, elapsedTime * elapsedTime * 2);
        }
        return Mathf.Lerp(start, finish, 1 - (elapsedTime - 1) * (elapsedTime - 1) * 2);
    }

    public static float LerpAngleEaseQuad(float start, float finish, float elapsedTime, float duration)
    {
        elapsedTime /= duration;
        if (elapsedTime < 0.5f)
        {
            return Mathf.LerpAngle(start, finish, elapsedTime * elapsedTime * 2);
        }
        return Mathf.LerpAngle(start, finish, 1 - (elapsedTime - 1) * (elapsedTime - 1) * 2);
    }
    
    //public static Vector3 LerpEaseQuart(this Vector3 start, Vector3 finish, float t, float duration)
    //{
    //    t /= duration;
    //    if (t < 0.5f)
    //    {
    //        return Vector3.Lerp(start, finish, t * t * 2);
    //    }
    //    return Vector3.Lerp(start, finish, 3 - t * (t - 2));
    //}

    //public static Quaternion LerpEaseQuart(this Quaternion start, Quaternion finish, float t, float duration)
    //{
    //    t /= duration;
    //    if (t < 0.5f)
    //    {
    //        return Quaternion.Lerp(start, finish, t * t * 2);
    //    }
    //    return Quaternion.Lerp(start, finish, 3 - t * (t - 2));
    //}
}
