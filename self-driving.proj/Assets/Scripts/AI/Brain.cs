// SerialID: [d9c2059b-6dd6-4071-8f77-d635fae0e462]
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class Brain
{
    public float Reward { get; set; }

    public abstract void Save(string path);
}
