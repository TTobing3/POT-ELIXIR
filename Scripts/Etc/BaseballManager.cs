
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseballManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI textMeshPro;

    public List<string> inputs;
    public List<string> answer;

    public string colors = "빨주노초파남보";

    void Awake()
    {
        inputField.onEndEdit.AddListener(Input);
    }

    void Start()
    {
        StartGame();
    }

    public void Input(string s)
    {
        AddLog($"[{System.DateTime.Now.Hour}:{System.DateTime.Now.Minute}:{System.DateTime.Now.Second}] {s}");
        inputField.text = "";
        inputField.ActivateInputField();

        if(Check(s)) return;

        inputs.Add(s);
    }

    bool Check(string s)
    {

        if(s == "게임 시작")
        {
            StartGame();
            return true;
        }

        if(answer.Count == s.Length)
        {
            AddLog("확인해보겠습니다!");

            if(string.Join("",answer) == s)
            {
                AddLog("정답!");
                AddLog("축하드려요!");
                return true;
            }

            for(int i = 0; i<s.Length; i++)
            {
                if(s[i].ToString() == answer[i])
                {
                    AddLog(s[i]+" 일치!");
                }
                else if(answer.Contains(s[i].ToString()))
                {
                    AddLog(s[i]+" 포함!");
                }
                else
                {
                    AddLog(s[i]+" 없음!");
                }
            }

            return true;
        }
        else
        {
            AddLog("각 항목을 색 한 글자로 입력해주세요!");
            AddLog("가능한 색은 다음과 같습니다.");
            AddLog($"[{colors}]");
            return false;
        }
    }

    public void AddLog(string s)
    {
        textMeshPro.text += s + "\n";
    }

    //

    public void StartGame()
    {
        AddLog("<포션 베이스볼 시작>");
        AddLog("무작위 공식 결정!");

        int count = Random.Range(1, 7);

        answer.Clear(); // Clear previous answers

        for(int i = 0; i<count; i++)
        {
            string color;
            do
            {
                color = colors[Random.Range(0, colors.Length)].ToString();
            } while (answer.Contains(color)); // Ensure no duplicate colors

            answer.Add(color);
        }

        AddLog(count+"칸!");
    }
}
