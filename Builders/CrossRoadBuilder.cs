using System.Collections.Generic;

namespace Assets.CityGen.Rewrite
{
    public class CrossRoadBuilder
    {
        public void Build(RoadNetwork network)
        {
            List<string> crossRoads = new List<string>();
            foreach (var key in network.Grid.Keys)
            {
                var cell = network.Grid[key];
                int neighbours = network.GetNeighboursCount(cell.Position);

                if (neighbours <= 1)
                    continue;

                if (neighbours >= 3)
                {
                    crossRoads.Add(key);
                    continue;
                }

                bool leftNeighbour = network.GetNeighbourCountToTheLeft(cell.Position, 1) != 0;
                bool rightNeighbour = network.GetNeighbourCountToTheRight(cell.Position, 1) != 0;
                bool upNeighbour = network.GetNeighbourCountToTheUp(cell.Position, 1) != 0;
                bool downNeighbour = network.GetNeighbourCountToTheDown(cell.Position, 1) != 0;

                if (leftNeighbour && rightNeighbour) //if we have neighbours on the left AND on the right
                    continue;
                else if (upNeighbour && downNeighbour) //if we have neighbours on the up AND on the down
                    continue;
                crossRoads.Add(key);
            }

            foreach (var item in crossRoads)
            {
                network.Grid[item] = new CrossRoad(network.Grid[item].Position);
            }
        }
    }
}
