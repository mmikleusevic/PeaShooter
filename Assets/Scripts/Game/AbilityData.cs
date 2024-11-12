using System;
using UnityEngine;

namespace Game
{
    //Name of the ability and level are fetched from the name of the ability
    //So the proper naming would be: (string)ability name + "Level" + (int)level number
    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
    public class AbilityData : ScriptableObject
    {
        public Sprite icon;
        public string abilityName;
        public string abilityDescription;
        public float cooldown;
        public float cooldownRemaining;
        public int range;
        public float rotationSpeed;
        public float speed;
        public float damage;
        public GameObject projectilePrefab;
        public float projectileScale;
        private Abilities ability;
        private int level;

        public Abilities Ability => ability;
        public int Level => level;
        public bool HasProjectile { get; private set; }

        private void OnValidate()
        {
            int levelIndex = name.IndexOf("Level", StringComparison.Ordinal);
            string abilityName = name.Substring(0, levelIndex);

            Enum.TryParse(abilityName, out ability);

            int.TryParse(name.Substring(levelIndex + 5), out level);

            HasProjectile = projectilePrefab;
        }
    }
}