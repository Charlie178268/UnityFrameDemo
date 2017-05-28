using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//把控件脚本注册到UIManager
//可以直接查找子控件
//可以挂载在所有控件上面，负责注册和移除事件
/// <summary>
/// 组件化开发的思想，将事件的注册和移除作为一个组件单独出来，需要用到的时候就挂载
/// </summary>
public class UIBehaviour : MonoBehaviour {
	void Awake(){
		//把控件添加到UIManger的管理中,可以直接查找子控件
		UIManager.Instance.RegistGameObject(this.name, gameObject);
	}
	//按钮的增加和移除事件
	public void AddButtonListener(UnityAction action){
		if (action != null){
			Button btn = transform.GetComponent<Button>();
			btn.onClick.AddListener(action);
		}
	}
	
	public void RemoveButtonListener(UnityAction action){
		if (action != null){
			Button btn = transform.GetComponent<Button>();
			btn.onClick.RemoveListener(action);
		}
	}

    //按钮的按下事件,需要自己写
    public void AddButtonDownListener(UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;    // 设置回调函数
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(action);
        // 添加事件触发记录到GameObject的事件触发组件
        trigger.triggers.Add(entry);
    }

    //按钮的抬起事件,需要自己写
    public void AddButtonUpListener(UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;    // 设置回调函数
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(action);
        // 添加事件触发记录到GameObject的事件触发组件
        trigger.triggers.Add(entry);
    }


    //单选按钮
    public void AddToggleListener(UnityAction<bool> action){
		if (action != null){
			Toggle btn = transform.GetComponent<Toggle>();
			btn.onValueChanged.AddListener(action);
		}
	}
	
	public void RemoveToggleListener(UnityAction<bool> action){
		if (action != null){
			Toggle btn = transform.GetComponent<Toggle>();
			btn.onValueChanged.RemoveListener(action);
		}
	}
	//滑动条
	public void AddSliderListener(UnityAction<float> action){
		if (action != null){
			Slider btn = transform.GetComponent<Slider>();
			btn.onValueChanged.AddListener(action);
		}
	}
	
	public void RemoveSliderListener(UnityAction<float> action){
		if (action != null){
			Slider btn = transform.GetComponent<Slider>();
			btn.onValueChanged.RemoveListener(action);
		}
	}
	//输入框
	public void AddInputListener(UnityAction<string> action){
		if (action != null){
			InputField btn = transform.GetComponent<InputField>();
			btn.onValueChanged.AddListener(action);
		}
	}
	
	public void RemoveInputListener(UnityAction<string> action){
		if (action != null){
			InputField btn = transform.GetComponent<InputField>();
			btn.onValueChanged.RemoveListener(action);
		}
	}
}
