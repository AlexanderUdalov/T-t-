using UnityEngine;
using System.Collections;

public static class EasingLerpExtensions
{
    public static Vector3 LerpEaseQuad(this Vector3 start, Vector3 finish, float t, float duration)
    {
        t /= duration;
        if (t < 0.5f)
        {
            return Vector3.Lerp(start, finish, t * t * 2);
        }
        return Vector3.Lerp(start, finish, 1 - (t - 1) * (t - 1) * 2);
    }
    
    public static Quaternion LerpEaseQuad(this Quaternion start, Quaternion finish, float t, float duration)
    {
        t /= duration;
        if (t < 0.5f)
        {
            return Quaternion.Lerp(start, finish, t * t * 2);
        }
        return Quaternion.Lerp(start, finish, 1 - (t - 1) * (t - 1) * 2);
    }

    public static float LerpEaseQuad(this float start, float finish, float t, float duration)
    {
        t /= duration;
        if (t < 0.5f)
        {
            return Mathf.Lerp(start, finish, t * t * 2);
        }
        return Mathf.Lerp(start, finish, 1 - (t - 1) * (t - 1) * 2);
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
