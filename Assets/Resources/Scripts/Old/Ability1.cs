using UnityEngine;
using UnityEngine.UI;

public class Ability1: Ability
{
    // Properties //


    // Functions //
    protected override void Awake()
    {
        base.Awake();
        Init("Abilitys/Icons/Ability_02");
    }

    // Select mode //
    public override bool Clicked()
    {
        if (base.Clicked() == false)
            return false;

        tags.Add("Enemy");
        AbilityManager.instance.SetSkillInSelectingMode(this, tags);

        return true;
    }

    //....//
    public override bool Cast(PlayerController caster, UnitController target, Vector3 position)
    {
        if (base.Cast(caster, target, position) == false)
            return false;

        DoHitEffect(position);
        target.GetComponent<Stats>().DealDamage(5, transform);

        return true;
    }

    void DoHitEffect(Vector3 position)
    {
        ParticleSystem explosion = Instantiate(EffectManager.instance.explosion);
        explosion.transform.position = position;

        explosion.Emit(1);
    }
}