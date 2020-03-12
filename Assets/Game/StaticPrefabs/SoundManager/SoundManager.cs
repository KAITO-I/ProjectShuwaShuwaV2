using UnityEngine;
using UnityEngine.Audio;

namespace Game.Manager
{
    public enum BGMID
    {
        Title,
        InGame,
        Result
    }

    public enum SEID
    {
        UISelect
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        //==============================
        // static
        //==============================
        static SoundManager Instance = null;

        public static void PlayBGM(AudioClip clip)
        {
            var bgm = Instance.bgm;

            if (bgm.isPlaying) bgm.Stop();

            bgm.clip = clip;
            bgm.Play();
        }

        public static void PlayBGM(BGMID id)
        {
            PlayBGM(Instance.bgmClips[(int)id]);
        }

        public static void StopBGM()
        {
            Instance.bgm.Stop();
        }

        public static void PlaySE(SEID id)
        {
            PlaySE(Instance.seClips[(int)id]);
        }

        public static void PlaySE(AudioClip clip)
        {
            Instance.PlaySoundEffect(clip);
        }

        public static void EndPlaySE(SE se)
        {
            Instance.EndPlaySoundEffect(se);
        }

        //==============================
        // instance
        //==============================
        AudioSource bgm;

        [SerializeField]
        int sePlayables;

        [SerializeField]
        AudioMixerGroup seAMG;

        SE[] standbySEs;
        SE[] playingSEs;

        [SerializeField]
        AudioClip[] bgmClips;

        [SerializeField]
        AudioClip[] seClips;
        
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            this.bgm = GetComponent<AudioSource>();

            this.standbySEs = new SE[this.sePlayables];
            this.playingSEs = new SE[this.sePlayables];
            for (int i = 0; i < this.sePlayables; i++)
            {
                var obj = new GameObject("SE");

                var se = obj.AddComponent<SE>();
                se.Init(gameObject, this.seAMG);

                this.standbySEs[i] = se;
            }
        }

        void PlaySoundEffect(AudioClip clip)
        {
            SE se = null;

            // SE待機配列より待機中を取得
            for (int i = 0; i < this.standbySEs.Length; i++)
            {
                if (this.standbySEs[i] != null)
                {
                    se = this.standbySEs[i];
                    this.standbySEs[i] = null;
                    break;
                }
            }

            // 待機中が無ければ再生しない
            if (se == null) return;

            // 再生し、SE再生配列に追加
            se.Play(clip);
            for (int i = 0; i < this.playingSEs.Length; i++)
            {
                if (this.playingSEs[i] == null)
                {
                    this.playingSEs[i] = se;
                    break;
                }
            }
        }

        void EndPlaySoundEffect(SE se)
        {
            // SE再生配列より削除
            for (int i = 0; i < this.playingSEs.Length; i++)
            {
                if (this.playingSEs[i] == se)
                {
                    this.playingSEs[i] = null;
                    break;
                }
            }

            // SE待機配列に追加
            for (int i = 0; i < this.standbySEs.Length; i++)
            {
                if (this.standbySEs[i] == null)
                {
                    this.standbySEs[i] = se;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// SEの再生／停止を管理するクラス
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SE : MonoBehaviour
    {
        AudioSource audioSource;

        /// <summary>
        /// SE再生用オブジェクトを初期化します。
        /// </summary>
        /// <param name="parent">オブジェクトを紐づけしておく親</param>
        /// <param name="group">SEに設定するAudioMixerGroup</param>
        public void Init(GameObject parent, AudioMixerGroup group)
        {
            gameObject.transform.parent = parent.transform;

            this.audioSource = gameObject.GetComponent<AudioSource>();
            this.audioSource.outputAudioMixerGroup = group;
            this.audioSource.playOnAwake           = false;
            this.audioSource.loop                  = false;
        }

        public void Play(AudioClip clip)
        {
            this.audioSource.clip = clip;
            this.audioSource.Play();

            Invoke("EndPlaySE", clip.length);
        }

        void EndPlaySE()
        {
            SoundManager.EndPlaySE(this);
        }
    }
}
