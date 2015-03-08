using UnityEngine;
using UnityEditor;

namespace Foundation.Core.Editor
{
    [InitializeOnLoad]
    public class HierarchySortKeybindings
    {
        [MenuItem("GameObject/Selection/Sort Transform Up &UP")]
        public static void SortHierarchyUp()
        {
            SortHierarchyBy(-1);
        }

        [MenuItem("GameObject/Selection/Sort Transform Down &DOWN")]
        public static void SortHierarchyDown()
        {
            SortHierarchyBy(1);
        }

        static void SortHierarchyBy(int offset)
        {
            if (Selection.activeTransform != null)
            {
                Selection.activeTransform.SetSiblingIndex(Selection.activeTransform.GetSiblingIndex() + offset);
            }
        }
        
    }
}