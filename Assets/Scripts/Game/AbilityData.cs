using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game
{
    //Name of the ability and level are fetched from the name of the ability
    //So the proper naming would be: (string)ability name + "Level" + (int)level number
    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
    public class AbilityData : ScriptableObject
    {
        public AbilityData lastLevelAbility;
        public Sprite icon;
        public string abilityName;
        public string abilityDescription;
        public float cooldown;
        public float cooldownRemaining;
        public int range;
        public float speed;
        public float damage;
        public GameObject abilityPrefab;
        public bool hasProjectile;
        public float scale;
        public bool updatePosition;
        private Abilities ability;
        private int level;

        public Abilities Ability => ability;
        public int Level => level;
        public string Description { get; private set; }

        private void OnValidate()
        {
            int levelIndex = name.IndexOf("Level", StringComparison.Ordinal);
            string abilityNameNoSpace = name[..levelIndex];
            abilityName = string.Join(" ", Regex.Split(abilityNameNoSpace, "(?=[A-Z])")).TrimStart();
            Enum.TryParse(abilityNameNoSpace, out ability);
            int.TryParse(name.Substring(levelIndex + 5), out level);

            SetDescription();
        }

        private void SetDescription()
        {
            string descriptionText = string.Empty;

            if (!lastLevelAbility)
            {
                descriptionText = $"Level: {level} \n" +
                                  $"{abilityDescription} \n" +
                                  $"Cooldown: {cooldown} \n" +
                                  $"Range: {range} \n" +
                                  $"Speed: {speed} \n" +
                                  $"Damage: {damage} \n";
            }
            else
            {
                bool isLevelHigher = level > lastLevelAbility.level;
                bool isCooldownLower = cooldown < lastLevelAbility.cooldown;
                bool isRangeHigher = range > lastLevelAbility.range;
                bool isSpeedHigher = speed > lastLevelAbility.speed;
                bool isDamageHigher = damage > lastLevelAbility.damage;

                string levelText = !isLevelHigher
                    ? $"Level: {level} \n"
                    : $"Level: <color=red>{lastLevelAbility.level}</color> > <color=green>{level}</color> \n";

                string cooldownText = !isCooldownLower
                    ? $"Cooldown: {cooldown} \n"
                    : $"Cooldown: <color=red>{lastLevelAbility.cooldown}</color> > <color=green>{cooldown}</color> \n";

                string rangeText = !isRangeHigher
                    ? $"Range: {range} \n"
                    : $"Range: <color=red>{lastLevelAbility.range}</color> > <color=green>{range}</color> \n";

                string speedText = !isSpeedHigher
                    ? $"Speed: {speed} \n"
                    : $"Speed: <color=red>{lastLevelAbility.speed}</color> > <color=green>{speed}</color> \n";

                string damageText = !isDamageHigher
                    ? $"Damage: {damage} \n"
                    : $"Damage: <color=red>{lastLevelAbility.damage}</color> > <color=green>{damage}</color> \n";

                descriptionText = levelText +
                                  $"{abilityDescription} \n" +
                                  cooldownText +
                                  rangeText +
                                  speedText +
                                  damageText;
            }

            Description = descriptionText;
        }
    }
}