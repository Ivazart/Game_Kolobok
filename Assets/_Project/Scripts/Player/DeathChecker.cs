using System;
using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

[RequireComponent(typeof(CollisionLogic))]
public class DeathChecker : MonoBehaviour
{
    private GameController gameController => GameController.Instance;
    private CollisionLogic collision;

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
    
    private void Awake()
    {
        collision = GetComponent<CollisionLogic>();
        collision.OnEnterPlayerSolidEnemySolid += OnCollision;
        collision.OnEnterPlayerSolidEnemyTrigger += OnCollision2;
    }

    private void OnCollision(CollisionEventData data)
    {
        Debug.Log($"Death from solid {data.OtherCollider.gameObject.name}");
        string enemyTag = data.OtherCollider.tag;
        SendDeathType(enemyTag);
    }
    
    private void OnCollision2(CollisionEventData data)
    {
        Debug.Log($"Death from trigger {data.OtherCollider.gameObject.name}");
        string enemyTag = data.OtherCollider.tag;
        SendDeathType(enemyTag);
    }
    
    private void OnParticleCollision(GameObject other)
    {
        SendDeathType(other.gameObject.tag);
    }

    private void SendDeathType(string enemyTag)
    {
        var deathTags = DeathTags.None;
        foreach (DeathTags value in Enum.GetValues(typeof(DeathTags)))
        {
            if (enemyTag.Equals(value.ToString()))
                deathTags = value;
        }

        DeathType type = deathTags switch
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
        {
            Debug.Log($"Death from tag: {enemyTag}");
            gameController.PlayerDeath(type);
        }
    }
}