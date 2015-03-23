// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Databinding.Components;
using Foundation.Databinding.Model;
using UnityEngine;

namespace Foundation.Databinding.Example
{
    /// <summary>
    /// Demonstration of a child view model
    /// </summary>
    public class ExampleScore : ObservableObject
    {
        [SerializeField]
        public string Username;
        
        [SerializeField]
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

        [SerializeField]
        private bool _isSelf;
        public bool IsSelf
        {
            get { return _isSelf; }
            set
            {
                if (_isSelf == value)
                    return;
                _isSelf = value;
                NotifyProperty("IsSelf", value);
            }
        }
    }

    /// <summary>
    /// Example High Score Menu
    /// </summary>
    [AddComponentMenu("Foundation/Databinding/Example/ExampleList")]
    public class ExampleList : ObservableBehaviour
    {
        /// <summary>
        /// Protip : Use IOC
        /// </summary>
        public ExampleOptions Options;

        #region view logic
        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                NotifyProperty("IsVisible", value);
                enabled = value;
            }
        }

        public void Close()
        {
            IsVisible = false;
        }
        #endregion
        
        #region model

        public string[] DemoNames = {
            "Beto",
            "Mac",
            "Velly",
            "Psylon",
            "Yoda",
            "Quark",
            "Torak",
            "Azreal",
            "Ishtar",
            "Itty",

        };

        private ExampleScore _myScore;
        public ExampleScore MyScore
        {
            get { return _myScore; }
            set
            {
                if (_myScore == value)
                    return;
                _myScore = value;
                NotifyProperty("MyScore", value);
            }
        }

        public ObservableCollection<ExampleScore> HighScores = new ObservableCollection<ExampleScore>(); 

        #endregion

        void OnEnable()
        {
            MyScore = new ExampleScore
            {
                IsSelf = true,
                Score = UnityEngine.Random.Range(100, 1000),
                Username = Options.UserName
            };

            HighScores.Add(MyScore);

            StartCoroutine(NewScoreAsync());
        }

        void OnDisable()
        {
            HighScores.Clear();
            StopCoroutine(NewScoreAsync());
        }

        IEnumerator NewScoreAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                if(!enabled)
                    yield break;

                var score = new ExampleScore
                {
                    IsSelf = false,
                    Score = UnityEngine.Random.Range(100, 1000),
                    Username = Random(DemoNames),
                };

                HighScores.Add(score);


                yield return new WaitForSeconds(1);
            }
        }

        T Random<T>(IEnumerable<T> list)
        {
            var count = list.Count();

            if (count == 0)
                return default(T);

            return list.ElementAt(UnityEngine.Random.Range(0, count));
        }
    }
}
