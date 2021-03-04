using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    public void StopAttack()
    {
        GetComponent<Animator>().SetBool("Attack", false);
    }
}
