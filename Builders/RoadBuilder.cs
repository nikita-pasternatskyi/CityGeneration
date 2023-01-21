using System.Collections.Generic;
using UnityEngine;

namespace Assets.CityGen.Rewrite
{
    public class RoadBuilder
    {
        private void AddPositions(Vector2Int[] points, RoadNetwork network)
        {
            foreach (var point in points)
            {
                network.AddCell(new Road(point));
            }
        }

        private Vector2Int[] CreatePositions(Vector2Int startPosition, int cellSize, Vector2Int direction, int count)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int j = 0; j < Mathf.Abs(count); j++)
            {
                var pos = startPosition;
                int multiplier = count < 0 ? 1 : -1;
                pos += direction * multiplier * j * cellSize;
                positions.Add(pos);
            }
            return positions.ToArray();
        }

        public void Build(RoadNetwork network, SegmentInt[] segments)
        {
            Vector2Int cellSize = network.CellSize;
            foreach (var segment in segments)
            {
                for (int i = 0; i < segment.Points.Count - 1; i++)
                {
                    Vector2Int point = segment.Points[i];
                    Vector2Int nextPoint = segment.Points[i + 1];

                    var difference = point - nextPoint;
                    int countX = (difference.x / cellSize.x);
                    int countY = (difference.y / cellSize.y);

                    Vector2Int XDirection = new Vector2Int(1, 0);
                    Vector2Int YDirection = new Vector2Int(0, 1);
                    var xPositions = CreatePositions(point, cellSize.x, XDirection, countX);
                    var yPositions = CreatePositions(point, cellSize.y, YDirection, countY);
                    AddPositions(xPositions, network);
                    AddPositions(yPositions, network);
                }
            }
        }
    }
}
