﻿using UnityEngine;

namespace Runtime {
    public static class Utilities {
        public static bool IsMouseInQuad(Vector2 mouseScreenPosition, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
            return PointTriangle(p0, p1, p2, mouseScreenPosition) ||
                   PointTriangle(p0, p2, p3, mouseScreenPosition);
        }

        public static bool PointTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p) {
            var areaMain = Mathf.Abs((b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y));
            var area1 = Mathf.Abs((a.x - p.x) * (b.y - p.y) - (b.x - p.x) * (a.y - p.y));
            var area2 = Mathf.Abs((b.x - p.x) * (c.y - p.y) - (c.x - p.x) * (b.y - p.y));
            var area3 = Mathf.Abs((c.x - p.x) * (a.y - p.y) - (a.x - p.x) * (c.y - p.y));
            return Mathf.Abs(area1 + area2 + area3 - areaMain) < 0.2f; // I have no idea why such a big tolerance is required but I'm not going to question it
        }
    }
}