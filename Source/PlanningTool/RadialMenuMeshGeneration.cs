using UnityEngine;

namespace PlanningTool
{
    public class RadialMenuMeshGeneration
    {
        /// <summary>
        /// Annulus is the region between two concentric circles.
        ///
        /// Follows Oxygen Not Included world coordinate, +x = right, +y = up, +z = away from camera.
        /// </summary>
        /// <param name="gameObject">GameObject to apply/change MeshFilter and optionally PolygonCollider2D to</param>
        /// <param name="innerRadius">inner radius of the annulus</param>
        /// <param name="outerRadius">outer radius of the annulus</param>
        /// <param name="angle">in degrees, arc of the annulus to generate</param>
        /// <param name="resolution">how many vertices to use per inner & outer circles</param>
        /// <param name="addCollider">if the gameobject should also have a PolygonCollider2D applied, for usage with e.g. MonoBehaviour.OnMouseEnter()</param>
        public static void GenerateAnnulus(GameObject gameObject, float innerRadius, float outerRadius, float angle, int resolution, bool addCollider)
        {
            var radians = angle * Mathf.PI / 180f;
            MeshFilter mf = gameObject.AddOrGet<MeshFilter>();
            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            Vector3[] vertices = new Vector3[resolution * 4];
            int[] tri = new int[resolution * 6];
            Vector3[] normals = new Vector3[resolution * 4];
            Vector2[] uv = new Vector2[resolution * 4];

            for (int i = 0; i < resolution; i++)
            {
                // Calculate the current and next angles
                float currentAngle = i * radians / resolution;
                float nextAngle = (i + 1) * radians / resolution;

                // Calculate the four vertices for each segment
                vertices[i * 4] = new Vector3(Mathf.Sin(currentAngle) * innerRadius, Mathf.Cos(currentAngle) * innerRadius, 0); // inner current
                vertices[i * 4 + 1] = new Vector3(Mathf.Sin(nextAngle) * innerRadius, Mathf.Cos(nextAngle) * innerRadius, 0); // inner next
                vertices[i * 4 + 2] = new Vector3(Mathf.Sin(currentAngle) * outerRadius, Mathf.Cos(currentAngle) * outerRadius, 0); // outer current
                vertices[i * 4 + 3] = new Vector3(Mathf.Sin(nextAngle) * outerRadius, Mathf.Cos(nextAngle) * outerRadius, 0); // outer next

                // Set the triangles for each segment
                // Must be clockwise, as Unity uses left-handed coordinate system
                // tri[i * 6] = i * 4;
                // tri[i * 6 + 1] = i * 4 + 1;
                // tri[i * 6 + 2] = i * 4 + 2;
                // tri[i * 6 + 3] = i * 4 + 1;
                // tri[i * 6 + 4] = i * 4 + 3;
                // tri[i * 6 + 5] = i * 4 + 2;
                tri[i * 6] = i * 4;
                tri[i * 6 + 1] = i * 4 + 2;
                tri[i * 6 + 2] = i * 4 + 1;
                tri[i * 6 + 3] = i * 4 + 1;
                tri[i * 6 + 4] = i * 4 + 2;
                tri[i * 6 + 5] = i * 4 + 3;

                // Set the normals for each vertex
                // Triangles goes clockwise, so normal is +z
                // forward = +z, since unity is left-handed coordinate system, +z is away from camera in world space
                normals[i * 4] = -Vector3.forward;
                normals[i * 4 + 1] = -Vector3.forward;
                normals[i * 4 + 2] = -Vector3.forward;
                normals[i * 4 + 3] = -Vector3.forward;

                // Set the uvs for each vertex
                uv[i * 4] = new Vector2(0, i / (float)resolution);
                uv[i * 4 + 1] = new Vector2(0, (i + 1) / (float)resolution);
                uv[i * 4 + 2] = new Vector2(1, i / (float)resolution);
                uv[i * 4 + 3] = new Vector2(1, (i + 1) / (float)resolution);
            }

            // Initialize the mesh
            mesh.vertices = vertices;
            mesh.triangles = tri;
            mesh.normals = normals;
            mesh.uv = uv;

            // Add a polygon collider if needed
            if (!addCollider)
                return;
            PolygonCollider2D polyCollider = gameObject.AddOrGet<PolygonCollider2D>();
            polyCollider.pathCount = resolution;
            for (int i = 0; i < resolution; i++)
            {
                // Set the path for each segment of the collider
                Vector2[] path = new Vector2[4];
                path[0] = vertices[i * 4];
                path[1] = vertices[i * 4 + 1];
                path[2] = vertices[i * 4 + 3];
                path[3] = vertices[i * 4 + 2];
                polyCollider.SetPath(i, path);
            }
        }
    }
}
