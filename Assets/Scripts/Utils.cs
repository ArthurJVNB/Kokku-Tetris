using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void ChangeChildTags(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            child.tag = tag;
        }
    }

    public static Transform[] GetTransformsFromColliders(Collider[] colliders)
    {
        Transform[] transforms = new Transform[colliders.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i] = colliders[i].transform;
        }

        return transforms;
    }

    public static Transform[] GetTransformsByTag(Transform[] transforms, string tag)
    {
        List<Transform> filtered = new List<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].CompareTag(tag))
                filtered.Add(transforms[i]);
        }

        transforms = new Transform[filtered.Count];
        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i] = filtered[i];
        }

        return transforms;
    }
}
