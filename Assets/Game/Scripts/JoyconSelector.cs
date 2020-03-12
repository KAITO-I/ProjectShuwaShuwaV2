using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Manager;

namespace Game.Title
{
    class JoyconSelector : MonoBehaviour
    {
        JoyconUI[] joyconUIs = new JoyconUI[4];

        [SerializeField]
        Sprite joyconLeftSprite;
        [SerializeField]
        Sprite joyconRightSprite;

        int joyconsCount;
        int selectedNum;

        bool active = false;
        public bool Active {
            get { return this.active; }
            set
            {
                if (this.active == value) return;

                gameObject.SetActive(this.active = value);
                if (this.active)
                {
                    var usingJoycon = JoyconManager.SelectedJoycon;
                    var joycons     = JoyconManager.ConntectedJoycons;
                    
                    this.joyconsCount = Mathf.Clamp(joycons.Count, 0, 4);

                    // 使用しているJoyConと一致する配列番号を取得
                    this.selectedNum = 0;
                    for (int i = 0; i < joyconsCount; i++)
                    {
                        if (joycons[0] == usingJoycon)
                        {
                            this.selectedNum = i;
                            break;
                        }
                    }

                    // UIに反映
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < joyconsCount) this.joyconUIs[i].Show(joycons[i].IsLeft ? this.joyconLeftSprite : this.joyconRightSprite);
                        else                  this.joyconUIs[i].Hide();
                    }
                    this.joyconUIs[this.selectedNum].SetCursor(true);
                }
            }
        }

        void Awake()
        {
            for (int i = 1; i <= 4; i++) this.joyconUIs[i - 1] = new JoyconUI(transform.Find("Joycons").Find(i + "P"));
        }

        void Update()
        {
            if (!this.active) return;

            // 左右キーで選択
            var selectedNum = Mathf.Clamp(this.selectedNum + JoyconManager.GetAxisX(), 0, this.joyconsCount - 1);
            if (this.selectedNum != selectedNum)
            {
                this.joyconUIs[this.selectedNum].SetCursor(false);
                this.joyconUIs[this.selectedNum = selectedNum].SetCursor(true);
            }

            // ZLまたはZRで振動
            if (JoyconManager.GetButton(Manager.Button.SHOLDER_2)) JoyconManager.ConntectedJoycons[this.selectedNum].SetRumble(160f, 320f, 0.5f, 15);
        }
    }

    class JoyconUI
    {
        GameObject ui;

        GameObject cursor;
        Image lr;

        public JoyconUI(Transform ui)
        {
            this.ui = ui.gameObject;

            this.cursor = ui.Find("Cursor").gameObject;
            this.cursor.SetActive(false);

            this.lr = ui.Find("LR").GetComponent<Image>();
        }

        public void Show(Sprite lr)
        {
            this.ui.SetActive(true);
            this.lr.sprite = lr;
        }

        public void SetCursor(bool select)
        {
            this.cursor.SetActive(select);
        }

        public void Rumble()
        {

        }

        public void Hide()
        {
            this.ui.SetActive(false);
        }
    }
}
