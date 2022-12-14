//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.2
//     from Assets/Inputs/GameControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GameControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameControls"",
    ""maps"": [
        {
            ""name"": ""PlayerGameplay"",
            ""id"": ""30ea3d81-5b23-4ec2-96a5-76eac299db6f"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""8f07292b-5ac2-44ca-9df7-341212313021"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""2fc8805c-0641-4fb0-80db-46e9b64f2fe9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""fcdab209-9e2b-49e9-9561-1f94bcfa9b5e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""f3cc9741-fe76-44ce-bd41-15e27d28781b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d264e853-c956-498c-8206-1e034cd22428"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ControllerAim"",
                    ""type"": ""PassThrough"",
                    ""id"": ""211e21a2-66d3-4584-88db-d8f76e5e5827"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""8d30e369-0684-46a2-befa-d921de3e592c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SlowTime"",
                    ""type"": ""Button"",
                    ""id"": ""19362b4a-e82d-4d48-a928-5af9dc275244"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Heal"",
                    ""type"": ""Button"",
                    ""id"": ""7c9564a8-0417-45cf-9266-ff126ab532a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Reset"",
                    ""type"": ""Button"",
                    ""id"": ""f702e2c2-4cca-446a-93f7-7ed8894ba32d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""3fa2a5c8-a241-433c-8731-738b7b09d9ce"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Kill"",
                    ""type"": ""Button"",
                    ""id"": ""605f4127-32d8-486b-807d-c58ed7c027f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ClearEncounter"",
                    ""type"": ""Button"",
                    ""id"": ""91e97500-8876-420b-9d33-fb4ae9b66c7b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""14cffd41-ac2c-4758-9071-3b8034e361b4"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SlowTime"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a02d7d3c-3d5b-456b-9555-413a745bcd85"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SlowTime"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Movement"",
                    ""id"": ""197baad8-9771-4bf9-8737-ba00f6aa4b12"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c32d16f2-c521-4c24-b822-af68e880a080"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3fdcdd52-23e5-4de9-a457-0e87c06290fd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6d18db27-b63e-451e-b91f-0fc27635ef51"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f790d006-3935-4b78-a3e0-3af3ddfc7ce9"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""abb30e25-e533-48a6-b421-1ad3fee97d79"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c8267198-6c7e-4aeb-9a10-38b503fae22e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f9a141b-e1bf-4950-ace4-fbed812fab96"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1fe5a9ec-d002-4444-afc1-1cc79b8fe92b"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Heal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""29cb17eb-a978-4fb0-85aa-783fc10de5d8"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b28d9ac3-3c80-4f8a-8355-8db0942d75e7"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5b197fac-1f8a-42e4-af93-bd673abdf80f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""16441322-ba76-4479-a33d-543fa2bcb9db"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e750d19b-88ad-4ed4-81cf-8a65032af88a"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d0892d9e-033f-40fc-aa9a-49a13c7c9f34"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b40069eb-36b1-4599-895d-95c969a73a34"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""01400614-7007-467d-afd0-7c42b21beeb3"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ffd8484c-89d4-466a-a067-bf8396cb5608"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControllerAim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c92ff0bf-0138-4eb3-b767-6744b09a6f0e"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a4e58ae0-480c-4d88-9db5-b7c79379fe32"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""e12e6fa9-b96e-4f7f-8a46-e154b59ba327"",
            ""actions"": [
                {
                    ""name"": ""Mouse"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c5acd629-26f9-4fd4-bc18-d27d50aa7d78"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Controller"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c306cda9-70ad-4e0e-9c30-86f5b7163051"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""ad8ff928-f829-4dec-9352-e241113e53eb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b93f5733-e1a6-4a31-a24d-4657aa6a5d62"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Controller"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5fe8ac79-4173-410b-9cbe-c0d21260f4cd"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Controller"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3a55982f-b851-4710-b192-6f1d2618cc55"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Controller"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""11f1d3ed-8f91-4425-ac42-7a28f84f9a6c"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d3ec7e2-ffbd-4bf4-8c95-4617e9032605"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c169a124-b65f-447b-8789-ebdd0dc0f05b"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerGameplay
        m_PlayerGameplay = asset.FindActionMap("PlayerGameplay", throwIfNotFound: true);
        m_PlayerGameplay_Pause = m_PlayerGameplay.FindAction("Pause", throwIfNotFound: true);
        m_PlayerGameplay_Move = m_PlayerGameplay.FindAction("Move", throwIfNotFound: true);
        m_PlayerGameplay_Jump = m_PlayerGameplay.FindAction("Jump", throwIfNotFound: true);
        m_PlayerGameplay_Sprint = m_PlayerGameplay.FindAction("Sprint", throwIfNotFound: true);
        m_PlayerGameplay_Aim = m_PlayerGameplay.FindAction("Aim", throwIfNotFound: true);
        m_PlayerGameplay_ControllerAim = m_PlayerGameplay.FindAction("ControllerAim", throwIfNotFound: true);
        m_PlayerGameplay_Shoot = m_PlayerGameplay.FindAction("Shoot", throwIfNotFound: true);
        m_PlayerGameplay_SlowTime = m_PlayerGameplay.FindAction("SlowTime", throwIfNotFound: true);
        m_PlayerGameplay_Heal = m_PlayerGameplay.FindAction("Heal", throwIfNotFound: true);
        m_PlayerGameplay_Reset = m_PlayerGameplay.FindAction("Reset", throwIfNotFound: true);
        m_PlayerGameplay_Interact = m_PlayerGameplay.FindAction("Interact", throwIfNotFound: true);
        m_PlayerGameplay_Kill = m_PlayerGameplay.FindAction("Kill", throwIfNotFound: true);
        m_PlayerGameplay_ClearEncounter = m_PlayerGameplay.FindAction("ClearEncounter", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Mouse = m_UI.FindAction("Mouse", throwIfNotFound: true);
        m_UI_Controller = m_UI.FindAction("Controller", throwIfNotFound: true);
        m_UI_Back = m_UI.FindAction("Back", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerGameplay
    private readonly InputActionMap m_PlayerGameplay;
    private IPlayerGameplayActions m_PlayerGameplayActionsCallbackInterface;
    private readonly InputAction m_PlayerGameplay_Pause;
    private readonly InputAction m_PlayerGameplay_Move;
    private readonly InputAction m_PlayerGameplay_Jump;
    private readonly InputAction m_PlayerGameplay_Sprint;
    private readonly InputAction m_PlayerGameplay_Aim;
    private readonly InputAction m_PlayerGameplay_ControllerAim;
    private readonly InputAction m_PlayerGameplay_Shoot;
    private readonly InputAction m_PlayerGameplay_SlowTime;
    private readonly InputAction m_PlayerGameplay_Heal;
    private readonly InputAction m_PlayerGameplay_Reset;
    private readonly InputAction m_PlayerGameplay_Interact;
    private readonly InputAction m_PlayerGameplay_Kill;
    private readonly InputAction m_PlayerGameplay_ClearEncounter;
    public struct PlayerGameplayActions
    {
        private @GameControls m_Wrapper;
        public PlayerGameplayActions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_PlayerGameplay_Pause;
        public InputAction @Move => m_Wrapper.m_PlayerGameplay_Move;
        public InputAction @Jump => m_Wrapper.m_PlayerGameplay_Jump;
        public InputAction @Sprint => m_Wrapper.m_PlayerGameplay_Sprint;
        public InputAction @Aim => m_Wrapper.m_PlayerGameplay_Aim;
        public InputAction @ControllerAim => m_Wrapper.m_PlayerGameplay_ControllerAim;
        public InputAction @Shoot => m_Wrapper.m_PlayerGameplay_Shoot;
        public InputAction @SlowTime => m_Wrapper.m_PlayerGameplay_SlowTime;
        public InputAction @Heal => m_Wrapper.m_PlayerGameplay_Heal;
        public InputAction @Reset => m_Wrapper.m_PlayerGameplay_Reset;
        public InputAction @Interact => m_Wrapper.m_PlayerGameplay_Interact;
        public InputAction @Kill => m_Wrapper.m_PlayerGameplay_Kill;
        public InputAction @ClearEncounter => m_Wrapper.m_PlayerGameplay_ClearEncounter;
        public InputActionMap Get() { return m_Wrapper.m_PlayerGameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerGameplayActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerGameplayActions instance)
        {
            if (m_Wrapper.m_PlayerGameplayActionsCallbackInterface != null)
            {
                @Pause.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnPause;
                @Move.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnJump;
                @Sprint.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnSprint;
                @Aim.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnAim;
                @ControllerAim.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnControllerAim;
                @ControllerAim.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnControllerAim;
                @ControllerAim.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnControllerAim;
                @Shoot.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnShoot;
                @SlowTime.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnSlowTime;
                @SlowTime.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnSlowTime;
                @SlowTime.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnSlowTime;
                @Heal.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnHeal;
                @Heal.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnHeal;
                @Heal.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnHeal;
                @Reset.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnReset;
                @Reset.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnReset;
                @Reset.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnReset;
                @Interact.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnInteract;
                @Kill.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnKill;
                @Kill.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnKill;
                @Kill.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnKill;
                @ClearEncounter.started -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnClearEncounter;
                @ClearEncounter.performed -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnClearEncounter;
                @ClearEncounter.canceled -= m_Wrapper.m_PlayerGameplayActionsCallbackInterface.OnClearEncounter;
            }
            m_Wrapper.m_PlayerGameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @ControllerAim.started += instance.OnControllerAim;
                @ControllerAim.performed += instance.OnControllerAim;
                @ControllerAim.canceled += instance.OnControllerAim;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @SlowTime.started += instance.OnSlowTime;
                @SlowTime.performed += instance.OnSlowTime;
                @SlowTime.canceled += instance.OnSlowTime;
                @Heal.started += instance.OnHeal;
                @Heal.performed += instance.OnHeal;
                @Heal.canceled += instance.OnHeal;
                @Reset.started += instance.OnReset;
                @Reset.performed += instance.OnReset;
                @Reset.canceled += instance.OnReset;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Kill.started += instance.OnKill;
                @Kill.performed += instance.OnKill;
                @Kill.canceled += instance.OnKill;
                @ClearEncounter.started += instance.OnClearEncounter;
                @ClearEncounter.performed += instance.OnClearEncounter;
                @ClearEncounter.canceled += instance.OnClearEncounter;
            }
        }
    }
    public PlayerGameplayActions @PlayerGameplay => new PlayerGameplayActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_Mouse;
    private readonly InputAction m_UI_Controller;
    private readonly InputAction m_UI_Back;
    public struct UIActions
    {
        private @GameControls m_Wrapper;
        public UIActions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Mouse => m_Wrapper.m_UI_Mouse;
        public InputAction @Controller => m_Wrapper.m_UI_Controller;
        public InputAction @Back => m_Wrapper.m_UI_Back;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @Mouse.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMouse;
                @Mouse.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMouse;
                @Mouse.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMouse;
                @Controller.started -= m_Wrapper.m_UIActionsCallbackInterface.OnController;
                @Controller.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnController;
                @Controller.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnController;
                @Back.started -= m_Wrapper.m_UIActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnBack;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Mouse.started += instance.OnMouse;
                @Mouse.performed += instance.OnMouse;
                @Mouse.canceled += instance.OnMouse;
                @Controller.started += instance.OnController;
                @Controller.performed += instance.OnController;
                @Controller.canceled += instance.OnController;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
            }
        }
    }
    public UIActions @UI => new UIActions(this);
    public interface IPlayerGameplayActions
    {
        void OnPause(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnControllerAim(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnSlowTime(InputAction.CallbackContext context);
        void OnHeal(InputAction.CallbackContext context);
        void OnReset(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnKill(InputAction.CallbackContext context);
        void OnClearEncounter(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnMouse(InputAction.CallbackContext context);
        void OnController(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
    }
}
