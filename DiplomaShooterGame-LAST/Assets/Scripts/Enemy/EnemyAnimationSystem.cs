using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyAnimationSystem : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Wallk = Animator.StringToHash("Wallk");
        private static readonly int IdleWait = Animator.StringToHash("IdleWait");
        private static readonly int Turning = Animator.StringToHash("Turning");
        private static readonly int ReceiveHit = Animator.StringToHash("ReceiveHit");
        private static readonly int Die = Animator.StringToHash("Die");

        // GET RID OF MONOBEHAVIOUR
        public void StartWallk()
        {
            animator.SetBool(Wallk, true);
        }
        public void StopWallk()
        {
            animator.SetBool(Wallk, false);
        }
        public void StartIdle()
        {
            animator.SetBool(IdleWait, true);
        }
        public void StopIdle()
        {
            animator.SetBool(IdleWait, false);
        }
        public void ChangeTurnAngle(float turn)
        {
            animator.SetFloat(Turning, turn);
        }
        // public void StopTurn()
        // {
        //     animator.SetBool("Turn", false);
        // }
        
        public void StartAttack()
        {
            animator.SetBool(Attack,true);
        }
        public void ResetAttack()
        {
            animator.SetBool(Attack, false);
        }

        public bool GetAttack()
        {
            return animator.GetBool(Attack);
        }

        public void RecieveHit()
        {
            animator.SetBool(ReceiveHit,true);
        }

        public void FinishRecievingHit()
        {
            Debug.Log("FinishRecievingHit");
            animator.SetBool(ReceiveHit,false);
        }
        public bool GetRecievingHit()
        {
            return animator.GetBool(ReceiveHit);
        }

        public void SetDeath()
        {
            animator.SetBool(Die,true);
        }
    }
}