using System.Collections.Generic;
using System.Linq;
using Runtime.Data;
using UnityEngine;

namespace Editor.Utils {
    public class BuildAreaBaker {
        public static void Bake(List<BuildArea.Quad> quads, float meshThickness, Mesh mesh) {
            mesh.Clear();
            
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            for (var quadIndex = 0; quadIndex < quads.Count; quadIndex++) {
                // sort vertices clockwise 
                var quadVertices = SortVertices(new List<Vector3>(quads[quadIndex].Points));
                
                // generate part of mesh for current quad
                var (quadVerts, quadTris) = GetMeshForQuad(quadVertices, meshThickness, quadIndex);
                vertices.AddRange(quadVerts);
                triangles.AddRange(quadTris);
            }
            
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.Optimize();
            mesh.RecalculateNormals();
        }
        
        private static (List<Vector3> vertices, List<int> triangles) GetMeshForQuad(List<Vector3> quadVertices, float thickness, int quadIndex) {
            var triangleOffset = quadIndex * 8;
            var halfThicknessVector3Up = thickness * 0.5f * Vector3.up;
            return (new List<Vector3> {
                quadVertices[0] + halfThicknessVector3Up,
                quadVertices[1] + halfThicknessVector3Up,
                quadVertices[2] + halfThicknessVector3Up,
                quadVertices[3] + halfThicknessVector3Up,
                quadVertices[0] - halfThicknessVector3Up,
                quadVertices[1] - halfThicknessVector3Up,
                quadVertices[2] - halfThicknessVector3Up,
                quadVertices[3] - halfThicknessVector3Up
            }, new List<int> {
                1 + triangleOffset, 0 + triangleOffset, 2 + triangleOffset, 2 + triangleOffset, 0 + triangleOffset, 3 + triangleOffset,
                4 + triangleOffset, 5 + triangleOffset, 6 + triangleOffset, 4 + triangleOffset, 6 + triangleOffset, 7 + triangleOffset,
                5 + triangleOffset, 4 + triangleOffset, 1 + triangleOffset, 4 + triangleOffset, 0 + triangleOffset, 1 + triangleOffset,
                7 + triangleOffset, 6 + triangleOffset, 3 + triangleOffset, 6 + triangleOffset, 2 + triangleOffset, 3 + triangleOffset, 
                4 + triangleOffset, 7 + triangleOffset, 0 + triangleOffset, 7 + triangleOffset, 3 + triangleOffset, 0 + triangleOffset, 
                6 + triangleOffset, 5 + triangleOffset, 2 + triangleOffset, 5 + triangleOffset, 1 + triangleOffset, 2 + triangleOffset
            });
        }
        
        private static List<Vector3> SortVertices(List<Vector3> inVertices) {
            var centroid = inVertices.Aggregate(Vector3.zero, (current, inVertex) => current + inVertex) / inVertices.Count;
            var outVertices = inVertices.ToList();
            outVertices.Sort((a, b) => Compare(a, b, centroid));
            return outVertices;
        }

        private static int Compare(Vector3 a, Vector3 b, Vector3 centroid) {
            // ignoring Y value
            
            // edge cases
            if (a.x - centroid.x >= 0.0f && b.x - centroid.x < 0.0f) return -1;
            if (a.x - centroid.x < 0.0f && b.x - centroid.x >= 0.0f) return 1;
            if (Mathf.Approximately(a.x - centroid.x, 0.0f) && Mathf.Approximately(b.x - centroid.x, 0.0f)) {
                if (a.z - centroid.z >= 0 || b.z - centroid.z >= 0)
                    return a.z > b.z ? -1 : 1;
                return b.z > a.z ? -1 : 1;
            }

            var det = (a.x - centroid.x) * (b.z - centroid.z) - (b.x - centroid.x) * (a.z - centroid.z);
            if (det < 0) return -1;
            if (det > 0) return 1;
            
            // same line from center, sort by closer
            var dstA = (a - centroid).sqrMagnitude;
            var dstB = (b - centroid).sqrMagnitude;
            return dstA > dstB ? -1 : 1;
        }
    }
}