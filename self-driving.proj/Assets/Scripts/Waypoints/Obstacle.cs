// SerialID: [d9c2059b-6dd6-4071-8f77-d635fae0e462]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Vector3 left = Vector3.zero;
    private Vector3 Left { get { return left; } set { left = value; } }

    [SerializeField] private Vector3 right = Vector3.zero;
    private Vector3 Right { get { return right; } set { right = value; } }

    [SerializeField] private string layerRoad = "Road";
    private string LayerRoad => layerRoad;

    [SerializeField] private float minForce = 100.0f;
    public float MinForce { get { return minForce; } set { minForce = value; } }

    [SerializeField] private float maxForce = 300.0f;
    public float MaxForce { get { return maxForce; } set { maxForce = value; } }

    [SerializeField] private float minScale = 0.5f;
    public float MinScale { get { return minScale; } set { minScale = value; } }

    [SerializeField] private float maxScale = 1.5f;
    public float MaxScale { get { return maxScale; } set { maxScale = value; } }

    private Vector3 StartPosition { get; set; }
    private Quaternion StartRotation { get; set; }

    private void Start() {
        StartPosition = transform.position;
        StartRotation = transform.rotation;
        var rg = GetComponentInChildren<Rigidbody>();
        if(rg != null) {
            rg.isKinematic = true;
        }
    }
    public void SetSide(Vector3 left, Vector3 right) {
        Left = left;
        Right = right;

        SetRandomPosition();
    }

    public void SetRandomPosition() {
        if(Left == Vector3.zero || Right == Vector3.zero) {
            return;
        }
        transform.position = Vector3.Lerp(Left, Right, UnityEngine.Random.Range(0.2f, 0.8f));

        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, float.MaxValue, LayerMask.GetMask(LayerRoad))) {
            transform.position = hit.point;
            var rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            transform.rotation = rotation * transform.rotation;
        }
    }

    public void SetBy(float minScale, float maxScale, float minForce, float maxForce) {
        MinScale = minScale;
        MaxScale = maxScale;
        MinForce = minForce;
        MaxForce = maxForce;
        ApplyScale();
    }

    public void ResetPosition() {
        transform.position = StartPosition;
        transform.rotation = StartRotation;
        ApplyScale();
        var rg = GetComponentInChildren<Rigidbody>();
        if(rg != null) {
            rg.velocity = Vector3.zero;
            rg.angularVelocity = Vector3.zero;
            rg.isKinematic = true;
        }
    }

    public void SetForce(Transform target) {
        var v = (target.transform.position - transform.position).normalized;
        var rg = GetComponent<Rigidbody>();
        if(rg != null) {
            rg.isKinematic = false;
            rg.AddForce(v * UnityEngine.Random.Range(MinForce, MaxForce), ForceMode.Impulse);
        }
    }

    private void ApplyScale() {
        var scale = UnityEngine.Random.Range(MinScale, MaxScale);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
