public class SpellDictionary
{
    public string Spell;
    public SpellDamage Fire;
    public SpellDamage Cold;
    public SpellDamage Poison;

    public override string ToString()
    {
        return $"Spell: {Spell}, FireDamage: {Fire}, ColdDamage: {Cold}, PoisonDamage: {Poison}";
    }
}

public class SpellDamage
{
    public int Damage;
    public int Duration;

    public override string ToString()
    {
        return $"SpellDamage: Damage: {Damage}, Duration: {Duration}";
    }
}