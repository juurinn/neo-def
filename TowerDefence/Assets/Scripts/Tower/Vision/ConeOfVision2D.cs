using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Draws a mesh that gets blocked by obstacles.
/// </summary>
[Serializable]
public class ConeOfVision2D : MonoBehaviour {

    /// <summary>
    /// Information about raycast hits.
    /// </summary>
    public struct RaycastData {
        public bool Hit;
        public Vector3 Point;
        public float Distance;
        public float Angle;

        public RaycastData(bool hit, Vector3 point, float distance, float angle) {
            Hit = hit;
            Point = point;
            Distance = distance;
            Angle = angle;
        }
    }

    /// <summary>
    /// Mesh edge information.
    /// </summary>
    public struct MeshEdgePosition {
        public Vector3 PointA;
        public Vector3 PointB;

        public MeshEdgePosition(Vector3 pointA, Vector3 pointB) {
            PointA = pointA;
            PointB = pointB;
        }
    }

    [Header("Vision")]
    [Tooltip("What objects block vision.")]
    public LayerMask ObstacleMask;


    [Header("Mesh")]
    [Tooltip("Does the mesh update.")]
    public bool ShouldDrawMesh = true;
    public float MeshDensity = 1f;
    public int EdgePrecision = 3;
    public float EdgeThreshold = 0.5f;

    [Tooltip("Mesh renderers filter goes here.")]
    public MeshFilter VisionMeshFilter;

    private Mesh _visionMesh;

    private List<Vector3> _viewPoints = new List<Vector3>();
    private RaycastData _oldViewCast = new RaycastData();
    private RaycastData _viewCast = new RaycastData();

    private Vector3[] _vertices;
    private int[] _triangles;
    private Vector3 _minPoint, _maxPoint, _direction;
    private RaycastData _returnRaycastData;
    private RaycastHit2D _raycastAtAngleHit2D;
    private int _numberOfVerticesLastTime = 0;

    public float VisionRadius;

    private void Awake() {
        _visionMesh = new Mesh();
        if (ShouldDrawMesh) {
            VisionMeshFilter.mesh = _visionMesh;
        }
    }


    /// <summary>
    /// Updates the vision mesh.
    /// </summary>
    public void UpdateMesh() {
        DrawMesh();
    }


    /// <summary>
    /// Handle vision activation.
    /// </summary>
    /// <param name="_Turret">Turret blueprint for vision values</param>
    public void Config(TurretBlueprint _Turret) {
        gameObject.SetActive(true);
        VisionRadius = _Turret.range;
    }


    /// <summary>
    /// Draws the vision mesh.
    /// </summary>
    private void DrawMesh() {
        if (!ShouldDrawMesh) {
            return;
        }

        int steps = Mathf.RoundToInt(MeshDensity * 360f);
        float stepsAngle = 360f / steps;

        _viewPoints.Clear();

        for (int i = 0; i <= steps; i++) {
            float angle = stepsAngle * i;
            _viewCast = RaycastAtAngle(angle);

            if (i > 0) {
                bool thresholdExceeded = Mathf.Abs(_oldViewCast.Distance - _viewCast.Distance) > EdgeThreshold;

                if ((_oldViewCast.Hit != _viewCast.Hit) || (_oldViewCast.Hit && _viewCast.Hit && thresholdExceeded)) {
                    MeshEdgePosition edge = FindMeshEdgePosition(_oldViewCast, _viewCast);
                    if (edge.PointA != Vector3.zero) {
                        _viewPoints.Add(edge.PointA);
                    }
                    if (edge.PointB != Vector3.zero) {
                        _viewPoints.Add(edge.PointB);
                    }
                }
            }

            _viewPoints.Add(_viewCast.Point);
            _oldViewCast = _viewCast;
        }

        int numberOfVertices = _viewPoints.Count + 1;
        if (numberOfVertices != _numberOfVerticesLastTime) {
            Array.Resize(ref _vertices, numberOfVertices);
            Array.Resize(ref _triangles, (numberOfVertices - 2) * 3);
        }

        _vertices[0].x = 0;
        _vertices[0].y = 0;
        _vertices[0].z = 0;

        for (int i = 0; i < numberOfVertices - 1; i++) {
            _vertices[i + 1] = this.transform.InverseTransformPoint(_viewPoints[i]);

            if (i < numberOfVertices - 2) {
                _triangles[i * 3] = 0;
                _triangles[i * 3 + 1] = i + 1;
                _triangles[i * 3 + 2] = i + 2;
            }
        }

        _visionMesh.Clear();
        _visionMesh.vertices = _vertices;
        _visionMesh.triangles = _triangles;
        _visionMesh.RecalculateNormals();
        _visionMesh.triangles = _visionMesh.triangles.Reverse().ToArray();

        _numberOfVerticesLastTime = numberOfVertices;
    }

    /// <summary>
    /// Finds mesh edges by raycasting.
    /// </summary>
    /// <returns> Positions of mesh edges. </returns>
    private MeshEdgePosition FindMeshEdgePosition(RaycastData minimumViewCast, RaycastData maximumViewCast) {
        float minAngle = minimumViewCast.Angle;
        float maxAngle = maximumViewCast.Angle;
        _minPoint = minimumViewCast.Point;
        _maxPoint = maximumViewCast.Point;

        for (int i = 0; i < EdgePrecision; i++) {
            float angle = (minAngle + maxAngle) / 2;
            RaycastData newViewCast = RaycastAtAngle(angle);

            bool thresholdExceeded = Mathf.Abs(minimumViewCast.Distance - newViewCast.Distance) > EdgeThreshold;
            if (newViewCast.Hit = minimumViewCast.Hit && !thresholdExceeded) {
                minAngle = angle;
                _minPoint = newViewCast.Point;
            } else {
                maxAngle = angle;
                _maxPoint = newViewCast.Point;
            }
        }

        return new MeshEdgePosition(_minPoint, _maxPoint);
    }

    /// <summary>
    /// Raycasts at an angle.
    /// </summary>
    /// <param name="angle"> Angle to raycast at. </param>
    /// <returns> Raycast hit information. </returns>
    private RaycastData RaycastAtAngle(float angle) {
        _direction = DirectionFromAngle2D(angle, 0f);

        _raycastAtAngleHit2D = Physics2D.Raycast(this.transform.position, _direction, VisionRadius, ObstacleMask);
        if (_raycastAtAngleHit2D) {
            _returnRaycastData.Hit = true;
            _returnRaycastData.Point = _raycastAtAngleHit2D.point;
            _returnRaycastData.Distance = _raycastAtAngleHit2D.distance;
            _returnRaycastData.Angle = angle;
        } else {
            _returnRaycastData.Hit = false;
            _returnRaycastData.Point = this.transform.position + _direction * VisionRadius;
            _returnRaycastData.Distance = VisionRadius;
            _returnRaycastData.Angle = angle;
        }

        return _returnRaycastData;
    }

    private Vector3 DirectionFromAngle2D(float angle, float additionalAngle) {
        angle += additionalAngle;

        Vector3 direction = Vector3.zero;
        direction.x = Mathf.Cos(angle * Mathf.Deg2Rad);
        direction.y = Mathf.Sin(angle * Mathf.Deg2Rad);
        direction.z = 0f;
        return direction;
    }
}