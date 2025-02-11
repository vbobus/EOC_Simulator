using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    private Button startButton;

    void Start()
    {
        // 获取根 VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 找到按钮并绑定点击事件
        startButton = root.Q<Button>("Accept");
        startButton.clicked += StartGame;
    }

    private void StartGame()
    {
        // 切换到游戏主场景
        SceneManager.LoadScene("GreyBox_V1");
    }
}