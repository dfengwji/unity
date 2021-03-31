using UnityEngine;
using UnityEngine.AI;

namespace ZStart.VRoom
{
    public class NavAITest:MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;

        public Transform ball;
        // Use this for initialization
        private void Awake()
        {
            if(navMeshAgent == null)
                navMeshAgent = GetComponent<NavMeshAgent>();//获取navmeshagent
        }
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {
            navMeshAgent.SetDestination(ball.position);//设置导航的目标点
        }
    }
}
