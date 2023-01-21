using Assets.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.CityGen.Rewrite
{
    public class RoadNetworkBuilder : MonoBehaviour
    {
        //unity specific stuff
        [SerializeField] private string _name = "road network";
        [SerializeField] private int _maxUnsuccesfulEmptyCells = 50;
        [SerializeField] private GameObject _roadPrefab;

        //builders, this is to separate everything in classes
        private RoadBuilder _roadBuilder;
        private CrossRoadBuilder _crossRoadBuilder;
        private BuildingSpaceBuilder _buildingSpaceBuilder;
        private AlleyWayBuilder _alleyWayBuilder;

        [Button]
        public void BuildNetwork()
        {
            CreateBuilders();

            RoadNetwork network = CreateRoadNetwork();
            network.Grid = new Dictionary<string, Cell>();
            network.CellSize = GetCellSizeFromGameObject(_roadPrefab);

            RoadSegment[] segments = transform.GetComponentsInChildren<RoadSegment>();
            SegmentInt[] roadSegments = GetSnappedToGridSegments(segments,network);

            _roadBuilder.Build(network, roadSegments);
            _crossRoadBuilder.Build(network);
            _buildingSpaceBuilder.Build(network, _maxUnsuccesfulEmptyCells);
            _alleyWayBuilder.Build(network);
            SaveRoadNetwork(network);
        }

        private SegmentInt[] GetSnappedToGridSegments(SegmentLine[] segments, RoadNetwork network)
        {
            List<SegmentInt> result = new List<SegmentInt>();

            foreach (var segment in segments)
            {
                SegmentInt roundedSegment = new SegmentInt();
                roundedSegment.Points = new List<Vector2Int>();
                foreach (var point in segment.Points)
                {
                    roundedSegment.Points.Add(network.GetSnappedPosition(point.position));
                }
                result.Add(roundedSegment);
            }
            return result.ToArray();
        }

        private Vector2Int GetCellSizeFromGameObject(GameObject go)
        {
            var roadSize = Spatials.GetMaxBounds(go);

            Vector2Int cellSize = new Vector2Int(0, 0);
            cellSize.x = (int)roadSize.extents.x * 2;
            cellSize.y = (int)roadSize.extents.z * 2;

            return cellSize;
        }

        #region UnitySpecificThings
        private RoadNetwork CreateRoadNetwork()
        {
            RoadNetwork roadNetwork = ScriptableObject.CreateInstance<RoadNetwork>();
            roadNetwork.name = _name;
            EditorUtility.SetDirty(roadNetwork);
            return roadNetwork;
        }
        private void SaveRoadNetwork(RoadNetwork network)
        {
            AssetDatabase.CreateAsset(network, "Assets/" + network.name.Replace(" ", "") + ".asset");
            AssetDatabase.SaveAssets();
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            foreach (var item in transform.GetComponentsInChildren<RoadSegment>())
            {
                for (int i = 0; i < item.Points.Length - 1; i++)
                {
                    var current = item.Points[i];
                    var next = item.Points[i + 1];
                    Gizmos.DrawLine(current.position, next.position);
                }
            }

            Gizmos.color = Color.red;
            foreach (var item in transform.GetComponentsInChildren<Separator>())
            {
                for (int i = 0; i < item.Points.Length - 1; i++)
                {
                    var current = item.Points[i];
                    var next = item.Points[i + 1];
                    Gizmos.DrawLine(current.position, next.position);
                }
            }
        }

        [Button]
        public void AddSegment()
        {
            var segmentGO = new GameObject();
            var a = new GameObject();
            var b = new GameObject();

            segmentGO.AddComponent<RoadSegment>();
            segmentGO.transform.parent = transform;
            a.transform.parent = segmentGO.transform;
            b.transform.parent = segmentGO.transform;

            Selection.activeObject = b;
        }
        private void CreateBuilders()
        {
            void CreateBuilder<T>(ref T check) where T : new()
            {
                if (check == null)
                    check = new T();
            }

            CreateBuilder(ref _roadBuilder);
            CreateBuilder(ref _crossRoadBuilder);
            CreateBuilder(ref _buildingSpaceBuilder);
            CreateBuilder(ref _alleyWayBuilder);
        }
        #endregion
    }
}
