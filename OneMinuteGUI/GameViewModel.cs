using System.Collections;
using Foundation.Databinding.Model;
using Foundation.Databinding.View;
using UnityEngine;

public enum DifficultyEnum
{
    Easy,
    Medium,
    Hard
}

public enum StarsEnum
{
    None,
    One,
    Two,
    Three
}

public class MainViewModel : ObservableBehaviour
{

    public int[] StarScores = {100,250,600};

    public string Leaderboard = "CgkIzci20KsaEAIQAA";

    private bool _sound;
    public bool EnableSound
    {
        get { return _sound; }
        set
        {
            _sound = value;
            NotifyProperty("EnableSound", value);
            PlayerPrefs.SetInt("EnableSound", value ? 1 : 0);
            AudioManager.SetVolume(AudioLayer.Sfx, value ? 1 : 0);
            AudioManager.SetVolume(AudioLayer.UISfx, value ? 1 : 0);
        }
    }

    private bool _music;
    public bool EnableMusic
    {
        get { return _music; }
        set
        {
            _music = value;
            NotifyProperty("EnableMusic", value);
            PlayerPrefs.SetInt("EnableMusic", value ? 1 : 0);
            AudioManager.SetVolume(AudioLayer.Music, value ? 1 : 0);
        }
    }

    private StarsEnum _stars;
    public StarsEnum Stars
    {
        get { return _stars; }
        set
        {
            if (_stars == value)
                return;
            _stars = value;
            NotifyProperty("Stars", value);
            NotifyProperty("OneStar", OneStar);
            NotifyProperty("TwoStar", TwoStar);
            NotifyProperty("ThreeeStar", ThreeeStar);
        }
    }

    public bool OneStar
    {
        get { return Stars >= StarsEnum.One; }
    }
    public bool TwoStar
    {
        get { return Stars >= StarsEnum.Two; }
    }
    public bool ThreeeStar
    {
        get { return Stars >= StarsEnum.Three; }
    }
    
    private int _score;
    public int Score
    {
        get { return _score; }
        set
        {
            if (_score == value)
                return;
            _score = value;
            NotifyProperty("Score", value);
        }
    }

    private bool _newHighScore;
    public bool NewHighScore
    {
        get { return _newHighScore; }
        set
        {
            if (_newHighScore == value)
                return;
            _newHighScore = value;
            NotifyProperty("NewHighScore", value);
        }
    }

    private bool _canQuit;
    public bool CanQuit
    {
        get { return _canQuit; }
        set
        {
            if (_canQuit == value)
                return;
            _canQuit = value;
            NotifyProperty("CanQuit", value);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        EnableSound = PlayerPrefs.GetInt("EnableSound", 1) == 1;
        EnableMusic = PlayerPrefs.GetInt("EnableMusic", 1) == 1;
        CanQuit = Application.platform != RuntimePlatform.IPhonePlayer;

    }

    public IEnumerator QuitApplication()
    {
        yield return 1;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_iOS
#else
        Application.Quit();
#endif

    }

    #region social

    void OnAuth(bool success)
    {
        Debug.Log("Welcome User");
    }

    public IEnumerator ShareTwitter()
    {
        yield return 1;
    }

    public IEnumerator ShareFacebook()
    {
        yield return 1;
    }

    public IEnumerator ShareGoogle()
    {
        yield return 1;
    }

    public IEnumerator ShowHighScore()
    {
        yield return 1;
        Social.ShowLeaderboardUI();
    }

    public IEnumerator OpenGameWeb()
    {
        yield return 1;
        Application.OpenURL("http://AvariceOnline.com");
    }

    public IEnumerator OpenCompanyWeb()
    {
        yield return 1;
        Application.OpenURL("http://AvariceOnline.com");
    }

    public IEnumerator OpenPersonalWeb()
    {
        yield return 1;
        Application.OpenURL("http://NicholasVentimiglia.com");
    }
    #endregion

    #region gameplay
    public void StartGame(DifficultyEnum arg)
    {
        MenuManager.Instance.Open(MenuViewEnum.GamePlay);
    }

    public void EndGame(int score)
    {
        Score = score;
        var highScore = PlayerPrefs.GetInt("HighScore", 0);

        NewHighScore = Score > highScore;

        if(NewHighScore)
        {
            PlayerPrefs.SetInt("HighScore", Score);
            PlayerPrefs.Save();
        }

        if (score >= StarScores[2])
            Stars = StarsEnum.Three;
        else if (score >= StarScores[1])
            Stars = StarsEnum.Two;
        else if (score >= StarScores[0])
            Stars = StarsEnum.One;
        else 
            Stars = StarsEnum.None;

        if (Social.localUser.authenticated)
        {
            Social.ReportScore(Score, Leaderboard, OnReportScore);
        }

        MenuManager.Instance.Open(MenuViewEnum.GameScore);
    }

    void OnReportScore(bool arg)
    {
        Debug.Log("Score Reported :" + arg);
    }

    public void EndGameOver()
    {
        MenuManager.Instance.Open(MenuViewEnum.MainMenu);
    }

    public void RestartGame()
    {
        MenuManager.Instance.Open(MenuViewEnum.GamePlay);
    }
    #endregion
}
