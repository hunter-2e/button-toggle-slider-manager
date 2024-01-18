using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {
    public enum ButtonEventType {
        SEARCH, NAVIGATION, PENTOOL
    }

    [System.Serializable]
    public struct ButtonAndChildrenButtons {
        public ButtonEventType buttonName;
        public GameObject buttonGameObject;
        public List<ButtonAndChildrenButtons> childrenButtons;
    }

    public List<ButtonAndChildrenButtons> buttonHierarchy;

    private static readonly IDictionary<ButtonEventType, UnityEvent> GameEvents = new Dictionary<ButtonEventType, UnityEvent>();

    private void Start() {
       SetupButtonEvents(buttonHierarchy);
    }

    private void SetupButtonEvents(List<ButtonAndChildrenButtons> buttonsAndChildrenButtons) {
        foreach(ButtonAndChildrenButtons button in buttonsAndChildrenButtons) {
            GameEvents[button.buttonName] = button.buttonGameObject.GetComponentInChildren<Button>().onClick;
            Debug.Log(button.buttonName);
            SetupButtonEvents(button.childrenButtons);
        }
   
    }

    public static void Subscribe(ButtonEventType eventType, UnityAction listener) {
        UnityEvent thisEvent;
        if (GameEvents.TryGetValue(eventType, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            GameEvents.Add(eventType, thisEvent);
        }
    }

    public static void Unsubscribe(ButtonEventType eventType, UnityAction listener) {
        UnityEvent thisEvent;
        if (GameEvents.TryGetValue(eventType, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void Publish(ButtonEventType eventType) {
        UnityEvent thisEvent;
        if (GameEvents.TryGetValue(eventType, out thisEvent)) {
            thisEvent.Invoke();
        }
    }
}
