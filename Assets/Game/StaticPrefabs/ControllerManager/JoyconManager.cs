using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
	public enum Button
	{
		A,
		B,
		OPTION,
		SL,
		SR,
		SHOLDER_2
	}

	class JoyconManager : MonoBehaviour
	{
		//==============================
		// static
		//==============================
		static JoyconManager instance = null;
		static List<Joycon> connectedJoycons = new List<Joycon>();
		static Joycon       selectedJoycon   = null;

		public static IList<Joycon> ConntectedJoycons { get { return connectedJoycons.AsReadOnly(); } }
		public static Joycon        SelectedJoycon    { get { return selectedJoycon; } }

		public static float AxisDead { get { return instance.axisDead; } }

		public static void ReloadJoycons()
		{
			connectedJoycons.Clear();
			foreach (JoyconLib.Joycon joycon in JoyconLib.JoyconManager.Instance.j) connectedJoycons.Add(new Joycon(joycon));
		}

		public static int GetAxisX()
		{
			var axis = 0;

			foreach (Joycon joycon in connectedJoycons)
			{
				axis = joycon.GetAxisDownX();
				if (axis != 0) break;
			}

			return axis;
		}

		public static int GetAxisY()
		{
			var axis = 0;

			foreach (Joycon joycon in connectedJoycons)
			{
				axis = joycon.GetAxisDownY();
				if (axis != 0) break;
			}

			return axis;
		}

		public static bool IsAnyButtonDown()
		{
			var down = false;

			foreach (Joycon joycon in connectedJoycons)
			{
				down = joycon.IsAnyButtonDown();
				if (down) break;
			}

			return down;
		}

		public static bool GetButton(Button button)
		{
			foreach (Joycon joycon in connectedJoycons)
				if (joycon.GetButton(button)) return true;
			return false;
		}

		//==============================
		// instance
		//==============================
		[SerializeField]
		float axisDead = 0.25f;

		void Awake()
		{
			if (instance != null)
			{
				Destroy(this.gameObject);
				return;
			}

			instance = this;
			DontDestroyOnLoad(this.gameObject);

			ReloadJoycons();
		}

		void Start()
		{
			ReloadJoycons();
		}
	}

	class Joycon
	{
		//==============================
		// instance
		//==============================
		readonly JoyconLib.Joycon joycon;

		public bool IsLeft { get { return this.joycon.isLeft; } }

		// STICK
		int[] axis = { 0, 0 };

		// BUTTON
		readonly JoyconLib.Joycon.Button[] buttons;
		readonly JoyconLib.Joycon.Button[] anyButtons =
		{
			JoyconLib.Joycon.Button.DPAD_LEFT,
			JoyconLib.Joycon.Button.DPAD_RIGHT,
			JoyconLib.Joycon.Button.DPAD_UP,
			JoyconLib.Joycon.Button.DPAD_DOWN,
			JoyconLib.Joycon.Button.SHOULDER_1,
			JoyconLib.Joycon.Button.SHOULDER_2,
			JoyconLib.Joycon.Button.SL,
			JoyconLib.Joycon.Button.SR,
		};

		/// <summary>
		/// JoyconLibのJoyconクラスの情報を使って、オリジナルのJoyconクラスを新しく生成します。
		/// </summary>
		/// <param name="joycon">接続されたJoycon</param>
		public Joycon(JoyconLib.Joycon joycon)
		{
			this.joycon = joycon;

			this.buttons = new JoyconLib.Joycon.Button[]
			{
				JoyconLib.Joycon.Button.DPAD_RIGHT,
				JoyconLib.Joycon.Button.DPAD_DOWN,
				(joycon.isLeft ? JoyconLib.Joycon.Button.MINUS : JoyconLib.Joycon.Button.PLUS),
				JoyconLib.Joycon.Button.SL,
				JoyconLib.Joycon.Button.SR,
				JoyconLib.Joycon.Button.SHOULDER_2
			};
		}

		//==============================
		// Axis
		//==============================
		/// <summary>
		/// スティックにおけるX軸の入力状態を取得します。
		/// ただし、連続した入力では0を返すため、一回だけ入力を受け付ける処理で使うことが最適です。
		/// </summary>
		/// <returns>入力状態 (0:NONE, 1:RIGHT, -1:LEFT)</returns>
		public int GetAxisDownX()
		{
			var axisX = this.joycon.GetStick()[0];

			// DEAD未満の値は入力としない
			if (Mathf.Abs(axisX) < JoyconManager.AxisDead) return this.axis[0] = 0;

			// 1F前の値と比較し、同じ値であれば処理しない
			axisX = axisX > 0f ? 1f : -1f;
			return axis[0] == axisX ? 0 : axis[0] = (int)axisX;
		}

		/// <summary>
		/// スティックにおけるY軸の入力状態を取得します。
		/// ただし、連続した入力では0を返すため、一回だけ入力を受け付ける処理で使うことが最適です。
		/// </summary>
		/// <returns>入力状態 (0:NONE, 1:UP, -1:DOWN)</returns>
		public int GetAxisDownY()
		{
			var axisY = this.joycon.GetStick()[1];

			// DEAD未満の値は入力としない
			if (Mathf.Abs(axisY) < JoyconManager.AxisDead) return this.axis[1] = 0;

			// 1F前の値と比較し、同じ値であれば処理しない
			axisY = axisY > 0f ? 1f : -1f;
			return axis[1] == axisY ? 0 : axis[1] = (int)axisY;
		}

		//==============================
		// Button
		//==============================
		/// <summary>
		/// 予め指定されているボタンの中から、どれか一つでもボタンが押されたかどうかを返します。
		/// </summary>
		/// <returns>どれか一つでも押されたかどうか</returns>
		public bool IsAnyButtonDown()
		{
			foreach (JoyconLib.Joycon.Button button in this.anyButtons)
				if (this.joycon.GetButtonDown(button)) return true;

			return false;
		}

		public bool GetButton(Button button)
		{
			return this.joycon.GetButton(this.buttons[(int)button]);
		}

		/// <summary>
		/// 指定されたボタンが押されたかどうかを返します。
		/// </summary>
		/// <param name="button">チェックするボタン</param>
		/// <returns>押されたかどうか</returns>
		public bool GetButtonDown(Button button)
		{
			return this.joycon.GetButtonDown(this.buttons[(int)button]);
		}

		//==============================
		// 
		//==============================
		/// <summary>
		/// ジャイロ値を取得します。
		/// </summary>
		/// <returns>ジャイロ値</returns>
		public Vector3 GetGyro()
		{
			return this.joycon.GetGyro();
		}

		public void SetRumble(float low_freq, float high_freq, float amp, int time)
		{
			this.joycon.SetRumble(low_freq, high_freq, amp, time);
		}
	}
}