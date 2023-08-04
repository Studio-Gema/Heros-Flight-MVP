using System;
using HeroesFlight.System.Character.Enum;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CharacterSimpleController :MonoBehaviour,CharacterControllerInterface
    {
        [SerializeField] CharacterSO characterSO;
        CharacterAnimationControllerInterface animationController;
        CharacterMovementController m_MovementController;
        CharacterInputReceiver m_InputReceiver;
        CharacterStatController m_CharacterStatController;
        ICharacterViewController viewController;
        Vector3 m_SavedVelocity = default;
        Transform m_Transform;

        public bool IsFacingLeft { get; private set; }
        public Transform CharacterTransform => transform;
        public CharacterSO CharacterSO => characterSO;
        public CharacterStatController CharacterStatController => m_CharacterStatController;
        public event Action<CharacterState> OnCharacterMoveStateChanged;
        CharacterState m_CurrentState;

        public  Vector3 GetVelocity() => m_SavedVelocity;

        bool isEnabled;

        public void Init()
        {
            animationController = GetComponent<CharacterAnimationController>();
            m_MovementController = GetComponent<CharacterMovementController>();
            m_InputReceiver = GetComponent<CharacterInputReceiver>();
            m_CharacterStatController = GetComponent<CharacterStatController>();
            viewController = GetComponent<ICharacterViewController>();
            m_Transform = GetComponent<Transform>();
            m_CharacterStatController.Initialize(CharacterSO.GetPlayerStatData);
            viewController.SetupView(CharacterSO.GetAppearanceData);
            animationController.Init(characterSO.AnimationData);
            m_CurrentState = CharacterState.Idle;
            IsFacingLeft = true;
            isEnabled = false;
        }
     
        void FixedUpdate()
        {
            
            ControllerUpdate();
        }

        public void SetActionState(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        void ControllerUpdate()
        {
            var input = isEnabled? m_InputReceiver.GetInput(): Vector2.zero;
            var velocity = CalculateCharacterVelocity(input);
            m_SavedVelocity = velocity;
            m_MovementController.SetVelocity(velocity);
            UpdateCharacterState(input);
        }

        void UpdateCharacterState(Vector3 input)
        {
            if(!isEnabled)
                return;
            
            var newState = CharacterState.Idle;
            if (input.Equals(Vector3.zero))
            {
                newState = CharacterState.Idle;
            }
            
            if (Mathf.Abs(input.y) < 0.4f)
            {
                switch (input.x)
                {
                    case > 0:
                        newState = CharacterState.FlyingRight;
                        break;
                    case < 0:
                        newState = CharacterState.FlyingLeft;
                        break;
                    case 0 :
                        newState = CharacterState.Idle;
                        break;
                            
                }
            }
            else
            {
                switch (input.y)
                {
                    case > 0.4f:
                        newState = CharacterState.FlyingUp;
                        break;
                    case < -0.4f:
                        newState = CharacterState.FlyingDown;
                        break;
              
                }
            }
           

           

            bool facingLeft;
            if (input.x != 0)
            {
                facingLeft = !(input.x > 0);
            }
            else
            {
                facingLeft = IsFacingLeft;
            }
            

            if (m_CurrentState == newState && IsFacingLeft==facingLeft)
                return;

            IsFacingLeft = facingLeft;
            m_CurrentState = newState;
            OnCharacterMoveStateChanged?.Invoke(m_CurrentState);
            animationController.AnimateCharacterMovement(m_CurrentState,IsFacingLeft);
        }

        Vector3 CalculateCharacterVelocity(Vector3 inputVector)
        {
            var velocity = CalculateMovementDirection(inputVector);
            return velocity * CharacterSO.GetPlayerStatData.MoveSpeed;
        }

        Vector3 CalculateMovementDirection(Vector3 inputVector)
        {
            var velocity = Vector3.zero;
            velocity += m_Transform.right * inputVector.x;
            velocity += m_Transform.up * inputVector.y;
            
            if(velocity.magnitude>1)
                velocity.Normalize();

            return velocity;
        }
    }
}