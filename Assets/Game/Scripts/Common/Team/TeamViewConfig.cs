using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Common.Team
{
    [CreateAssetMenu(
        fileName = "TeamViewConfig",
        menuName = "SampleGame/Common/New TeamViewConfig"
    )]
    public sealed class TeamViewConfig : ScriptableObject
    {
        [SerializeField]
        private TeamInfo[] _teams;

        public TeamInfo GetTeam(TeamType teamType)
        {
            for (int i = 0, count = _teams.Length; i < count; i++)
            {
                TeamInfo info = _teams[i];
                if (info.Type == teamType)
                    return info;
            }

            throw new KeyNotFoundException($"Team of type {teamType} is not found!");
        }

        [Serializable]
        public sealed class TeamInfo
        {
            [SerializeField]
            private TeamType type;

            [SerializeField]
            private Material material;

            public Material Material
            {
                get { return this.material; }
            }

            public TeamType Type
            {
                get { return type; }
            }
        }
    }
}