using UnityEngine;

namespace Enemy
{
    public class SimpleDiedHandler : DiedHandler
    {
        public override void Die()
        {
            Destroy(gameObject);
        }
    }
}