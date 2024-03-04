using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController: MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void EnemyMove(bool anmationRun)
    {
        animator.SetBool("IsWalk", anmationRun);
    }

    public void EnemyAttack(bool anmationRun)
    {
        animator.SetBool("IsAttack", anmationRun);
    }
}
