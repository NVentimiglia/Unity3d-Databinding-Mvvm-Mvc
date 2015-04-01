// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System.ComponentModel;
using UnityEngine;

namespace Foundation.Databinding.Example
{
    /// <summary>
    /// Example main menu
    /// </summary>
    [AddComponentMenu("Foundation/Databinding/Example/ExampleNotifyChanged")]
    public class ExampleNotifyChanged : MonoBehaviour, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;



        [SerializeField]
        private string _time;
        public string LevelTime
        {
            get { return _time; }
            set
            {
                if (_time == value)
                    return;
                _time = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LevelTime"));
                }
            }
        }

        

        void Update()
        {
            LevelTime = Time.timeSinceLevelLoad.ToString();
        }
    }
}
