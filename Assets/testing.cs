using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region cone
public static class MakeUVCone
{
    public static GameObject Create(Vector3 top, Vector3 bottom, int sectors, int bands, GameObject go)
    {
        if (bands < 2) bands = 2;
        
        MeshFilter mf = go.GetComponent<MeshFilter> ();
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3> ();
        List<int> tris = new List<int> ();
        List<Vector2> uvs = new List<Vector2> ();

        for (int i = 0; i <= sectors; i++)
        {
            float longitude = (Mathf.PI * 2 * i) / sectors;
            for (int j = 0; j < bands; j++)
            {
                int n = verts.Count;

                float fraction = (float)j / (bands - 1);

                Vector3 radius = Vector3.Lerp( bottom, top, fraction);

                Vector3 conicalPoint = new Vector3 (
                    Mathf.Cos (longitude) * radius.x,
                    Mathf.Lerp( bottom.y, top.y, fraction),
                    Mathf.Sin (longitude) * radius.z);

                verts.Add (conicalPoint);

                Vector2 uvPoint = new Vector2 ((float)i / sectors, (float)j / (bands - 1));
                uvs.Add (uvPoint);

                if (i > 0 && j > 0)
                {
                    tris.Add (n);
                    tris.Add (n - (bands + 1));
                    tris.Add (n - bands);

                    tris.Add (n);
                    tris.Add (n - 1);
                    tris.Add (n - (bands + 1));
                }
            }
        }

        mesh.vertices = verts.ToArray ();
        mesh.triangles = tris.ToArray ();
        mesh.uv = uvs.ToArray ();

        mesh.RecalculateBounds ();
        mesh.RecalculateNormals ();

        mf.mesh = mesh;
        return go;
    }
    
}

#endregion

#region gradient
public static class gradientconvert
    {
        public static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, false, false);
        }

        public static UnityEngine.Gradient LerpNoAlpha(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, true, false);
        }

        public static UnityEngine.Gradient LerpNoColor(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, false, true);
        }

        static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t, bool noAlpha, bool noColor)
        {
            //list of all the unique key times
            var keysTimes = new List<float>();

            if (!noColor)
            {
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    float k = a.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }

                for (int i = 0; i < b.colorKeys.Length; i++)
                {
                    float k = b.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }
            }

            if (!noAlpha)
            {
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    float k = a.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }

                for (int i = 0; i < b.alphaKeys.Length; i++)
                {
                    float k = b.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }
            }

            GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
            GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

            //Pick colors of both gradients at key times and lerp them
            for (int i = 0; i < keysTimes.Count; i++)
            {
                float key = keysTimes[i];
                var clr = Color.LerpUnclamped(a.Evaluate(key), b.Evaluate(key), t);
                clrs[i] = new GradientColorKey(clr, key);
                alphas[i] = new GradientAlphaKey(clr.a, key);
            }

            var g = new UnityEngine.Gradient();
            g.SetKeys(clrs, alphas);

            return g;
        }
    }
    #endregion