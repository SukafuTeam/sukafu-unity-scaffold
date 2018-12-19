using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AlertCanvasController : MonoBehaviour
{
	public Image Background;
	public Text Title;
	public Text Body;
	public Text Confirm;
	public Action ConfirmAction;
	public Text Cancel;
	public Action CancelAction;

	public GameObject AlertObject;
	public GameObject ConfirmObject;
	public GameObject CancelObject;
	
	public void Show(string title, string body, string confirm, Action confirmAcion, string cancel, Action cancelAction)
	{
		ConfirmObject.SetActive(confirm != "");
		CancelObject.SetActive(cancel != "");
		
		Title.text = title;
		Body.text = body;
		Confirm.text = confirm;
		ConfirmAction = confirmAcion;
		Cancel.text = cancel;
		CancelAction = cancelAction;

		AlertObject.transform.localScale = Vector3.zero;

		Background.DOFade(0.7f, 0.5f);
		AlertObject.transform
			.DOScale(Vector3.one, 0.5f)
			.SetEase(Ease.InOutElastic);
	}

	public void OnConfirm()
	{
		if(ConfirmAction != null)
			ConfirmAction.Invoke();
		Hide();
	}

	public void OnCancel()
	{
		if(CancelAction != null)
			CancelAction.Invoke();
		Hide();
	}

	public void Hide()
	{
		Background.DOFade(0, 0.5f);
		AlertObject.transform
			.DOScale(Vector3.zero, 0.5f)
			.SetEase(Ease.InOutElastic)
			.OnComplete(() =>
				{
					Destroy(gameObject);
				}
			);
	}
}
