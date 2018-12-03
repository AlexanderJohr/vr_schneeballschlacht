using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

    public Language currentLanguage = Language.German;

    [Header("Main Screen")]
    public Text play_Game_Text;
    public Text tutorial_Text;
    public Text option_Text;
    public Text exit_Text;

    // Update is called once per frame
    void Update()
    {
        // if(currentLanguage != SettingManager.Language){
        //currentLanguage = SettingManager.Language;
        ChanceLanguage();
        //}

	}

    private void ChanceLanguage() {
        switch (currentLanguage)
        {
            case Language.German:
                {
                    SetLanguageGerman(); 
                    break;
                }
            case Language.English:
                {
                    SetLanguageEnglish();
                    break;
                }
            default:
                Debug.LogError("Language Manager: This language not exist: "  + currentLanguage);
                break;
         }
    }

    private void SetLanguageGerman() {
        GermanLanguage.SetMainScreen(play_Game_Text, tutorial_Text, option_Text, exit_Text);

    }

    private void SetLanguageEnglish()
    {
        EnglishLanguage.SetMainScreen(play_Game_Text, tutorial_Text, option_Text, exit_Text);

    }
}
