using UnityEngine;
public class PuffEffect : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;
    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        if(animator ==null)
        {
            return;
        }
        animator.SetTrigger("Jump");
    }
    public void Destroy()
    {
        Destroy(gameObject,lifeTime);
    }
}
