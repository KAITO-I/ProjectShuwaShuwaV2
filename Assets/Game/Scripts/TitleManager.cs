using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Manager;

namespace Game.Title
{
	public delegate void SelectedMenuButton();

	public class TitleManager : MonoBehaviour
	{
		[SerializeField]
		Transform buttonsParent;

		MenuButton[] button;
		int selectButtonNum = 0;

		bool selectedButton = false;

		[SerializeField]
		float buttonFunctionTime = 0.5f;

		[SerializeField]
		JoyconSelector jSelector;
		
		void Start()
		{
			this.button = new MenuButton[3];
			this.button[0] = new MenuButton(this.buttonsParent.Find("Play"));
			this.button[1] = new MenuButton(this.buttonsParent.Find("Ranking"));
			this.button[2] = new MenuButton(this.buttonsParent.Find("End"));

			this.jSelector.Active = true;

			SoundManager.PlayBGM(BGMID.Title);
		}
		
		void Update()
		{
			if (this.selectedButton) return;
			if (this.jSelector.Active) return;

			// 上下移動
			var selectedButtonNum = this.selectButtonNum;
			selectedButtonNum -= JoyconManager.GetAxisY();
			selectedButtonNum = Mathf.Clamp(selectedButtonNum, 0, 2);
			if (selectedButtonNum != this.selectButtonNum)
			{
				this.button[this.selectButtonNum].Select(false);
				this.selectButtonNum = selectedButtonNum;
				this.button[this.selectButtonNum].Select(true);
			}

			// 決定
			if (JoyconManager.IsAnyButtonDown())
			{
				this.selectedButton = true;
				SoundManager.PlaySE(SEID.UISelect);
				this.button[this.selectButtonNum].Selected();
				StartCoroutine(PushButton());
			}
		}

		IEnumerator PushButton()
		{
			yield return new WaitForSeconds(this.buttonFunctionTime);
		}
	}

	public class MenuButton
	{
		Image[] images;

		public MenuButton(Transform tf)
		{
			this.images = new Image[3];
			this.images[0] = tf.Find("Normal").GetComponent<Image>();
			this.images[1] = tf.Find("Select").GetComponent<Image>();
			this.images[2] = tf.Find("Selected").GetComponent<Image>();
		}

		public void Select(bool select)
		{
			this.images[0].gameObject.SetActive(!select);
			this.images[1].gameObject.SetActive(select);
		}

		public void Selected()
		{
			this.images[0].gameObject.SetActive(false);
			this.images[1].gameObject.SetActive(false);
			this.images[2].gameObject.SetActive(true);
		}
	}
}