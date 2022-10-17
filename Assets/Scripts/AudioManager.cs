using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SoundAudioClip<soundT>
{
    public string name;
    public soundT sound;
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    public bool loop = false;

}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private static GameObject _oneShotGameObject;
    private static AudioSource _oneShotAudioSource;
    private static SoundTrackObject _soundTrackObject;
    private List<GameObject> _spacialSounds;
    [Range(0f, 1f)]
    [SerializeField] private float _masterVolumeMultiplier;
    [Range(0f, 1f)]
    [SerializeField] private float _soundTrackVolume;
    public float MasterVolume // master volume multiplier
    {
        get
        {
            return _masterVolumeMultiplier;
        }
        set
        {
            _masterVolumeMultiplier = value;
            OnMasterVolumeChange();

        }
    }
    public float SountrackVolume // sound track volume
    {
        get
        {
            return _soundTrackVolume;
        }
        set
        {
            _soundTrackVolume = value;
            OnSoundTrackVolumeChange();

        }
    }
    public enum Sound
    {
        BombSizzle,
        BombExplode,
        Pop,
        LaserBeam,
        Shield,
        ButtonClick,
        DVDOpen,
    }
    public enum SoundTrack
    {
        ST01,
        ST02,
        ST03,
        ST03_1,
        ST04,
        ST05,
        None = -1,

    }
    public class SoundTrackObject
    {
        public GameObject gameObject;
        public AudioSource audioSource;
        public AudioLowPassFilter lowPassFilter;
        public AudioDistortionFilter distortionFilter;
        private float _distortionLevel = .3f;
        private int _cutoffFrequency = 750;
        public SoundTrackObject()
        {
            gameObject = new GameObject();
            audioSource = gameObject.AddComponent<AudioSource>();
            distortionFilter = gameObject.AddComponent<AudioDistortionFilter>();
            lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();

            distortionFilter.distortionLevel = _distortionLevel;
            lowPassFilter.cutoffFrequency = _cutoffFrequency;
        }
        public void EnableFilter()
        {
            distortionFilter.enabled = true;
            lowPassFilter.enabled = true;            
        }
        public void DisableFilter()
        {
            distortionFilter.enabled = false;
            lowPassFilter.enabled = false;
        }
    }
    public class soundTimer
    {
        public Sound soundType;
        public float lastTimePlayed;
        public float timeDelay;
        public soundTimer(Sound sound, float LastTimePlayed, float TimeDelay)
        {
            this.soundType = sound;
            this.lastTimePlayed = LastTimePlayed;
            this.timeDelay = TimeDelay;
        }
    }

    /*
    List of sound with delay add more into it if there are more sound need time delay 
    Current array size: 0 <- increase this number if there are more 
    */
    public static soundTimer[] soundTimerArray;

    public SoundAudioClip<AudioManager.Sound>[] soundAudiosClipArray;
    public SoundAudioClip<AudioManager.SoundTrack>[] soundTrackArray;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        switch (SceneManager.GetActiveScene().name)
        {
            default:
                break;
        }
        soundTimerArray = new soundTimer[0];
        _spacialSounds = new List<GameObject>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlaySoundTrack(SoundTrack.ST02);
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        // can play main theme or music here
        // Play("Theme");
        LoadAudioSetting();
    }

    // Get spacial sound object
    public GameObject GetSpacialSoundObject()
    {
        foreach (GameObject sound in _spacialSounds)
        {
            if (sound.activeInHierarchy == false)
                return sound;
        }
        return null;
    }
    public List<AudioSource> GetActiveSpacialSound()
    {
        List<AudioSource> audioSources = new List<AudioSource>();
        foreach (GameObject spacialSound in _spacialSounds)
        {
            if (spacialSound.activeInHierarchy)
            {
                audioSources.Add(spacialSound.GetComponent<AudioSource>());
            }
        }
        return audioSources;
    }
    // disable spacial sound after a certain amonut of time
    public IEnumerator DisableSoundObject(GameObject sound, float time)
    {
        yield return new WaitForSeconds(time); // this also get affected by timeScale
        if (sound)
            sound.SetActive(false);

    }
    // play spacial Sound 
    public void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = GetSpacialSoundObject();
            if (soundGameObject == null)
            {
                soundGameObject = new GameObject("Spacial Sound");
                soundGameObject.AddComponent<AudioSource>();
                soundGameObject.transform.parent = AudioManager.instance.transform;
                _spacialSounds.Add(soundGameObject);
            }
            soundGameObject.transform.position = position;
            soundGameObject.SetActive(true);
            SoundAudioClip<Sound> s = System.Array.Find(soundAudiosClipArray, Sound => Sound.sound == sound);
            soundGameObject.name = $"Spacial Sound {s.name}";
            var audioSource = soundGameObject.GetComponent<AudioSource>();
            audioSource.loop = s.loop;
            audioSource.volume = s.volume * MasterVolume;
            audioSource.pitch = s.pitch;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = 100f;
            audioSource.dopplerLevel = 0f;
            audioSource.clip = GetAudioClip(sound);
            audioSource.Play();
            if (!audioSource.loop)
                StartCoroutine(DisableSoundObject(soundGameObject, audioSource.clip.length));
        }
    }
    public GameObject PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            if (_oneShotGameObject == null)
            {
                _oneShotGameObject = new GameObject();
                _oneShotGameObject.transform.parent = instance.transform;
                _oneShotAudioSource = _oneShotGameObject.AddComponent<AudioSource>();
            }
            //GameObject soundGameObject = new GameObject("Sound");
            //AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            SoundAudioClip<Sound> s = System.Array.Find(soundAudiosClipArray, Sound => Sound.sound == sound);
            _oneShotGameObject.SetActive(true);
            _oneShotGameObject.name = "Oneshot sound " + s.name;
            _oneShotAudioSource.loop = s.loop;
            _oneShotAudioSource.volume = s.volume * MasterVolume;
            _oneShotAudioSource.pitch = s.pitch;
            _oneShotAudioSource.clip = GetAudioClip(sound);
            _oneShotAudioSource.Play();
            if (!_oneShotAudioSource.loop)
            {
                StartCoroutine(DisableSoundObject(_oneShotGameObject, _oneShotAudioSource.clip.length));
            }
            return _oneShotGameObject;
        }
        return null;

    }
    public void PlaySoundTrack(SoundTrack soundTrack)
    {
        if (_soundTrackObject == null)
        {
            _soundTrackObject = new SoundTrackObject();
            _soundTrackObject.DisableFilter();
            _soundTrackObject.gameObject.transform.parent = instance.transform;
            // DontDestroyOnLoad(_soundTrackGameObject);
        }
        SoundAudioClip<SoundTrack> s = System.Array.Find(soundTrackArray, SoundTrack => SoundTrack.sound == soundTrack);
        _soundTrackObject.audioSource.name = s.name;
        _soundTrackObject.audioSource.loop = s.loop;
        _soundTrackObject.audioSource.volume = SountrackVolume * MasterVolume;
        _soundTrackObject.audioSource.pitch = s.pitch;
        // soundTrackAudioSource.PlayOneShot(GetAudioClip(soundTrack));
        _soundTrackObject.audioSource.clip = GetAudioClip(soundTrack);
        _soundTrackObject.audioSource.Play();
    }

    public void PauseAllSound()
    {
        if (_soundTrackObject != null)
        {
            _soundTrackObject.EnableFilter();
        }
        if (_oneShotGameObject)
            _oneShotGameObject.GetComponent<AudioSource>().Pause();
        foreach (var spacialSound in GetActiveSpacialSound())
        {
            spacialSound.Pause();
        }
    }
    public void ResumeAllSound()
    {
        if (_soundTrackObject != null)
        {
            _soundTrackObject.DisableFilter();
        }
        if (_oneShotGameObject)
            _oneShotGameObject.GetComponent<AudioSource>().Play();
        foreach (var spacialSound in GetActiveSpacialSound())
        {
            spacialSound.Play();
        }
    }
    public static AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAudioClip<Sound> soundAudioClip in instance.soundAudiosClipArray)
        {
            if (soundAudioClip.sound == sound)
                return soundAudioClip.audioClip;
        }
        Debug.LogErrorFormat("Sound" + sound + "not found!");
        return null;
    }
    public static AudioClip GetAudioClip(SoundTrack sound)
    {
        foreach (SoundAudioClip<SoundTrack> soundTrack in instance.soundTrackArray)
        {
            if (soundTrack.sound == sound)
                return soundTrack.audioClip;
        }
        Debug.LogErrorFormat("Sound Track: " + sound + "not found!");
        return null;
    }


    private static bool CanPlaySound(Sound sound)
    {
        // for sound like foot step and such
        soundTimer soundT = System.Array.Find(soundTimerArray, soundtimer => soundtimer.soundType == sound);
        if (soundT != null)
        {
            if (soundT.lastTimePlayed + soundT.timeDelay < Time.time)
            {
                soundT.lastTimePlayed = Time.time;
                return true;
            }
            else return false;
        }
        return true;
    }

    public SoundTrackObject GetSoundTrackGameObject()
    {
        return _soundTrackObject;
    }

    public IEnumerator FadeOutST(float fadeDuration = 0f, float targetVolumne = 0, SoundTrack NextST = SoundTrack.None)
    {
        float currentTime = 0;
        float start = _soundTrackObject.audioSource.volume;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            _soundTrackObject.audioSource.volume = Mathf.Lerp(start, targetVolumne, currentTime / fadeDuration);
            yield return null;
        }
        PlaySoundTrack(NextST);
        yield break;
    }

    public virtual void OnMasterVolumeChange()
    {
        if (_soundTrackObject != null)
        {
            _soundTrackObject.audioSource.volume = SountrackVolume * MasterVolume; // redundant 
        }
    }
    public virtual void OnSoundTrackVolumeChange()
    {
        if (_soundTrackObject != null)
        {
            _soundTrackObject.audioSource.volume = SountrackVolume * MasterVolume; // redundant
        }
    }
    public void LoadAudioSetting()
    {
        if (GameManager.instance.settingData != null)
        {
            MasterVolume = GameManager.instance.settingData.masterVolume;
            SountrackVolume = GameManager.instance.settingData.musicVolume;
        }
    }
    public void RemoveSoundTrackFilter()
    {
        _soundTrackObject.DisableFilter();
    }
}
