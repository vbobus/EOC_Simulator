using System;
using Character.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Events
{
    
    public enum ActionMap
    {
        Player,
        UI,
    }
    
    public class InputManager : MonoBehaviour
    {
        #region Parameters
        
        public static InputManager Instance;
        
        [Header("References")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Actions: Mouse Actions")]
        [SerializeField] private InputActionReference leftClickAction; // 
        [SerializeField] private InputActionReference rightClickAction; // 
        [SerializeField] private InputActionReference pointerDeltaAction; // The amount that the mouse has moved this frame
        [SerializeField] private InputActionReference pointerPositionAction; // The position on the screen

        [Header("Actions: Misc Player Actions")]
        [SerializeField] private InputActionReference move4DirectionAction;
        [SerializeField] private InputActionReference interactionAction; // Properly just "E"?
        [SerializeField] private InputActionReference confirmAction; //

        [SerializeField] private InputActionReference commonExitAction;
        
        
        
        [Header("WebGL Click to play after UI")]        
        [SerializeField] private GameObject clickToPlayCanvas;

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
        
        public UnityAction OnCommonExitActionPressed { get; set; }
        
        [HideInInspector] public ActionMap ActionMap { get; private set; }
        public UnityAction<ActionMap> OnSwitchedActionMap { get; set; }

        
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
         
        }

        private void Start()
        {
            // Temp solution
            // SwitchToPlayerMap();
            SwitchToUIMap();
            // SwitchToPlayerMap();
            
            inputActions.FindActionMap("Common").Enable();
        }

        private void OnEnable()
        {
            // Subscribe to all actions
            leftClickAction.action.performed += OnLeftClickPerformed;
            rightClickAction.action.performed += OnRightClickPerformed;
            interactionAction.action.performed += OnInteractionPerformed;
            confirmAction.action.performed += OnConfirmPerformed;


            commonExitAction.action.performed += OnCommonExitPerformed;
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
        private void OnCommonExitPerformed(InputAction.CallbackContext obj)
        {
            OnCommonExitActionPressed?.Invoke();

            if (ActionMap == ActionMap.Player) SwitchToUIMap();
            else SwitchToPlayerMap();
        }
        
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
            UpdatePlayerChecks();
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


        public void SwitchToUIMapInDialogue()
        {
            SwitchToUIMap();
            _inDialogue = true;
            clickToPlayCanvas.SetActive(false);
        }
        
        public void SwitchToPlayerMapInDialogue()
        {
            _inDialogue = false;
            // SwitchToPlayerMap();
            clickToPlayCanvas.SetActive(true);
        }

        private bool _inDialogue = false;
        
        public void SwitchToUIMap()
        {
            if (_inDialogue) return;
            // Disable the current action map
            inputActions.FindActionMap("Player").Disable();

            // Enable the UI action map
            inputActions.FindActionMap("UI").Enable();
            
            // inputActionRef.
            ActionMap = ActionMap.UI;
            OnSwitchedActionMap?.Invoke(ActionMap);
        }

        
        public void SwitchToPlayerMap()
        {
            if (_inDialogue) return;
         
            InFocusPlayerMap();
            // clickToPlayCanvas.SetActive(true);
        }

        public void InFocusPlayerMap()
        {
            // Disable the UI action map
            inputActions.FindActionMap("UI").Disable();

            // Enable the Player action map
            inputActions.FindActionMap("Player").Enable();
            
            ActionMap = ActionMap.Player;
            OnSwitchedActionMap?.Invoke(ActionMap);
        }
        
        public bool ActionMapIsUI() => ActionMap == ActionMap.UI;
    }

}