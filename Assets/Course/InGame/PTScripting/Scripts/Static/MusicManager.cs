using UnityEngine;

namespace Course.PrototypeScripting
{
    public class MusicManager : MonoBehaviour
    {
        public AudioSource source1;
        public AudioSource source2;

        public static MusicManager _instance;
        public static MusicManager Instance
        {
            get
            {
                if (_instance == null)
                    throw new System.Exception("No MusicManager in the scene. Import the prefab SimpleGameEngine from the project folder.");
                return _instance;
            }
            private set { _instance = value; }
        }
        enum State { None, Play1, Play2, SwitchTo1, SwitchTo2 }
        State state;

        bool active;
        void Awake()
        {
            if (_instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (!active)
                return;
        
            timer += Time.deltaTime;
            float percent = (timer / switchTime);
            switch (state)
            {
                case State.SwitchTo1:  
                    source1.volume = percent * targetVolume;
                    if(source2.isPlaying)
                        source2.volume = (1 - percent) * startVolumeOther;
                    break;
                case State.SwitchTo2:
                    source2.volume = percent * targetVolume;
                    if (source1.isPlaying)
                        source1.volume = (1 - percent) * startVolumeOther;
                    break;
            }

            if(timer >= switchTime)
            {
                EndSwitch();
            }
        }

        void EndSwitch()
        {
            active = false;
            switch (state)
            {
                case State.SwitchTo1:
                    source1.volume = targetVolume;
                    source2.volume = 0;
                    source2.Stop();
                    state = State.Play1;
                    break;
                case State.SwitchTo2:
                    source2.volume = targetVolume;
                    source1.volume = 0;
                    source1.Stop();
                    state = State.Play2;
                    break;
            }
        }
        float switchTime;
        float timer = 0;
        float targetVolume;
        float startVolumeOther;
        public void SwitchMusic(AudioClip clip, float time, float _targetVolume)
        {
            switchTime = time;
            timer = 0;
            targetVolume = _targetVolume;
        
            switch (state)
            {
                case State.None:
                case State.Play2:
                case State.SwitchTo2:
                    source1.loop = true;
                    source1.clip = clip;
                    source1.volume = 0;
                    source1.Play();
                    startVolumeOther = source2.volume;
                    state = State.SwitchTo1;
                    break;
                case State.Play1:
                case State.SwitchTo1:
                    source2.loop = true;
                    source2.clip = clip;
                    source2.volume = 0;
                    source2.Play();
                    startVolumeOther = source1.volume;
                    state = State.SwitchTo2;
                    break;
            }
            active = true;
       
        }
    }
}
