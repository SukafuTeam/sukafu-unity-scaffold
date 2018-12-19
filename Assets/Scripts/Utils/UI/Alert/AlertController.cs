using System;
using UnityEngine;

public class AlertController : MonoBehaviour
{

	public static AlertController Instance;
	public GameObject AlertCanvasPrefab;

	[HideInInspector]
	public string Title;
	[HideInInspector]
	public string Body;
	[HideInInspector]
	public string ConfirmButtonText;
	[HideInInspector]
	public Action ConfirmButtonAction;
	[HideInInspector]
	public string CancelButtonText;
	[HideInInspector]
	public Action CancelButtonAction;

	public void Awake()
	{
		Instance = this;
	}
	
	public AlertController SetTitle(string title)
	{
		Title = title;

		Body = "";
		ConfirmButtonText = "";
		ConfirmButtonAction = null;
		CancelButtonText = "";
		CancelButtonAction = null;
		
		return this;
	}

	public AlertController SetBody(string body)
	{
		Body = body;
		return this;
	}

	public AlertController SetConfirmButton(string text, Action action = null)
	{
		ConfirmButtonText = text;
		ConfirmButtonAction = action;
		return this;
	}
	
	public AlertController SetCancelButton(string text, Action action = null)
	{
		CancelButtonText = text;
		CancelButtonAction = action;
		return this;
	}

	public void Show()
	{
		var clone = Instantiate(AlertCanvasPrefab);
		clone.GetComponent<AlertCanvasController>().Show(
			Title, Body, ConfirmButtonText, ConfirmButtonAction, CancelButtonText, CancelButtonAction);
	}
	
	
}
