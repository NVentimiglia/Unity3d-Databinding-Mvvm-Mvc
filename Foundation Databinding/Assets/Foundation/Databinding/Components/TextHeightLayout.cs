// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Foundation.Databinding.Components
{
    /// <summary>
    /// Usefull for dynamic text. Sets the LayoutElement height based on the text's height.
    /// </summary>
    [AddComponentMenu("Foundation/uGUI/TextHeightLayout")]
    public class TextHeightLayout : MonoBehaviour
    {
        public LayoutElement Element;
        public Text Text;
        public int BaseHeight;
        public bool DebugMode;

        void Awake()
        {
            Text.RegisterDirtyLayoutCallback(Recalculate);
        }

        IEnumerator Start()
        {
            yield return 1;
            Recalculate();
        }

        [ContextMenu("Recalculate")]
        void Recalculate()
        {
            if (DebugMode)
            {
                Element.preferredHeight = BaseHeight + Text.preferredHeight;
                Debug.Log(BaseHeight+" "+Text.preferredHeight);
                
            }
            else
            {
                Element.preferredHeight = BaseHeight + Text.preferredHeight;
                
            }
        }
    }
}