using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{

    private const float dotThreshold = 0.5f;
    
    //是否在正面扇形范围内
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        //点乘判断方向
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;
    }
}
