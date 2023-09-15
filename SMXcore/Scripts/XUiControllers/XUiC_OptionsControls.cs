using InControl;
using Platform;
using Quartz.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMXcore
{
    public class XUiC_OptionsControls : XUiController
    {
        private struct ControllerLabelMapping
        {
            public readonly string[] ControlTypeNames;

            public readonly InputControlType[] ControlTypes;

            public readonly XUiV_Label[] Labels;

            public ControllerLabelMapping(string[] _controlTypeNames, XUiV_Label[] _labels, InputControlType[] _controlTypes = null)
            {
                ControlTypeNames = _controlTypeNames;
                if (_controlTypes == null)
                {
                    ControlTypes = new InputControlType[_controlTypeNames.Length];
                    for (int i = 0; i < _controlTypeNames.Length; i++)
                    {
                        ControlTypes[i] = EnumUtils.Parse<InputControlType>(_controlTypeNames[i], _ignoreCase: true);
                    }
                }
                else
                {
                    ControlTypes = _controlTypes;
                }

                Labels = _labels;
            }

            public ControllerLabelMapping(string _controlTypeName, XUiV_Label[] _labels)
            {
                ControlTypeNames = new string[] { _controlTypeName };
                ControlTypes = new InputControlType[] { EnumUtils.Parse<InputControlType>(_controlTypeName, _ignoreCase: true) };
                Labels = _labels;
            }
        }

        public static string ID = "";

        private XUiC_TabSelector tabs;

        private XUiC_ComboBoxFloat comboLookSensitivity;
        private XUiC_ComboBoxFloat comboZoomSensitivity;
        private XUiC_ComboBoxFloat comboZoomAccel;
        private XUiC_ComboBoxFloat comboVehicleSensitivity;
        private XUiC_ComboBoxBool comboWeaponAiming;
        private XUiC_ComboBoxBool comboInvertMouseLookY;
        private XUiC_ComboBoxBool comboAllowController;
        private XUiC_ComboBoxBool comboControllerVibration;
        private XUiC_ComboBoxFloat comboControllerInterfaceSensitivity;
        private XUiC_ComboBoxBool comboShowDS4;
        private XUiC_ComboBoxList<string> comboShowBindingsFor;

        private XUiC_SimpleButton btnBack;
        private XUiC_SimpleButton btnDefaults;
        private XUiC_SimpleButton btnApply;

        private GameObject headerTemplate;
        private GameObject controlTemplate;

        private Color unbindButtonDefaultColor;
        private Color unbindButtonHoverColor;

        private readonly Dictionary<PlayerAction, UILabel> actionToValueLabel = new Dictionary<PlayerAction, UILabel>();

        private bool initialized;
        private bool closedForNewBinding;

        private readonly List<string> actionBindingsOnOpen = new List<string>();
        private readonly List<ControllerLabelMapping> labelsForControllers = new List<ControllerLabelMapping>();

        private PlayerActionsBase actionSetIngame;
        private PlayerActionsBase actionSetVehicles;
        private PlayerActionsBase actionSetMenu;

        public static event Action OnSettingsChanged;

        public override void Init()
        {
            base.Init();
            global::XUiC_OptionsControls.ID = base.WindowGroup.ID;
            comboLookSensitivity = GetChildById("LookSensitivity").GetChildByType<XUiC_ComboBoxFloat>();
            comboZoomSensitivity = GetChildById("ZoomSensitivity").GetChildByType<XUiC_ComboBoxFloat>();
            comboZoomAccel = GetChildById("ZoomAccel").GetChildByType<XUiC_ComboBoxFloat>();
            comboVehicleSensitivity = GetChildById("VehicleSensitivity").GetChildByType<XUiC_ComboBoxFloat>();
            comboWeaponAiming = GetChildById("WeaponAiming").GetChildByType<XUiC_ComboBoxBool>();
            comboInvertMouseLookY = GetChildById("InvertMouseLookY").GetChildByType<XUiC_ComboBoxBool>();
            comboAllowController = GetChildById("AllowController").GetChildByType<XUiC_ComboBoxBool>();
            comboControllerVibration = GetChildById("ControllerVibration").GetChildByType<XUiC_ComboBoxBool>();
            comboControllerInterfaceSensitivity = GetChildById("ControllerInterfaceSensitivity").GetChildByType<XUiC_ComboBoxFloat>();
            comboShowDS4 = GetChildById("ShowDS4").GetChildByType<XUiC_ComboBoxBool>();
            comboShowBindingsFor = GetChildById("ShowBindingsFor").GetChildByType<XUiC_ComboBoxList<string>>();
            comboLookSensitivity.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboZoomSensitivity.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboZoomAccel.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboVehicleSensitivity.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboWeaponAiming.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboInvertMouseLookY.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboAllowController.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboControllerVibration.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboControllerInterfaceSensitivity.OnValueChangedGeneric += Combo_OnValueChangedGeneric;
            comboLookSensitivity.Min = 0.05000000074505806;
            comboLookSensitivity.Max = 1.5;
            comboZoomSensitivity.Min = 0.05000000074505806;
            comboZoomSensitivity.Max = 1.0;
            comboZoomAccel.Min = 0.0;
            comboZoomAccel.Max = 3.0;
            comboVehicleSensitivity.Min = 0.05000000074505806;
            comboVehicleSensitivity.Max = 3.0;
            comboControllerInterfaceSensitivity.Min = 0.05000000074505806;
            comboControllerInterfaceSensitivity.Max = 1.5;
            comboShowDS4.OnValueChanged += ComboShowDs4_OnOnValueChanged;
            comboShowBindingsFor.OnValueChanged += ComboShowMenuBindings_OnOnValueChanged;
            tabs = GetChildById("tabs") as XUiC_TabSelector;
            btnBack = GetChildById("btnBack") as XUiC_SimpleButton;
            btnDefaults = GetChildById("btnDefaults") as XUiC_SimpleButton;
            btnApply = GetChildById("btnApply") as XUiC_SimpleButton;
            btnBack.OnPressed += BtnBack_OnPressed;
            btnDefaults.OnPressed += BtnDefaults_OnOnPressed;
            btnApply.OnPressed += BtnApply_OnPressed;
            headerTemplate = GetChildById("headerTemplate").ViewComponent.UiTransform.gameObject;
            controlTemplate = GetChildById("controlTemplate").ViewComponent.UiTransform.gameObject;
            XUiV_Button xUiV_Button = (XUiV_Button)GetChildById("controlTemplate").GetChildById("unbind").ViewComponent;
            unbindButtonDefaultColor = xUiV_Button.DefaultSpriteColor;
            unbindButtonHoverColor = xUiV_Button.HoverSpriteColor;
            actionSetIngame = xui.playerUI.playerInput;
            actionSetVehicles = xui.playerUI.playerInput.VehicleActions;
            actionSetMenu = xui.playerUI.playerInput.GUIActions;
            AddControllerLabelMappingsForButton("Menu");
            AddControllerLabelMappingsForButton("RightTrigger");
            AddControllerLabelMappingsForButton("RightBumper");
            AddControllerLabelMappingsForButton("Action4");
            AddControllerLabelMappingsForButton("Action3");
            AddControllerLabelMappingsForButton("Action2");
            AddControllerLabelMappingsForButton("Action1");
            AddControllerLabelMappingsForButton("RightStick", new string[] { "RightStickLeft", "RightStickRight", "RightStickUp", "RightStickDown" });
            AddControllerLabelMappingsForButton("RightStickButton");
            AddControllerLabelMappingsForButton("View");
            AddControllerLabelMappingsForButton("LeftTrigger");
            AddControllerLabelMappingsForButton("LeftBumper");
            AddControllerLabelMappingsForButton("LeftStick", new string[] { "LeftStickLeft", "LeftStickRight", "LeftStickUp", "LeftStickDown" });
            AddControllerLabelMappingsForButton("LeftStickButton");
            AddControllerLabelMappingsForButton("DPadUp");
            AddControllerLabelMappingsForButton("DPadLeft");
            AddControllerLabelMappingsForButton("DPadDown");
            AddControllerLabelMappingsForButton("DPadRight");
            updateControllerMappingLabels();
        }

        private void Combo_OnValueChangedGeneric(XUiController _sender)
        {
            btnApply.Enabled = true;
        }

        private void AddControllerLabelMappingsForButton(string _uiName, string[] _controlTypeNames = null, InputControlType[] _controlTypes = null)
        {
            XUiController[] childrenById = GetChildById("controllerlayout").GetChildrenById(_uiName);
            XUiV_Label[] array = new XUiV_Label[childrenById.Length];
            for (int i = 0; i < childrenById.Length; i++)
            {
                array[i] = childrenById[i].ViewComponent as XUiV_Label;
            }

            if (_controlTypeNames == null)
            {
                labelsForControllers.Add(new ControllerLabelMapping(_uiName, array));
            }
            else
            {
                labelsForControllers.Add(new ControllerLabelMapping(_controlTypeNames, array, _controlTypes));
            }
        }

        private void BtnApply_OnPressed(XUiController _sender, int _mouseButton)
        {
            applyChanges();
            btnApply.Enabled = false;
        }

        private void BtnDefaults_OnOnPressed(XUiController _sender, int _mouseButton)
        {
            GameOptionsManager.ResetGameOptions(GameOptionsManager.ResetType.Controls);
            updateOptions();
            updateActionBindingLabels();
            updateControllerMappingLabels();
            btnApply.Enabled = true;
        }

        private void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
        {
            closedForNewBinding = false;
            xui.playerUI.windowManager.Close(windowGroup.ID);
            xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, _bModal: true);
        }

        private void ComboShowDs4_OnOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
        {
            IsDirty = true;
            updateControllerMappingLabels();
        }

        private void ComboShowMenuBindings_OnOnValueChanged(XUiController _sender, string _s, string _newValue1)
        {
            IsDirty = true;
            updateControllerMappingLabels();
        }

        private void updateControllerMappingLabels()
        {
            List<PlayerAction> list = new List<PlayerAction>();
            PlayerActionsBase actionSet = ((comboShowBindingsFor.SelectedIndex <= 0) ? actionSetIngame : ((comboShowBindingsFor.SelectedIndex == 1) ? actionSetVehicles : actionSetMenu));
            int num = (comboShowDS4.Value ? 1 : 0);
            foreach (ControllerLabelMapping labelsForController in labelsForControllers)
            {
                string text = "";
                string text2 = "";
                InputControlType[] controlTypes = labelsForController.ControlTypes;
                for (int i = 0; i < controlTypes.Length; i++)
                {
                    controlTypes[i].GetBoundAction(actionSet, list);
                    if (list.Count <= 0)
                    {
                        continue;
                    }

                    foreach (PlayerAction item in list)
                    {
                        PlayerActionData.ActionUserData actionUserData = item.UserData as PlayerActionData.ActionUserData;
                        if (actionUserData != null)
                        {
                            if (actionUserData.appliesToInputType == PlayerActionData.EAppliesToInputType.Both || actionUserData.appliesToInputType == PlayerActionData.EAppliesToInputType.ControllerOnly)
                            {
                                if (text.Length > 0)
                                {
                                    text += ", ";
                                    text2 += ", ";
                                }

                                text += actionUserData.LocalizedName;
                                text2 += actionUserData.LocalizedDescription;
                            }
                        }
                        else if (ActionSetManager.DebugLevel > ActionSetManager.EDebugLevel.Off)
                        {
                            text += " !NULL! ";
                            text2 += " !NULL! ";
                        }
                    }
                }

                if (text.Length == 0)
                {
                    text = Localization.Get("inpUnboundControllerKey");
                    text2 = Localization.Get("inpUnboundControllerKeyTooltip");
                }

                int num2 = 0;
                if (labelsForController.Labels.Length > 1)
                {
                    num2 = num;
                }

                labelsForController.Labels[num2].Text = text;
                labelsForController.Labels[num2].ToolTip = text2;
            }
        }

        private void updateOptions()
        {
            comboLookSensitivity.Value = GamePrefs.GetFloat(EnumGamePrefs.OptionsLookSensitivity);
            comboZoomSensitivity.Value = GamePrefs.GetFloat(EnumGamePrefs.OptionsZoomSensitivity);
            comboZoomAccel.Value = GamePrefs.GetFloat(EnumGamePrefs.OptionsZoomAccel);
            comboVehicleSensitivity.Value = GamePrefs.GetFloat(EnumGamePrefs.OptionsVehicleLookSensitivity);
            comboWeaponAiming.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsWeaponAiming);
            comboInvertMouseLookY.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsInvertMouse);
            comboAllowController.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsAllowController);
            comboControllerVibration.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerVibration);
            comboControllerInterfaceSensitivity.Value = GamePrefs.GetFloat(EnumGamePrefs.OptionsInterfaceSensitivity);
        }

        private void createControlsEntries()
        {
            SortedDictionary<PlayerActionData.ActionTab, SortedDictionary<PlayerActionData.ActionGroup, List<PlayerAction>>> sortedDictionary = new SortedDictionary<PlayerActionData.ActionTab, SortedDictionary<PlayerActionData.ActionGroup, List<PlayerAction>>>();
            PlayerActionsBase[] array = new PlayerActionsBase[5]
            {
            base.xui.playerUI.playerInput,
            base.xui.playerUI.playerInput.VehicleActions,
            base.xui.playerUI.playerInput.PermanentActions,
            base.xui.playerUI.playerInput.GUIActions,
            PlayerActionsGlobal.Instance
            };
            for (int i = 0; i < array.Length; i++)
            {
                foreach (PlayerAction action in ((PlayerActionSet)array[i]).Actions)
                {
                    PlayerActionData.ActionUserData actionUserData = action.UserData as PlayerActionData.ActionUserData;
                    if (actionUserData == null)
                    {
                        continue;
                    }

                    switch (actionUserData.appliesToInputType)
                    {
                        default:
                            throw new ArgumentOutOfRangeException();
                        case PlayerActionData.EAppliesToInputType.KbdMouseOnly:
                        case PlayerActionData.EAppliesToInputType.Both:
                            {
                                SortedDictionary<PlayerActionData.ActionGroup, List<PlayerAction>> sortedDictionary2;
                                if (sortedDictionary.ContainsKey(actionUserData.actionGroup.actionTab))
                                {
                                    sortedDictionary2 = sortedDictionary[actionUserData.actionGroup.actionTab];
                                }
                                else
                                {
                                    sortedDictionary2 = new SortedDictionary<PlayerActionData.ActionGroup, List<PlayerAction>>();
                                    sortedDictionary.Add(actionUserData.actionGroup.actionTab, sortedDictionary2);
                                }

                                List<PlayerAction> list;
                                if (sortedDictionary2.ContainsKey(actionUserData.actionGroup))
                                {
                                    list = sortedDictionary2[actionUserData.actionGroup];
                                }
                                else
                                {
                                    list = new List<PlayerAction>();
                                    sortedDictionary2.Add(actionUserData.actionGroup, list);
                                }

                                list.Add(action);
                                break;
                            }
                        case PlayerActionData.EAppliesToInputType.None:
                        case PlayerActionData.EAppliesToInputType.ControllerOnly:
                            break;
                    }
                }
            }

            int num = 1;
            foreach (KeyValuePair<PlayerActionData.ActionTab, SortedDictionary<PlayerActionData.ActionGroup, List<PlayerAction>>> item in sortedDictionary)
            {
                tabs.SetTabCaption(num, Localization.Get(item.Key.tabNameKey));
                Transform uiTransform = tabs.GetTabRect(num).Controller.GetChildById("controlsGrid").ViewComponent.UiTransform;
                XUiV_ScrollView scrollView = tabs.GetTabRect(num).Controller.GetChildById("scrollview").ViewComponent as XUiV_ScrollView;
                int num2 = 0;
                foreach (KeyValuePair<PlayerActionData.ActionGroup, List<PlayerAction>> item2 in item.Value)
                {
                    if (num2 > 0)
                    {
                        createHeader(uiTransform, null, scrollView);
                        createHeader(uiTransform, null, scrollView);
                    }

                    num2++;
                    int num3 = 0;
                    foreach (PlayerAction item3 in item2.Value)
                    {
                        createControl(uiTransform, item3, num3, scrollView);
                        num3++;
                    }

                    if (num3 % 2 != 0)
                    {
                        createHeader(uiTransform, null, scrollView);
                    }
                }

                ((XUiV_Grid)tabs.GetTabRect(num).Controller.GetChildById("controlsGrid").ViewComponent).Grid.Reposition();
                num++;
            }
        }

        private void Tooltip(bool _show, string _tooltip)
        {
            if (xui.currentToolTip != null)
            {
                xui.currentToolTip.ToolTip = ((!string.IsNullOrEmpty(_tooltip) && _show) ? _tooltip : "");
            }
        }

        private void createHeader(Transform _parent, PlayerActionData.ActionGroup _group, XUiV_ScrollView scrollView)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(headerTemplate, _parent);
            if (_group == null)
            {
                return;
            }

            Transform transform = gameObject.transform.Find("caption");
            transform.GetComponent<UILabel>().text = _group.LocalizedName.ToUpper();
            string tooltip = _group.LocalizedDescription;
            if (tooltip != null)
            {
                UIEventListener.Get(transform.gameObject).onTooltip = delegate (GameObject _go, bool _show)
                {
                    Tooltip(_show, tooltip);
                };

                if (scrollView != null)
                {
                    UIEventListener.Get(transform.gameObject).onScroll += scrollView.OnScroll;
   
                }
            }
        }

        private void createControl(Transform _parent, PlayerAction _action, int _controlNum, XUiV_ScrollView scrollView)
        {
            PlayerActionData.ActionUserData actionUserData = (PlayerActionData.ActionUserData)_action.UserData;
            GameObject gameObject = UnityEngine.Object.Instantiate(controlTemplate, _parent);
            gameObject.name = $"{_controlNum:00}_{_action.Name}";
            Transform transform = gameObject.transform.Find("label");
            transform.GetComponent<UILabel>().text = actionUserData.LocalizedName;
            string tooltip = actionUserData.LocalizedDescription;
            if (tooltip != null)
            {
                UIEventListener.Get(transform.gameObject).onTooltip = delegate (GameObject _go, bool _show)
                {
                    Tooltip(_show, tooltip);
                };

                if (scrollView != null)
                {
                    UIEventListener.Get(transform.gameObject).onScroll += scrollView.OnScroll;

                }
            }

            actionToValueLabel.Add(_action, gameObject.transform.Find("value").GetComponent<UILabel>());
            Transform transform2 = gameObject.transform.Find("background");
            Transform transform3 = gameObject.transform.Find("unbind");
            UISprite sprite = transform3.GetComponent<UISprite>();
            if (actionUserData.allowRebind)
            {
                string unbdingTooltip = Localization.Get("xuiRemoveBinding");
                UIEventListener.Get(transform2.gameObject).onClick = delegate
                {
                    newBindingClick(_action);
                };
                UIEventListener.Get(transform3.gameObject).onClick = delegate
                {
                    unbindButtonClick(_action);
                };
                UIEventListener.Get(transform3.gameObject).onHover = delegate (GameObject _go, bool _state)
                {
                    unbindButtonHover(_action, sprite, _state);
                };
                UIEventListener.Get(transform3.gameObject).onTooltip = delegate (GameObject _go, bool _show)
                {
                    Tooltip(_show, unbdingTooltip);
                };

                if (scrollView != null)
                {
                    UIEventListener.Get(transform2.gameObject).onScroll += scrollView.OnScroll;
                    UIEventListener.Get(transform3.gameObject).onScroll += scrollView.OnScroll;

                } 
            }
            else
            {
                transform2.GetComponent<UISprite>().alpha = 0.1f;
                transform3.GetComponent<UISprite>().alpha = 0f;
            }
        }

        private IEnumerator updateActionBindingLabelsLater()
        {
            yield return null;
            updateActionBindingLabels();
        }

        private void updateActionBindingLabels()
        {
            foreach (KeyValuePair<PlayerAction, UILabel> item in actionToValueLabel)
            {
                item.Value.text = item.Key.GetBindingString(_forController: false);
            }
        }

        private void newBindingClick(PlayerAction _action)
        {
            closedForNewBinding = true;
            btnApply.Enabled = true;
            base.xui.playerUI.windowManager.Close(windowGroup.ID);
            XUiC_OptionsControlsNewBinding.GetNewBinding(base.xui, _action, windowGroup.ID);
        }

        private void unbindButtonClick(PlayerAction _action)
        {
            _action.UnbindBindingsOfType(_controller: false);
            ThreadManager.StartCoroutine(updateActionBindingLabelsLater());
            btnApply.Enabled = true;
        }

        private void unbindButtonHover(PlayerAction _action, UISprite _sprite, bool _state)
        {
            _sprite.color = (_state ? unbindButtonHoverColor : unbindButtonDefaultColor);
        }

        private void applyChanges()
        {
            GamePrefs.Set(EnumGamePrefs.OptionsLookSensitivity, (float)comboLookSensitivity.Value);
            GamePrefs.Set(EnumGamePrefs.OptionsZoomSensitivity, (float)comboZoomSensitivity.Value);
            GamePrefs.Set(EnumGamePrefs.OptionsZoomAccel, (float)comboZoomAccel.Value);
            GamePrefs.Set(EnumGamePrefs.OptionsVehicleLookSensitivity, (float)comboVehicleSensitivity.Value);
            GamePrefs.Set(EnumGamePrefs.OptionsWeaponAiming, comboWeaponAiming.Value);
            GamePrefs.Set(EnumGamePrefs.OptionsInvertMouse, comboInvertMouseLookY.Value);
            GamePrefs.Set(EnumGamePrefs.OptionsAllowController, comboAllowController.Value);
            GamePrefs.Set(EnumGamePrefs.OptionsControllerVibration, comboControllerVibration.Value);
            GamePrefs.Set(EnumGamePrefs.OptionsInterfaceSensitivity, (float)comboControllerInterfaceSensitivity.Value);
            GameOptionsManager.SaveControls();
            GamePrefs.Instance.Save();
            PlayerMoveController.UpdateControlsOptions();
            storeCurrentBindings();
            XUiC_OptionsControls.OnSettingsChanged?.Invoke();
        }

        private void storeCurrentBindings()
        {
            actionBindingsOnOpen.Clear();
            foreach (PlayerActionsBase actionSet in PlatformManager.NativePlatform.Input.ActionSets)
            {
                actionBindingsOnOpen.Add(((PlayerActionSet)actionSet).Save());
            }
        }

        public override void OnOpen()
        {
            base.WindowGroup.openWindowOnEsc = XUiC_OptionsMenu.ID;
            if (!initialized)
            {
                createControlsEntries();
                initialized = true;
            }

            if (!closedForNewBinding)
            {
                updateOptions();
                storeCurrentBindings();
            }

            closedForNewBinding = false;
            updateActionBindingLabels();
            base.OnOpen();
            headerTemplate.SetActive(value: false);
            controlTemplate.SetActive(value: false);
            PlayerInputManager.InputStyle currentInputStyle = PlatformManager.NativePlatform.Input.CurrentInputStyle;
            if (currentInputStyle != PlayerInputManager.InputStyle.Keyboard)
            {
                tabs.SelectedTabIndex = tabs.TabCount - 1;
                bool flag = currentInputStyle == PlayerInputManager.InputStyle.PS4;
                if (flag != comboShowDS4.Value)
                {
                    comboShowDS4.Value = flag;
                    updateControllerMappingLabels();
                }
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            if (!closedForNewBinding)
            {
                for (int i = 0; i < actionBindingsOnOpen.Count && i < PlatformManager.NativePlatform.Input.ActionSets.Count; i++)
                {
                    ((PlayerActionSet)PlatformManager.NativePlatform.Input.ActionSets[i]).Load(actionBindingsOnOpen[i]);
                }

                btnApply.Enabled = false;
            }
        }

        public override void Update(float _dt)
        {
            base.Update(_dt);
            if (IsDirty)
            {
                RefreshBindings();
                IsDirty = false;
            }
        }

        public override bool GetBindingValue(ref string _value, string _bindingName)
        {
            switch (_bindingName)
            {
                case "isds4":
                    _value = (comboShowDS4 != null && comboShowDS4.Value).ToString();
                    return true;
                case "isxb1":
                    _value = (comboShowDS4 != null && !comboShowDS4.Value).ToString();
                    return true;
                case "controller_atlas":
                    _value = ((comboShowDS4 != null && comboShowDS4.Value) ? "UIAtlas_GUI_2_PS4" : "UIAtlas_GUI_2_XBox");
                    return true;
                default:
                    return base.GetBindingValue(ref _value, _bindingName);
            }
        }
    }
}
