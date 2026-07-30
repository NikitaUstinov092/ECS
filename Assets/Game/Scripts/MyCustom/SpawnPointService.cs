using UnityEngine;

namespace Game.Scripts.MyCustom
{
    public sealed class SpawnPointService : MonoBehaviour
    {
        public static SpawnPointService Instance { get; private set; }
        
        [SerializeField] 
        private Transform[] _spawnPointsBlueTeam;
        
        [SerializeField] 
        private Transform[] _spawnPointsRedTeam;
        

        private void Awake()
        {
            Instance = this;
        }
        
        public Transform GetBlueRandomSpawnPoint()
        {
            return _spawnPointsBlueTeam[Random.Range(0, _spawnPointsBlueTeam.Length)];
        }
        
        public Transform GetRedRandomSpawnPoint()
        {
            return _spawnPointsRedTeam[Random.Range(0, _spawnPointsRedTeam.Length)];
        }
    }
}