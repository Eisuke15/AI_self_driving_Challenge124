// SerialID: [d9c2059b-6dd6-4071-8f77-d635fae0e462]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GraphicsToggle : MonoBehaviour
{
    private List<MeshRenderer> Renderer { get; set; } = new List<MeshRenderer>();

    public void OnToggle(bool b) {
        if(b) {
            Renderer.ForEach(r => r.enabled = true);
        }
        else {
            Renderer.Clear();
            Renderer.AddRange(FindObjectsOfType<MeshRenderer>());
            Renderer.ForEach(r => r.enabled = false);
        }
    }
}
