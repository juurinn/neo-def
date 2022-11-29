using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
/// <summary>
/// Draws vision cone gizmos in editor.
/// </summary>
[CustomEditor(typeof(ConeOfVision2D))]
public class ConeOfVision2DInspector : Editor {
    protected ConeOfVision2D _coneOfVision;

    protected virtual void OnSceneGUI() {
        // Draws a circle around the object to represent the cone of vision's radius
        _coneOfVision = (ConeOfVision2D)target;

        Handles.color = Color.yellow;
        Handles.DrawWireArc(_coneOfVision.transform.position, -Vector3.forward, Vector3.up, 360f, _coneOfVision.VisionRadius);

        // Draws two lines to mark the vision angle
        Vector3 visionAngleLeft = DirectionFromAngle2D(-360 / 2f);
        Vector3 visionAngleRight = DirectionFromAngle2D(360 / 2f);

        Handles.DrawLine(_coneOfVision.transform.position, _coneOfVision.transform.position + visionAngleLeft * _coneOfVision.VisionRadius);
        Handles.DrawLine(_coneOfVision.transform.position, _coneOfVision.transform.position + visionAngleRight * _coneOfVision.VisionRadius);
    }

    private Vector3 DirectionFromAngle2D(float angle) {
        Vector3 direction = Vector3.zero;
        direction.x = Mathf.Cos(angle * Mathf.Deg2Rad);
        direction.y = Mathf.Sin(angle * Mathf.Deg2Rad);
        direction.z = 0f;
        return direction;
    }
}
#endif