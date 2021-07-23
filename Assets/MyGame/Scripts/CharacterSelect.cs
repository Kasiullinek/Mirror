using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CharacterSelect : NetworkBehaviour
{
    [SerializeField] private Transform characterPreviewParent = default;
    [SerializeField] private TMP_Text characterNameText = default;
    public Character[] characters = default;

    private int currentCharacterIndex = 0;

    private List<GameObject> characterInstances = new List<GameObject>();

    public override void OnStartClient()
    {
        foreach (var character in characters)
        {
            GameObject characterInstance = Instantiate(character.CharacterPreviewPrefab, characterPreviewParent); 

            characterInstance.SetActive(false);


            characterInstances.Add(characterInstance);
        }

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;
    }
 
    public void Right()
    {
        if (!hasAuthority) { return; }

        characterInstances[currentCharacterIndex].SetActive(false);

        currentCharacterIndex = (currentCharacterIndex + 1) % characterInstances.Count;

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;
    }

    public void Left()
    {
        if (!hasAuthority) { return; }

        characterInstances[currentCharacterIndex].SetActive(false);

        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex += characterInstances.Count;
        }

        characterInstances[currentCharacterIndex].SetActive(true);
        characterNameText.text = characters[currentCharacterIndex].CharacterName;
    }

    public void Select()
    {
        CmdSelect(currentCharacterIndex);
    }

    [Command]
    public void CmdSelect(int characterIndex)
    {
        PlayerPrefs.SetInt("PlayerIndex",characterIndex);
        PlayerPrefs.Save();
    }

}
