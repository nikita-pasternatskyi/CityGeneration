using UnityEngine;

namespace Assets.CityGen.Rewrite
{
    public class RoadVision : MonoBehaviour
    {
        [SerializeField] private RoadNetwork _roadNetwork;
        [SerializeField] private bool _drawRoads = true;
        [SerializeField] private bool _drawCrossRoad = true;
        [SerializeField] private bool _drawBuidlingSpace = true;
        [SerializeField] private bool _drawAlleyWays = true;

        private void OnDrawGizmos()
        {
            if (_roadNetwork == null || _roadNetwork.Grid == null)
                return;
            var size = new Vector3(_roadNetwork.CellSize.x, 0, _roadNetwork.CellSize.y);

            foreach (var cell in _roadNetwork.Grid)
            {
                var color = Color.white;
                var type = cell.Value.GetType();

                if (type == typeof(Road))
                {
                    if (_drawRoads == false)
                        continue;
                }
                else if (type == typeof(CrossRoad))
                {
                    if (_drawCrossRoad == false)
                        continue;
                    color = Color.red;
                }
                else if (type == typeof(BuildingSpace))
                {
                    if (_drawBuidlingSpace == false)
                        continue;
                    color = Color.green;
                }
                else if (type == typeof(AlleyWay))
                {
                    if (_drawAlleyWays == false)
                        continue;
                    color = Color.yellow;
                }

                color.a = 0.5f;
                Gizmos.color = color;
                var pos = cell.Value.Position;
                Gizmos.DrawCube(new Vector3(pos.x, 0, pos.y), size);
            }
        }
    }

}
