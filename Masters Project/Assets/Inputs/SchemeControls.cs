//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.2
//     from Assets/Inputs/SchemeControls.inputactions
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

public partial class @SchemeControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @SchemeControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""SchemeControls"",
    ""maps"": [
        {
            ""name"": ""ObserveGamePad"",
            ""id"": ""55112b08-cdca-4988-92e5-a15cbc0544c8"",
            ""actions"": [
                {
                    ""name"": ""SwapControls"",
                    ""type"": ""PassThrough"",
                    ""id"": ""80067c6a-25fe-49f3-997f-ac9f7ef5adb0"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AxisInput"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fabab959-56c7-44bf-90c2-933d4316c3a5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b1c2e240-5115-49d1-85b3-84b55ee88e7b"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2476745b-b712-4b49-9c82-86f625adf6e7"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f15c32eb-5044-4c67-abb0-88d15525bdd8"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03be254a-9d83-4429-9108-6f3d7be255cf"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""751ddc28-8338-43b8-8b10-0f231cd3de5c"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cbfc2da3-2101-4c33-ab52-51d564cb07c1"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aa8301a2-6595-451d-8425-d89e220c02af"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""511deb19-c52e-487e-92b7-3f25d8444fcd"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e9142906-3e1f-41a2-ab35-e934bd524ab4"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb37d18d-b697-4f24-b5fd-d983cdf75a95"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2822a530-c31a-47e0-94d9-81e5c1f38ac1"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""394846a8-862d-4b46-aa8c-5bf140308bf0"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8ee3929-2cb9-41dc-87d5-60ca7d689663"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0669593f-5d92-4a9a-8a5e-427d628db3f9"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e2276e3-b316-478b-ba6a-c1a05a4e443e"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb2cbb2a-3ab1-47be-bbe5-5a67de86eb1f"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ccae47e-cf55-4f9a-9a01-334a84a47ab3"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7d8e2fe3-f5a5-446d-921c-a6cc45b2dfc3"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73f3b1ed-6ae3-4d95-aa82-cdc2513d91e3"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b18b990-5860-46dc-b0ba-01d565f74f2c"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone,ScaleVector2(x=10,y=10)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""AxisInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48aa559c-12eb-45fd-bbe4-0636ae8197d7"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone,ScaleVector2(x=10,y=10)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""AxisInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""ObserveMK"",
            ""id"": ""f656a77c-f906-4cf4-a6e4-d2dbcc79564d"",
            ""actions"": [
                {
                    ""name"": ""SwapControls"",
                    ""type"": ""PassThrough"",
                    ""id"": ""7b5155f0-ca53-43f9-8f46-47ab99d4a5b4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AxisInput"",
                    ""type"": ""PassThrough"",
                    ""id"": ""efea97c2-5af8-480a-bcc5-3b1b9fa61fc3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7f63b6a5-d335-4c70-8405-732f2106794a"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c04b053-957e-4b63-9c49-c3f16742753b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5bbbec27-c980-42fa-90a5-b6fbcdadabaa"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""SwapControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""06e68065-6daf-4bd5-97db-ec8474f655ee"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""AxisInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // ObserveGamePad
        m_ObserveGamePad = asset.FindActionMap("ObserveGamePad", throwIfNotFound: true);
        m_ObserveGamePad_SwapControls = m_ObserveGamePad.FindAction("SwapControls", throwIfNotFound: true);
        m_ObserveGamePad_AxisInput = m_ObserveGamePad.FindAction("AxisInput", throwIfNotFound: true);
        // ObserveMK
        m_ObserveMK = asset.FindActionMap("ObserveMK", throwIfNotFound: true);
        m_ObserveMK_SwapControls = m_ObserveMK.FindAction("SwapControls", throwIfNotFound: true);
        m_ObserveMK_AxisInput = m_ObserveMK.FindAction("AxisInput", throwIfNotFound: true);
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

    // ObserveGamePad
    private readonly InputActionMap m_ObserveGamePad;
    private IObserveGamePadActions m_ObserveGamePadActionsCallbackInterface;
    private readonly InputAction m_ObserveGamePad_SwapControls;
    private readonly InputAction m_ObserveGamePad_AxisInput;
    public struct ObserveGamePadActions
    {
        private @SchemeControls m_Wrapper;
        public ObserveGamePadActions(@SchemeControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SwapControls => m_Wrapper.m_ObserveGamePad_SwapControls;
        public InputAction @AxisInput => m_Wrapper.m_ObserveGamePad_AxisInput;
        public InputActionMap Get() { return m_Wrapper.m_ObserveGamePad; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ObserveGamePadActions set) { return set.Get(); }
        public void SetCallbacks(IObserveGamePadActions instance)
        {
            if (m_Wrapper.m_ObserveGamePadActionsCallbackInterface != null)
            {
                @SwapControls.started -= m_Wrapper.m_ObserveGamePadActionsCallbackInterface.OnSwapControls;
                @SwapControls.performed -= m_Wrapper.m_ObserveGamePadActionsCallbackInterface.OnSwapControls;
                @SwapControls.canceled -= m_Wrapper.m_ObserveGamePadActionsCallbackInterface.OnSwapControls;
                @AxisInput.started -= m_Wrapper.m_ObserveGamePadActionsCallbackInterface.OnAxisInput;
                @AxisInput.performed -= m_Wrapper.m_ObserveGamePadActionsCallbackInterface.OnAxisInput;
                @AxisInput.canceled -= m_Wrapper.m_ObserveGamePadActionsCallbackInterface.OnAxisInput;
            }
            m_Wrapper.m_ObserveGamePadActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SwapControls.started += instance.OnSwapControls;
                @SwapControls.performed += instance.OnSwapControls;
                @SwapControls.canceled += instance.OnSwapControls;
                @AxisInput.started += instance.OnAxisInput;
                @AxisInput.performed += instance.OnAxisInput;
                @AxisInput.canceled += instance.OnAxisInput;
            }
        }
    }
    public ObserveGamePadActions @ObserveGamePad => new ObserveGamePadActions(this);

    // ObserveMK
    private readonly InputActionMap m_ObserveMK;
    private IObserveMKActions m_ObserveMKActionsCallbackInterface;
    private readonly InputAction m_ObserveMK_SwapControls;
    private readonly InputAction m_ObserveMK_AxisInput;
    public struct ObserveMKActions
    {
        private @SchemeControls m_Wrapper;
        public ObserveMKActions(@SchemeControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SwapControls => m_Wrapper.m_ObserveMK_SwapControls;
        public InputAction @AxisInput => m_Wrapper.m_ObserveMK_AxisInput;
        public InputActionMap Get() { return m_Wrapper.m_ObserveMK; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ObserveMKActions set) { return set.Get(); }
        public void SetCallbacks(IObserveMKActions instance)
        {
            if (m_Wrapper.m_ObserveMKActionsCallbackInterface != null)
            {
                @SwapControls.started -= m_Wrapper.m_ObserveMKActionsCallbackInterface.OnSwapControls;
                @SwapControls.performed -= m_Wrapper.m_ObserveMKActionsCallbackInterface.OnSwapControls;
                @SwapControls.canceled -= m_Wrapper.m_ObserveMKActionsCallbackInterface.OnSwapControls;
                @AxisInput.started -= m_Wrapper.m_ObserveMKActionsCallbackInterface.OnAxisInput;
                @AxisInput.performed -= m_Wrapper.m_ObserveMKActionsCallbackInterface.OnAxisInput;
                @AxisInput.canceled -= m_Wrapper.m_ObserveMKActionsCallbackInterface.OnAxisInput;
            }
            m_Wrapper.m_ObserveMKActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SwapControls.started += instance.OnSwapControls;
                @SwapControls.performed += instance.OnSwapControls;
                @SwapControls.canceled += instance.OnSwapControls;
                @AxisInput.started += instance.OnAxisInput;
                @AxisInput.performed += instance.OnAxisInput;
                @AxisInput.canceled += instance.OnAxisInput;
            }
        }
    }
    public ObserveMKActions @ObserveMK => new ObserveMKActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IObserveGamePadActions
    {
        void OnSwapControls(InputAction.CallbackContext context);
        void OnAxisInput(InputAction.CallbackContext context);
    }
    public interface IObserveMKActions
    {
        void OnSwapControls(InputAction.CallbackContext context);
        void OnAxisInput(InputAction.CallbackContext context);
    }
}
