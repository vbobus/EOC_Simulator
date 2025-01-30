using System;
using Character.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Events
{
    public class InputManager : MonoBehaviour
    {
        #region Parameters
        
        public static InputManager Instance;
        
        [Title("References")]
        [SerializeField] private InputActionAsset inputActions;

        [Title("Actions","Mouse Actions")]
        [SerializeField] [Required] private InputActionReference leftClickAction; // 
        [SerializeField] [Required] private InputActionReference rightClickAction; // 
        [SerializeField] [Required] private InputActionReference pointerDeltaAction; // The amount that the mouse has moved this frame
        [SerializeField] [Required] private InputActionReference pointerPositionAction; // The position on the screen

        [Title("Actions","Misc Player Actions")]
        [SerializeField] [Required] private InputActionReference move4DirectionAction;
        [SerializeField] [Required] private InputActionReference interactionAction; // Properly just "E"?
        [SerializeField] [Required] private InputActionReference confirmAction; //
        
        /// <summary>
        /// Can be used for normal WASD movement, or WS for forward and back, and AD for rotation.
        /// <para>Can also use the arrow keys</para>
        /// </summary>
        public UnityAction<Vector2> On4DirectionMoveActionPressed { get; set; }
        public UnityAction<Vector2> OnPointerDelta { get; set; }
        public UnityAction<Vector2> OnPointerPosition { get; set; }
        
        public UnityAction OnLeftClickActionPressed { get; set; }
        public UnityAction OnRightClickActionPressed { get; set; }
        public UnityAction OnInteractActionPressed { get; set; }
        public UnityAction OnConfirmActionPressed { get; set; }
        
        private bool _actionMapPlayer;
        
        #endregion

        #region Start Class

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this); 
            }
            else Destroy(Instance);
            
            SwitchToPlayerMap();
        }

        private void OnEnable()
        {
            // Subscribe to all actions
            leftClickAction.action.performed += OnLeftClickPerformed;
            rightClickAction.action.performed += OnRightClickPerformed;
            interactionAction.action.performed += OnInteractionPerformed;
            confirmAction.action.performed += OnConfirmPerformed;
        }

        private void OnDisable()
        {
            // Unsubscribe from all actions
            leftClickAction.action.performed -= OnLeftClickPerformed;
            rightClickAction.action.performed -= OnRightClickPerformed;
            interactionAction.action.performed -= OnInteractionPerformed;
            confirmAction.action.performed -= OnConfirmPerformed;
        }       
        #endregion

        #region Performed Methods
        private void OnLeftClickPerformed(InputAction.CallbackContext context)
        {
            OnLeftClickActionPressed?.Invoke();
        }

        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            OnRightClickActionPressed?.Invoke();
        }

        private void OnInteractionPerformed(InputAction.CallbackContext context)
        {
            OnInteractActionPressed?.Invoke();
        }

        private void OnConfirmPerformed(InputAction.CallbackContext context)
        {
            OnConfirmActionPressed?.Invoke();
        }
        #endregion
        
        private void Update()
        {
            if (_actionMapPlayer)
                UpdatePlayerChecks();
            else
                UpdateUIChecks();
        }
        private void UpdateUIChecks()
        {
        }

        #region Player ActionMap Continuos Check Methods

        private void UpdatePlayerChecks()
        {
            // Move position every frame
            Vector2 directionMovement = move4DirectionAction.action.ReadValue<Vector2>();
            On4DirectionMoveActionPressed?.Invoke(directionMovement);
            
            // Poll the Delta value every frame
            Vector2 delta = pointerDeltaAction.action.ReadValue<Vector2>();
            OnPointerDelta?.Invoke(delta);
            
            // Poll the position value every frame
            Vector2 lookPosition = pointerPositionAction.action.ReadValue<Vector2>();
            OnPointerPosition?.Invoke(lookPosition);
        }


         
        #endregion

        public void SwitchToUIMap()
        {
            // Disable the current action map
            inputActions.FindActionMap("Player").Disable();

            // Enable the UI action map
            inputActions.FindActionMap("UI").Enable();
            
            _actionMapPlayer = false;
        }

        public void SwitchToPlayerMap()
        {
            // Disable the UI action map
            inputActions.FindActionMap("UI").Disable();

            // Enable the Player action map
            inputActions.FindActionMap("Player").Enable();
            
            _actionMapPlayer = true;
        }
    }
}