// SerialID: [d9c2059b-6dd6-4071-8f77-d635fae0e462]
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarAgent : Agent
{
    [SerializeField] private int currentStep = 0;
    private int CurrentStep { get { return currentStep; } set { currentStep = value; } }

    [SerializeField] private int currentStepMax = 5000;
    private int CurrentStepMax => currentStepMax;

    [SerializeField] private int localStep = 0;
    private int LocalStep { get { return localStep; } set { localStep = value; } }

    [SerializeField] private int localStepMax = 200;
    private int LocalStepMax => localStepMax;

    [SerializeField] private bool allowPlusReward = true;
    private bool AllowPlusReward => allowPlusReward;

    [SerializeField] private bool isLearning = true;
    private bool IsLearning => isLearning;

    private Sensor[] Sensors { get; set; }
    private CarController Controller { get; set; }
    private Rigidbody CarRb { get; set; }
    private Vector3 StartPosition { get; set; }
    private Quaternion StartRotation { get; set; }
    private Vector3 LastPosition { get; set; }
    private float TotalDistance { get; set; }
    private int WaypointIndex { get; set; }

    private void Awake() {
        CarRb = GetComponent<Rigidbody>();
        Controller = GetComponent<CarController>();
        Sensors = GetComponentsInChildren<Sensor>();
    }

    public void Start() {
        StartPosition = transform.position;
        StartRotation = transform.rotation;
        CurrentStep = 0;
        LocalStep = 0;
        LastPosition = StartPosition;
        TotalDistance = 0;
    }

    public override void AgentReset() {
        transform.position = StartPosition;
        transform.rotation = StartRotation;

        Controller.GasInput = 0;
        Controller.SteerInput = 0;
        Controller.BrakeInput = 0;

        gameObject.SetActive(false);
        gameObject.SetActive(true);

        CurrentStep = 0;
        LocalStep = 0;
        TotalDistance = 0;
        LastPosition = StartPosition;

        WaypointIndex = 0;
    }

    public override int GetState() {
        var stateDivide = 3;
        var results = new List<double>();
        var r = 0;
        Array.ForEach(Sensors, sensor => {
            results.AddRange(sensor.Hits());
        });
        for(int i = 0; i < results.Count; i++) {
            var v = Mathf.FloorToInt(Mathf.Lerp(0, stateDivide - 1, (float)results[i]));
            if(results[i] >= 0.99f) {
                v = stateDivide - 1;
            }
            r += (int)(v * Mathf.Pow(stateDivide, i));
        }
        var numStates = (int)Mathf.Pow(stateDivide, results.Count);
        int n;
        if(CarRb.velocity.magnitude < 10) { n = 0; }
        else if(CarRb.velocity.magnitude < 15) { n = 1; }
        else { n = 2; }
        r += numStates * n;
        return r;
    }

    public override List<double> CollectObservations() {
        // センサーの距離をリストに追加する
        var results = new List<double>();
        Array.ForEach(Sensors, sensor => {
            results.AddRange(sensor.Hits());
        });
        Vector3 local_v = CarRb.transform.InverseTransformDirection(CarRb.velocity);
        results.Add(local_v.x / 5.0f);
        results.Add(local_v.z / 5.0f);
        return results;
    }

    public override double[] ActionNumberToVectorAction(int ActionNumber) {
        var action = new double[3];
        var steering = 0.0d;
        var braking = 0.0d;
        if(ActionNumber % 6 == 1) {
            steering = 1d;
        }
        else if(ActionNumber % 6 == 2) {
            steering = -1d;
        }
        else if(ActionNumber % 6 == 3) {
            steering = 0.5d;
        }
        else if(ActionNumber % 6 == 4) {
            steering = -0.5d;
        }
        else if(ActionNumber % 6 == 5) {
            braking = 0.5d;
        }

        var gasInput = 0.5d;
        action[0] = steering;
        action[1] = gasInput;
        action[2] = braking;
        return action;
    }

    public override void AgentAction(double[] vectorAction) {
        CurrentStep++;
        LocalStep++;
        TotalDistance += (transform.position - LastPosition).magnitude;

        if(IsLearning) {
            if(CurrentStep > CurrentStepMax) {
                DoneWithReward(TotalDistance);
                return;
            }

            if(LocalStep > LocalStepMax) {
                DoneWithReward(-1.0f / TotalDistance);
                return;
            }
        }

        var steering = Mathf.Clamp((float)vectorAction[0], -1.0f, 1.0f);
        var gasInput = Mathf.Clamp((float)vectorAction[1], 0.5f, 1.0f);
        var braking = Mathf.Clamp((float)vectorAction[2], 0.0f, 0.5f);

        Controller.SteerInput = steering;
        Controller.GasInput = gasInput;
        Controller.BrakeInput = braking;
        LastPosition = transform.position;
    }

    /// <summary>
    /// 衝突時に呼び出されるコールバック
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "wall") {
            DoneWithReward(-1.0f / TotalDistance);
        }
    }

    public void OnTriggerEnter(Collider other) {
        var waypoint = other.GetComponent<Waypoint>();
        if(waypoint == null) {
            return;
        }

        if(WaypointIndex >= waypoint.Index) {
            SetReward(-1.0f / TotalDistance);
            return;
        }
        
        WaypointIndex = waypoint.Index;
        if(waypoint.IsLast) {
            WaypointIndex = 0;
        }
        LocalStep = 0;
    }

    public override void Stop() {
        CarRb.velocity = Vector3.zero;
        CarRb.angularVelocity = Vector3.zero;
        Controller.Stop();
    }

    private void DoneWithReward(float reward) {
        if(reward > 0 && !AllowPlusReward) {
            reward = 0;
        }

        SetReward(reward);
        Done();
    }
}
