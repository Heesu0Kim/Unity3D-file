using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;                       //UI를 컨트롤 할 것이라서  추가
using System;                               //Arrary 수전 기능을 사용 하기 위해 추가

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private SpeakerUI[] speakers;               // 대화에 참여하는 캐릭터들의 UI 배열
    [SerializeField]
    private DialogData[] dialogs;               // 현재 분기의 대사 목록 배열
    [SerializeField]
    private bool DialogInit = true;             // 자동 시작 여부
    [SerializeField]
    private bool dialogsDB = false;             // DB를 통해 읽는 것 설정

    public int currentDialogIndex = -1;         // 현재 대사 순번
    public int currentSpeakerIndex = 0;         // 현재 말을 하는 화자Speaker의 speakers
    public float typingSpeed = 0.1f;            // 텍스트 타이핑 효과의 재생 속도
    private bool isTypingEffect = false;        // 텍스트 타이핑 효과를 재생중인지

    private void Awake()
    {
        SetAllClose();
    }

    public bool UpdateDialog(int currentIndex, bool InitType)
    {
        // 대사 분기가 시작될 때 1회만 호출
        if (DialogInit == true && InitType == true)
        {
            SetAllClose();
            SetNextDialog(currentIndex);
            DialogInit = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 텍스트 타이핑 효과를 재생중일때 마우스 왼쪽 클릭하면 타이핑 효과 종료
            if (isTypingEffect == true)
            {
                isTypingEffect = false;
                StopCoroutine("OnTypingText"); // 타이팡 효과를 중지하고, 현재 대사 전체를 출력함
                speakers[currentSpeakerIndex].objectArrow.SetActive(true);
                return false;
            }

            if (dialogs[currentDialogIndex].nextindex != -100)
            {
                SetNextDialog(dialogs[currentDialogIndex].nextindex);
            }
            else
            {
                SetAllClose();
                DialogInit = true;
                return true;
            }
        }

        return false;
    }

    private void SetActiveObjects(SpeakerUI speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);

        // 화살표는 대사가 종료되었을 때만 활성화하기 때문에 항상 false
        speaker.objectArrow.SetActive(false);

        // 캐릭터 알파 값 변경
        Color color = speaker.imgCharacter.color;
        if (visible)
        {
            color.a = 1;
        }
        else
        {
            color.a = 0.2f;
        }
        speaker.imgCharacter.color = color;
    }

    private void SetAllClose()
    {
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
        }
    }

    private void SetNextDialog(int currentIndex)
    {
        SetAllClose();
        currentDialogIndex = currentIndex; //다음 대사를 진행하도록
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerUIindex; //현재 화자 순번
        SetActiveObjects(speakers[currentSpeakerIndex], true); //현재 화자의 대화 관련 오브젝트

        // 현재 화자 이름 텍스트 설정
        speakers[currentSpeakerIndex].textName.text = dialogs[currentDialogIndex].name;
        StartCoroutine("OnTypingText");
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;

        if (dialogs[currentDialogIndex].characterPath != "None")
        {
            speakers[currentSpeakerIndex].imgCharacter =
                (Image)Resources.Load(dialogs[currentDialogIndex].characterPath);
        }

        // 텍스트를 한글자씩 타이핑치듯 재생
        while (index < dialogs[currentDialogIndex].dialogue.Length + 1)
        {
            speakers[currentSpeakerIndex].textDialogue.text =
                dialogs[currentDialogIndex].dialogue.Substring(0, index);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;

        // 대사가 완료되었을 때 출력되는 커서 활성화
        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
    }
}

[System.Serializable]
public struct SpeakerUI
{
    public Image imgCharacter;      // 캐릭터 이미지
    public Image imageDialog;       // 대화창 Image UI
    public Text textName;           // 현재 대사중인 캐릭터 이름 출력 Text UI
    public Text textDialogue;       // 현재 대사 출력 Text UI
    public GameObject objectArrow;  // 대사가 완료되었을 때 출력되는 커서 오브젝트
}

[System.Serializable]
public struct DialogData
{
    public int index;                   // 대사 번호
    public int speakerUIindex;          //스피커 배열 번호
    public string name;                 // 이름
    public string dialogue;             // 대사
    public string characterPath;        // 캐릭터 이미지 저장
    public int tweenType;               // 다른 트윈 번호
    public int nextindex;               // 다음 대사
}