using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Ability: MonoBehaviour
{
    // Properties //
    public static Sprite spritee;
    protected List<string> tags = new List<string>();
    protected int skillCost = 2;

    // Functions //
    protected virtual void Init(string spritePath)
    {
        spritee = Resources.Load<Sprite>(spritePath);

        GetComponent<Button>().onClick.AddListener(delegate { Clicked(); });
    }

    protected virtual void Awake() { }

    public virtual bool Clicked()
    {
        if (GameManager.instance.currentUnit == null)
            return false;

        return true;
    }

    public virtual bool Cast(PlayerController caster, UnitController target, Vector3 position)
    {
        if (!target || target.GetComponent<Stats>().dead == true || caster.currentActionPoints < skillCost)
            return false;

        caster.WasteActionPoints(skillCost);

        if (target.tag == "Enemy")
        {
            caster.FocusTarget(target.GetComponent<EnemyController>());
        }

        caster.LookAt(target.transform.position);

        return true;
    }
}