using System.Collections.Generic;
using UnityEngine;

namespace Assets.CityGen.Rewrite
{
    public class BuildingSpaceBuilder
    {
        public void Build(RoadNetwork network, int maxUnsuccesfulCells)
        {
            Dictionary<string, Vector2Int> empties = new Dictionary<string, Vector2Int>();

            void findFirstExistingCell(int startPosition, int x)
            {
                for (int y = startPosition; y <= network.MinMaxY.Max; y += network.CellSize.y) // go through all Y
                {
                    Vector2Int position = network.GetSnappedPosition(new Vector2Int(x, y));
                    string key = network.GetKey(position);
                    if (network.Grid.ContainsKey(key))
                    {
                        int nCount = network.GetNeighbourCountToTheUp(position, 1);
                        if (nCount > 0) //if after this solid block, comes another, then it's a road. return
                            return;
                        collectEmptyCells(y + network.CellSize.y, x);
                    }
                }
                return;
            }

            void collectEmptyCells(int startPosition, int x)
            {
                for (int y = startPosition; y <= network.MinMaxY.Max; y += network.CellSize.y)
                {
                    Vector2Int position = network.GetSnappedPosition(new Vector2Int(x, y));
                    string key = network.GetKey(position);
                    if (empties.ContainsKey(key))
                        continue;

                    bool isEmpty = !network.Grid.ContainsKey(key);

                    if (isEmpty)
                        empties.Add(key, position);

                    if (!isEmpty && empties.Count > 0)
                    {
                        foreach (var e in empties)
                        {
                            network.AddCell(new BuildingSpace(e.Value));
                        }
                        empties.Clear();
                    }

                    if (empties.Count > maxUnsuccesfulCells)
                    {
                        empties.Clear();
                        findFirstExistingCell(position.y, x);
                        return;
                    }

                }
            }

            for (int x = network.MinMaxX.Min; x <= network.MinMaxX.Max; x += network.CellSize.x) // for each X
            {
                findFirstExistingCell(network.MinMaxY.Min, x);
            }
        }
    }
}
