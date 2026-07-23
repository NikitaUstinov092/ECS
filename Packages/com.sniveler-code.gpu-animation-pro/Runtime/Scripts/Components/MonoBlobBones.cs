using System;
using System.Collections.Generic;
using System.Linq;
using SnivelerCode.GpuAnimation.Runtime.Utils;
using UnityEngine;

namespace SnivelerCode.GpuAnimation.Runtime.Components
{
    [Serializable]
    public class MonoBlobBones
    {
        public List<string> BonesNames;
        public List<MonoBlobBone> BlobBones;

        public MonoBlobBones(Transform[] bones)
        {
            BonesNames = bones.Select(a => a.name).ToList();
            BlobBones = new List<MonoBlobBone>();
        }

        public void Add(int boneIndex, int count, Matrix4x4 matrix)
        {
            var monoBone = BlobBones.FirstOrDefault(bone => bone.Index == boneIndex);
            if (monoBone == null)
            {
                monoBone = new MonoBlobBone
                {
                    Animations = new List<MonoBlobBoneAnimation>(),
                    Index = boneIndex
                };

                BlobBones.Add(monoBone);
            }

            var monoBoneAnimation = monoBone.Animations
                .FirstOrDefault(anim => anim.Index == count);

            if (monoBoneAnimation == null)
            {
                monoBoneAnimation = new MonoBlobBoneAnimation
                {
                    Index = count,
                    Frames = new List<DualQuaternion>()
                };

                monoBone.Animations.Add(monoBoneAnimation);
            }

            monoBoneAnimation.Frames.Add(new DualQuaternion(matrix));
        }
    }

    [Serializable]
    public class MonoBlobBone
    {
        public int Index;
        public List<MonoBlobBoneAnimation> Animations;
    }

    [Serializable]
    public class MonoBlobBoneAnimation
    {
        public int Index;
        public List<DualQuaternion> Frames;
    }
}
