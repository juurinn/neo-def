using UnityEngine;
using TMPro;

public class DamageCounter : MonoBehaviour {

    private Turret _Turret;
    private TMP_Text _Text;
    private LineRenderer _LineRenderer;

    private void Start() {
        _Turret = GetComponentInParent<Turret>();
        _Text = GetComponent<TMP_Text>();
        _LineRenderer = GetComponent<LineRenderer>();

        _LineRenderer.SetPosition(0, transform.position);
        _LineRenderer.SetPosition(1, transform.position);
    }


    private void Update() {
        if (_Turret == null) return;
        _Text.text = _Turret.totalDmg.ToString();

        if (_Turret.currentTarget != null)
            _LineRenderer.SetPosition(1, _Turret.currentTarget.position);
    }
}
