using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    public GameObject cloneButton;
    public bool messageOn;
    private List<GameObject> messageList;
    private List<float> generateTimeList;
    

    private void Start()
    {
        messageOn = false;
        messageList = new List<GameObject>();
        generateTimeList = new List<float>();
    }

    private void Update()
    {
        //// 여기서 이제 생기는 조건만 만들면됨. 기능은 다 구현
        //if(Input.GetMouseButtonDown(0))
        //{
        //    // 각 index의 포지션값을 변화 시켜야겠다.
        //    GenerateMessage("sdfsdf");
        //}
        if (messageOn)
        {
            if (messageList.Count > 0)
            {
                List<int> removeList = new List<int>();
                for (int i = 0; i < messageList.Count; i++)
                    if (MessageFadeOut(i))
                        removeList.Add(i);

                for (int i = removeList.Count - 1; i >= 0; i--)
                {
                    Destroy(messageList[removeList[i]]);
                    messageList.RemoveAt(removeList[i]);
                    generateTimeList.RemoveAt(removeList[i]);
                }
            }
            else
            {
                messageOn = false;
            }
        }
    }

    private bool MessageFadeOut(int messageIndex)
    {
        float timeInterval = Time.time - generateTimeList[messageIndex];
        ColorBlock newColor = messageList[messageIndex].GetComponent<Button>().colors;
        Color newText = messageList[messageIndex].GetComponentInChildren<Text>().color;
        //이렇게만해도 전달되나? 주소값이 전해지는거 아닌가?

        if (timeInterval >= 3)
            return true;
        else
        {
            Color newNormal = newColor.normalColor;
            Color newTextColor = newText;
            newNormal.a = (1-timeInterval / 3);
            newTextColor.a = (1 - timeInterval / 3);
            newColor.normalColor = newNormal;
            newText = newTextColor;
            messageList[messageIndex].GetComponent<Button>().colors = newColor;
            messageList[messageIndex].transform.localPosition = new Vector3(messageList[messageIndex].transform.localPosition.x, (-66 - 25 * messageIndex), messageList[messageIndex].transform.localPosition.z);
            return false;
        }
    }

    public void GenerateMessage(string errorMessage)
    {
        messageOn = true;
        // 5칸이 지나면 마지막거 지우기.
        if (messageList.Count < 5)
        {
            GameObject newChat = Instantiate(cloneButton, GameObject.Find("ButtonManager").transform);
            newChat.SetActive(true);
            newChat.GetComponentInChildren<Text>().text = errorMessage;
            messageList.Add(newChat);
            generateTimeList.Add(Time.time);
        }
        else
        {
            Destroy(messageList[0]);
            messageList.RemoveAt(0);
            generateTimeList.RemoveAt(0);
            GameObject newChat = Instantiate(cloneButton, GameObject.Find("ButtonManager").transform);
            newChat.SetActive(true);
            newChat.GetComponentInChildren<Text>().text = errorMessage;
            messageList.Add(newChat);
            generateTimeList.Add(Time.time);
        }
    }

}
