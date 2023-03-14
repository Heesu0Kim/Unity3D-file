using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;                       //UI�� ��Ʈ�� �� ���̶�  �߰�
using System;                               //Arrary ���� ����� ��� �ϱ� ���� �߰�

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private SpeakerUI[] speakers;               // ��ȭ�� �����ϴ� ĳ���͵��� UI �迭
    [SerializeField]
    private DialogData[] dialogs;               // ���� �б��� ��� ��� �迭
    [SerializeField]
    private bool DialogInit = true;             // �ڵ� ���� ����
    [SerializeField]
    private bool dialogsDB = false;             // DB�� ���� �д� �� ����

    public int currentDialogIndex = -1;         // ���� ��� ����
    public int currentSpeakerIndex = 0;         // ���� ���� �ϴ� ȭ��Speaker�� speakers
    public float typingSpeed = 0.1f;            // �ؽ�Ʈ Ÿ���� ȿ���� ��� �ӵ�
    private bool isTypingEffect = false;        // �ؽ�Ʈ Ÿ���� ȿ���� ���������

    private void Awake()
    {
        SetAllClose();
    }

    public bool UpdateDialog(int currentIndex, bool InitType)
    {
        // ��� �бⰡ ���۵� �� 1ȸ�� ȣ��
        if (DialogInit == true && InitType == true)
        {
            SetAllClose();
            SetNextDialog(currentIndex);
            DialogInit = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // �ؽ�Ʈ Ÿ���� ȿ���� ������϶� ���콺 ���� Ŭ���ϸ� Ÿ���� ȿ�� ����
            if (isTypingEffect == true)
            {
                isTypingEffect = false;
                StopCoroutine("OnTypingText"); // Ÿ���� ȿ���� �����ϰ�, ���� ��� ��ü�� �����
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

        // ȭ��ǥ�� ��簡 ����Ǿ��� ���� Ȱ��ȭ�ϱ� ������ �׻� false
        speaker.objectArrow.SetActive(false);

        // ĳ���� ���� �� ����
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
        currentDialogIndex = currentIndex; //���� ��縦 �����ϵ���
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerUIindex; //���� ȭ�� ����
        SetActiveObjects(speakers[currentSpeakerIndex], true); //���� ȭ���� ��ȭ ���� ������Ʈ

        // ���� ȭ�� �̸� �ؽ�Ʈ ����
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

        // �ؽ�Ʈ�� �ѱ��ھ� Ÿ����ġ�� ���
        while (index < dialogs[currentDialogIndex].dialogue.Length + 1)
        {
            speakers[currentSpeakerIndex].textDialogue.text =
                dialogs[currentDialogIndex].dialogue.Substring(0, index);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;

        // ��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� Ȱ��ȭ
        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
    }
}

[System.Serializable]
public struct SpeakerUI
{
    public Image imgCharacter;      // ĳ���� �̹���
    public Image imageDialog;       // ��ȭâ Image UI
    public Text textName;           // ���� ������� ĳ���� �̸� ��� Text UI
    public Text textDialogue;       // ���� ��� ��� Text UI
    public GameObject objectArrow;  // ��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� ������Ʈ
}

[System.Serializable]
public struct DialogData
{
    public int index;                   // ��� ��ȣ
    public int speakerUIindex;          //����Ŀ �迭 ��ȣ
    public string name;                 // �̸�
    public string dialogue;             // ���
    public string characterPath;        // ĳ���� �̹��� ����
    public int tweenType;               // �ٸ� Ʈ�� ��ȣ
    public int nextindex;               // ���� ���
}