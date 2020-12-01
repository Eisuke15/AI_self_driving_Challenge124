// SerialID: [d9c2059b-6dd6-4071-8f77-d635fae0e462]
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    public void OnTimeScaleChanged(float timeScale) {
        Time.timeScale = timeScale;
    }
}
