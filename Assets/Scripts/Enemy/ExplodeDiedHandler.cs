using System;
using System.Collections;
using Interfaces;
using Pathfinding;
using UnityEngine;

namespace Enemy
{
    public class ExplodeDiedHandler : DiedHandler
    {
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private int damage = 10;
        [SerializeField] private float explodeRange = 1f;
        [SerializeField] private LayerMask targetLayerMask;
        [SerializeField] private float timeToExplode = 2f;

        private Animator _animator;
        private static readonly int AnimDieParam = Animator.StringToHash("Die");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public override void Die()
        {
            _animator.SetBool(AnimDieParam, true);
            GetComponent<AIPath>().enabled = false;
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            yield return new WaitForSeconds(timeToExplode);
            var position = transform.position;
            GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            Destroy(explosion, 1f);
            var targetResults = new Collider2D[2];
            var targetCount =
                Physics2D.OverlapCircleNonAlloc(position, explodeRange, targetResults, targetLayerMask);
            for (var i = 0; i < targetCount; i++)
            {
                var targetCollider2D = targetResults[i];
                targetCollider2D.GetComponent<IDamageable>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explodeRange);
        }
    }
}