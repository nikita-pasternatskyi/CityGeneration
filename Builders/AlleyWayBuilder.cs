using UnityEngine;

namespace Assets.CityGen.Rewrite
{
    public class AlleyWayBuilder
    {
        public void Build(RoadNetwork network)
        {
            T findFirstCellGoingUpOfType<T>(int startPosition, int x, out int yPosition) where T : Cell
            {
                yPosition = startPosition;
                for (int y = startPosition; y <= network.MinMaxY.Max; y += network.CellSize.y) // go through all Y
                {
                    Vector2Int position = network.GetSnappedPosition(new Vector2Int(x, y));
                    string key = network.GetKey(position);
                    yPosition = y;
                    if (network.Grid.ContainsKey(key))
                    {
                        if (network.Grid[key].GetType() == typeof(T))
                            return network.Grid[key] as T;
                    }
                }
                return null;
            }

            T findFirstCellGoingRightOfType<T>(int startPosition, int y, out int xPosition) where T : Cell
            {
                xPosition = startPosition;
                for (int x = startPosition; x <= network.MinMaxX.Max; x += network.CellSize.x) // go through all X
                {
                    Vector2Int position = network.GetSnappedPosition(new Vector2Int(x, y));
                    string key = network.GetKey(position);
                    xPosition = x;
                    if (network.Grid.ContainsKey(key))
                    {
                        if (network.Grid[key].GetType() == typeof(T))
                            return network.Grid[key] as T;
                    }
                }
                return null;
            }

            System.Random random = new System.Random();

            for (int x = network.MinMaxX.Min; x <= network.MinMaxX.Max; x += network.CellSize.x) // for each X
            {
                for (int y = network.MinMaxY.Min; y <= network.MinMaxY.Max;)
                {
                    var downLeftCrossRoad = findFirstCellGoingUpOfType<CrossRoad>(y, x, out int downY);
                    if (downLeftCrossRoad == null)
                    {
                        y += network.CellSize.y;
                        continue;
                    }
                    var upperLeftCrossRoad = findFirstCellGoingUpOfType<CrossRoad>(downY + network.CellSize.y, x, out int upperY);
                    var upperRightCrossRoad = findFirstCellGoingRightOfType<CrossRoad>(x + network.CellSize.x, upperY, out int rightX);

                    if (upperRightCrossRoad == null)
                    {
                        y += network.CellSize.y;
                        continue;
                    }

                    int width = rightX - x;
                    int widthInCells = width / network.CellSize.x - 1;
                    if (widthInCells <= 2)
                    {
                        y += network.CellSize.y;
                        continue;
                    }


                    int difference = upperLeftCrossRoad.Position.y - downLeftCrossRoad.Position.y;
                    int differenceInCells = (difference / network.CellSize.y) - 2;

                    if (differenceInCells < 0)
                    {
                        y += network.CellSize.y;
                        continue;
                    }
                    int number = random.Next(0, differenceInCells);
                    int startYPos = (downLeftCrossRoad.Position.y + network.CellSize.y) + number * network.CellSize.y;
                    startYPos = Mathf.Clamp(startYPos, downLeftCrossRoad.Position.y + network.CellSize.y * 2, downLeftCrossRoad.Position.y + difference - network.CellSize.y * 2);

                    //build the alley!
                    int horizontalSteps = random.Next(2, widthInCells);

                    bool verticalDirection = random.Next(2) == 0;
                    int verticalSteps = 0;
                    int multiplier = 1;
                    if (verticalDirection == false)
                    {
                        verticalSteps = Mathf.Abs(downLeftCrossRoad.Position.y - startYPos) / network.CellSize.y;
                        multiplier = -1;
                    }
                    else if (verticalDirection == true)
                    {
                        verticalSteps = Mathf.Abs(upperY - startYPos) / network.CellSize.y;
                    }

                    for (int i = 0; i < horizontalSteps; i++)
                    {
                        var yPosition = startYPos;
                        var xPosition = x + i * network.CellSize.x;
                        var roundedPosition = network.GetSnappedPosition(new Vector2Int(xPosition, yPosition));
                        var key = network.GetKey(roundedPosition);
                        if (network.Grid.ContainsKey(key))
                        {
                            if (network.Grid[key].GetType() == typeof(BuildingSpace))
                                network.Grid[key] = new AlleyWay(roundedPosition);
                        }
                    }

                    for (int i = 0; i < verticalSteps; i++)
                    {
                        var yPosition = startYPos + i * multiplier * network.CellSize.y;
                        var xPosition = x + horizontalSteps * network.CellSize.x;
                        var roundedPosition = network.GetSnappedPosition(new Vector2Int(xPosition, yPosition));
                        var key = network.GetKey(roundedPosition);
                        if (network.Grid.ContainsKey(key))
                        {
                            if (network.Grid[key].GetType() == typeof(BuildingSpace))
                                network.Grid[key] = new AlleyWay(roundedPosition);
                        }
                    }
                    y = upperY;

                }
            }
        }
    }
}
