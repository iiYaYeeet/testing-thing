using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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