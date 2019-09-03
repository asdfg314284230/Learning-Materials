using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



// 编辑器扩展开发
public class TestWindow : EditorWindow {


    // 在视窗上方新建一个按钮
    [MenuItem("Tools/TestWindow")]
    private static void Open()
    {
        TestWindow test = GetWindow<TestWindow>();
        test.Show();
    }


}
