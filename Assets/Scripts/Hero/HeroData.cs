using System.Collections.Generic;

namespace Hero
{
    public class HeroData
    {
        public int Health = 0;

        public int Armor = 0;

        public int Strength = 0;

        public int Agility = 0;

        public int Intelligence = 0;

        public int Stamina = 0;

        public int Wisdom = 0;

        public int ActionPoints = 0;

        public List<SpellDictionary> Spells;

        public override string ToString()
        {
            return $"Health: {Health}, Armor: {Armor}, Agility: {Agility}, " +
                   $"Intelligence: {Intelligence},  Stamina: {Stamina}, Wisdom: {Wisdom},  " +
                   $"ActionPoints: {ActionPoints}";
        }
    }
}