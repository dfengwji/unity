using UnityEngine;

namespace ZStart.VRoom.Controller
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class MoveController: MonoBehaviour
    {
        public enum Way
        {
            None = 0,
            AI = 1,
            Input = 2,
        }

        public Transform header;
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public Transform target;
        public float WalkSpeed = 1.5f;   // Speed when walking forward
        public float RunMultiplier = 2.0f;   // Speed when sprinting
        public float JumpForce = 30f;
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;
        [Tooltip("set it to 0.1 or more if you get stuck in wall")]
        public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float stickToGroundHelperDistance = 0.5f; // stops the character
        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded, m_Running;
        private float Speed = 0f;
        private float steps = 0f;
        Vector3 m_GroundNormal;
        float m_ForwardAmount;
        public Way moveWay = Way.None;
        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
                return m_Running;
            }
        }

        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            Speed = WalkSpeed;
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            agent.updateRotation = false;
            agent.updatePosition = true;
        }

        public void SetTarget(Transform aim)
        {
            if (moveWay == Way.Input)
                return;
            this.target = aim;
            if (aim == null)
            {
                SwitchWay(Way.None);
            }
            else
            {
                SwitchWay(Way.AI);
            }
        }

        private void Update()
        {
            if (moveWay == Way.Input)
                RotateView();
            if (moveWay == Way.AI)
            {
               
                agent.SetDestination(target.position);
            }
            if (moveWay == Way.AI)
            {
                if (agent.remainingDistance > agent.stoppingDistance)
                    Move(agent.desiredVelocity);
                else
                {
                    Move(Vector3.zero);
                }
            }

            //if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            //{
            //    m_Jump = true;
            //}
        }

        private void FixedUpdate()
        {
            GroundCheck();
            if (moveWay == Way.AI)
                return;
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && m_IsGrounded)
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = header.forward * input.y + header.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
                desiredMove.x = desiredMove.x * Speed;
                desiredMove.z = desiredMove.z * Speed;
                desiredMove.y = desiredMove.y * Speed;
                if (m_RigidBody.velocity.sqrMagnitude < (Speed * Speed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
        }

        private void Move(Vector3 move)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            m_ForwardAmount = move.z;

            // control and velocity handling is different when grounded and airborne:
            HandleAirborneMovement();
        }

        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_RigidBody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_RigidBody.velocity.y < 0 ? m_GroundCheckDistance : 0.01f;
        }

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return SlopeCurveModifier.Evaluate(angle);
        }

        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) +
                                   stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
            Vector2 input = Vector2.zero;
#if SVR
            if (GvrControllerInput.ClickButtonDown || Input.GetMouseButton(0))
            {
                steps += 0.01f;
                if (steps > 1f)
                    steps = 1f;
                input = new Vector2(0f, steps);
                SwitchWay(Way.Input);
            }
            else
            {
                steps = 0f;
                SwitchWay(Way.None);
            }
#else
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
#endif

            return input;
        }

        private void SwitchWay(Way aim)
        {
            if(moveWay == aim)
            {
                return;
            }
            steps = 0;
            if(moveWay == Way.AI)
            {
                agent.isStopped = true;
                target = null;
            }
            else if(moveWay == Way.Input)
            {

            }
            moveWay = aim;
            if (aim == Way.AI)
            {
                agent.isStopped = false;
            }
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;
            if (m_IsGrounded)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
