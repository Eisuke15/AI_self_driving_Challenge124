// SerialID: [d9c2059b-6dd6-4071-8f77-d635fae0e462]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private List<Obstacle> Obstacles { get; } = new List<Obstacle>();

    private void Start() {
        Obstacles.AddRange(FindObjectsOfType<Obstacle>());
    }

    public void SetRandom() {
        Obstacles.ForEach(o => o.SetRandomPosition());
    }

    public void ResetPosition() {
        Obstacles.ForEach(o => o.ResetPosition());
    }
}
