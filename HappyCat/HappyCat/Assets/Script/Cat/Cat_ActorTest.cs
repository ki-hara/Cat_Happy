using Cysharp.Threading.Tasks;
using HC.Data;
using HC.Event;
using HC.Resource;
using HC.Utils;
using NUnit.Framework.Constraints;
using UGS;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace HC.Game
{
    public class Cat_ActorTest : MonoBehaviour
    {
        int catCode;
        SpriteRenderer spriteRenderer;
        Animator animator;
        NavMeshAgent agent;
        E_ANIMATION catState = E_ANIMATION.IDLE;
        static float agentDrift = 0.0001f;

        E_DESTINATION destination = E_DESTINATION.NONE;
        public GuestTable.Data CatData { get; set; }
        public FoodData foodData;


        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log(mousePos);
                SetDestination(mousePos);
            }
            if (Input.GetMouseButtonDown(1))
            {
                agent.ResetPath();
            }
        }
        private void OnDestroy()
        {
        }
        void SetDestination(Vector3 target)
        {
            var driftPos = target;
            if (Mathf.Abs(transform.position.x - target.x) < agentDrift)
                driftPos = target + new Vector3(agentDrift, 0f, 0f);
            agent.SetDestination(driftPos);
        }
    }
}
