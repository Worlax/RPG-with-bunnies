using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityPanel: MonoBehaviour
{
    // Properties //
    List<Transform> slots = new List<Transform>();

    float defaultTransparente = 0.3f;

    // Functions //
    void Start()
    {
        FillSlots();

        AddSkill(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            slots[0].GetComponent<Ability>()?.Clicked();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            slots[1].GetComponent<Ability>()?.Clicked();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            slots[2].GetComponent<Ability>()?.Clicked();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            slots[3].GetComponent<Ability>()?.Clicked();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            slots[4].GetComponent<Ability>()?.Clicked();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            slots[5].GetComponent<Ability>()?.Clicked();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            slots[6].GetComponent<Ability>()?.Clicked();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            slots[7].GetComponent<Ability>()?.Clicked();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            slots[8].GetComponent<Ability>()?.Clicked();
        }
    }

    // Fill containers //
    void FillSlots()
    {
        foreach (Transform trans in transform)
        {
            slots.Add(trans);
        }
    }

    //....//
    void AddSkill(int index)
    {
        slots[index].gameObject.AddComponent<Ability1>();

        Image image = slots[index].GetComponent<Image>();
        Color color = image.color;

        image.sprite = Ability1.spritee;
        color.a = 1f;
        image.color = color;
    }

    void RemoveSkill(int index)
    {
        Destroy(slots[index].GetComponent<Ability>());

        Image image = slots[index].GetComponent<Image>();
        Color color = image.color;

        image.sprite = null;
        color.a = defaultTransparente;
        image.color = color;
    }
}
