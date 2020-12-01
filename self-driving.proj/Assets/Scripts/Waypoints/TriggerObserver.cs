// SerialID: [d9c2059b-6dd6-4071-8f77-d635fae0e462]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TriggerObserver : MonoBehaviour
{
    [SerializeField] private TransformEvent onEnter = new TransformEvent();
    public TransformEvent OnEnter => onEnter;

    [SerializeField] private Obstacle targetObstacle = null;
    public Obstacle TargetObstacle { get { return targetObstacle; } set { targetObstacle = value; } }

    public void OnTriggerEnter(Collider other) {
        OnEnter.Invoke(other.transform);
        if(TargetObstacle != null) {
            TargetObstacle.SetForce(other.transform);
        }
    }

    [Serializable] public class TransformEvent : UnityEvent<Transform> {
    }
}
