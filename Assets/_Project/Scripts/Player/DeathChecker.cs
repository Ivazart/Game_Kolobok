using System;
using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

public class DeathChecker : MonoBehaviour
{
    private GameController gameController => GameController.Instance;

    private enum DeathTags
    {
        None,
        poison,
        fire,
        swamp,
        Acid_source,
        Infection_source,
        Splash_source
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SendDeathType(collision.gameObject.tag);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SendDeathType(collision.gameObject.tag);
    }

    private void SendDeathType(string Tag)
    {
        var tag = DeathTags.None;
        foreach (DeathTags value in Enum.GetValues(typeof(DeathTags)))
        {
            if (Tag.Equals(value.ToString()))
                tag = value;
        }

        DeathType type = tag switch
        {
            DeathTags.None => DeathType.None,
            DeathTags.poison => DeathType.Poison,
            DeathTags.fire => DeathType.Fire,
            DeathTags.swamp => DeathType.Swamp,
            DeathTags.Acid_source => DeathType.Acid,
            DeathTags.Infection_source => DeathType.Infection,
            DeathTags.Splash_source => DeathType.Splash,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (type != DeathType.None)
            gameController.PlayerDeath(type);
    }
}