using System;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.DemoZone2
{
    public sealed class DemoArmorySkyboxMovement: MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * 0.1f);
        }
    }
}
