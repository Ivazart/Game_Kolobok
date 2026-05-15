using System;
using Global;

namespace _Project.Player
{
    using UnityEngine;

    public class Bounce : MonoBehaviour
    {
        [SerializeField] private Collider2D colliderWithMaterial;
        
        [SerializeField] private float rocks;
        [SerializeField] private float swamp;
        
        private PhysicsMaterial2D heroMaterial;

        private SceneController sceneController => SceneController.Instance;
        private void Awake()
        {
            heroMaterial = colliderWithMaterial.sharedMaterial;
            if (heroMaterial != null)
                heroMaterial = Instantiate(heroMaterial);
            colliderWithMaterial.sharedMaterial = heroMaterial;

            SetBounce();
        }

        private void SetBounce()
        {
            SceneName scene = sceneController.CurrentScene;
            float bounce = scene switch
            {
                SceneName.Rocks => rocks,
                SceneName.Swamp => swamp,
                _ => .7f
            };

            bounce = Mathf.Clamp01(bounce);
            if (heroMaterial != null)
                heroMaterial.bounciness = bounce; 
        }
        
    }
}