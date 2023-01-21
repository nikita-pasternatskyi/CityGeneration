using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CityGen.Rewrite
{
    public class RoadNetwork : SerializedScriptableObject
    {
        public Dictionary<string, Cell> Grid;
        public Vector2Int CellSize;
        public Range MinMaxX = new Range(int.MinValue, int.MaxValue);
        public Range MinMaxY = new Range(int.MinValue, int.MaxValue);

        public const string SEPARATOR = ".";

        public int GetNeighboursCount(Vector2Int cellPosition)
        {
            return 
                GetNeighbourCountToTheRight(cellPosition, 1) + 
                GetNeighbourCountToTheDown(cellPosition, 1) + 
                GetNeighbourCountToTheLeft(cellPosition, 1) + 
                GetNeighbourCountToTheUp(cellPosition, 1);
        }

        public int GetNeighbourCountToTheRight(Vector2Int cellPosition, int distanceInCells)
        {
            int count = 0;
            Vector2Int currentPosition = cellPosition;
            int offset = CellSize.x;

            List<Vector2Int> possiblePositions = new List<Vector2Int>(distanceInCells);

            for (int i = 1; i <= distanceInCells; i++)
            {
                possiblePositions.Add(new Vector2Int(currentPosition.x + offset * i, currentPosition.y));
            }

            foreach (var position in possiblePositions)
            {
                var key = GetKey(position);
                if (Grid.ContainsKey(key))
                    count++;
            }
            return count;
        }

        public int GetNeighbourCountToTheLeft(Vector2Int cellPosition, int distanceInCells)
        {
            int count = 0;
            Vector2Int currentPosition = cellPosition;
            int offset = -CellSize.x;

            List<Vector2Int> possiblePositions = new List<Vector2Int>(distanceInCells);

            for (int i = 1; i <= distanceInCells; i++)
            {
                possiblePositions.Add(new Vector2Int(currentPosition.x + offset * i, currentPosition.y));
            }

            foreach (var position in possiblePositions)
            {
                var key = GetKey(position);
                if (Grid.ContainsKey(key))
                    count++;
            }
            return count;
        }

        public int GetNeighbourCountToTheUp(Vector2Int cellPosition, int distanceInCells)
        {
            int count = 0;
            Vector2Int currentPosition = cellPosition;
            int offset = CellSize.y;

            List<Vector2Int> possiblePositions = new List<Vector2Int>(distanceInCells);

            for (int i = 1; i <= distanceInCells; i++)
            {
                possiblePositions.Add(new Vector2Int(currentPosition.x, currentPosition.y + offset * i));
            }

            foreach (var position in possiblePositions)
            {
                var key = GetKey(position);
                if (Grid.ContainsKey(key))
                    count++;
            }
            return count;
        }

        public int GetNeighbourCountToTheDown(Vector2Int cellPosition, int distanceInCells)
        {
            int count = 0;
            Vector2Int currentPosition = cellPosition;
            int offset = -CellSize.y;

            List<Vector2Int> possiblePositions = new List<Vector2Int>(distanceInCells);

            for (int i = 1; i <= distanceInCells; i++)
            {
                possiblePositions.Add(new Vector2Int(currentPosition.x, currentPosition.y + offset * i));
            }

            foreach (var position in possiblePositions)
            {
                var key = GetKey(position);
                if (Grid.ContainsKey(key))
                    count++;
            }
            return count;
        }

        public Vector2Int GetSnappedPosition(Vector3 pos)
        {
            Vector2Int position = new Vector2Int(0, 0);
            position.x = (Mathf.RoundToInt(pos.x / CellSize.x) * CellSize.x);
            position.y = (Mathf.RoundToInt(pos.z / CellSize.y) * CellSize.y);
            return position;
        }

        public Vector2Int GetSnappedPosition(Vector2Int pos)
        {
            Vector2Int position = new Vector2Int(0, 0);
            position.x = (Mathf.RoundToInt(pos.x / CellSize.x) * CellSize.x);
            position.y = (Mathf.RoundToInt(pos.y / CellSize.y) * CellSize.y);
            return position;
        }

        public string GetKey(Vector2Int position)
        {
            int keyA = Mathf.RoundToInt(position.x / CellSize.x);
            int keyB = Mathf.RoundToInt(position.y / CellSize.y);

            return keyA + SEPARATOR + keyB;
        }

        public void AddCell(Cell cell)
        {
            string key = GetKey(cell.Position);
            if (Grid.ContainsKey(key))
                return;
            MinMaxX.UpdateRange(cell.Position.x);
            MinMaxY.UpdateRange(cell.Position.y);
            Grid.Add(key, cell);
        }

        public (int, int) Decode(string key)
        {
            string[] numbers = key.Split(SEPARATOR);

            int first = int.Parse(numbers[0]);
            int last = int.Parse(numbers[1]);

            return (first, last);
        }
    }
}
