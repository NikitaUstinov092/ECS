using System;
using Unity.Entities;

namespace Game.Scripts.Domain.GameEntities.Core.Stamina
{
   [Serializable]
   public struct MaxStamina : IComponentData
   {
      public float Value;
   }
}
