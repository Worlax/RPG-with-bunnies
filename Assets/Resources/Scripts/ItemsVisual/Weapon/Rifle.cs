using UnityEngine;
using System;

public class Rifle: Gun
{
    // Properties //


    // Functions //
    public override int GetMaxAmmo()
    {
        return Rifle.magazineMaxAmmo;
    }
}
