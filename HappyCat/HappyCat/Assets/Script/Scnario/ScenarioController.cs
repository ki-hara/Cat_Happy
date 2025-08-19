using UnityEngine;

public class ScenarioController : MonoBehaviour
{
    [SerializeField]    Animator animator;
    [SerializeField]    HCButton button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(Next);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Next()
    {
        animator.SetTrigger("Next");
    }
}
