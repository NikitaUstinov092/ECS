using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SampleGame
{
    
    [CreateAssetMenu(
        fileName = "TeamViewCatalog",
        menuName = "SampleGame/View/New TeamViewCatalog"
    )]
    public sealed class TeamViewCatalog : ScriptableObject
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
            [FormerlySerializedAs("_team")]
            [SerializeField]
            private TeamType type;

            [SerializeField]
            private Material material;

            public Material Material => this.material;

            public TeamType Type => type;
        }
    }
}