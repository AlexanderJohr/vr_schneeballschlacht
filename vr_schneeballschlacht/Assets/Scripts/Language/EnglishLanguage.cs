using UnityEngine.UI;

public class EnglishLanguage
{

    public static void SetMainScreen(Text playButton, Text tutorialButton, Text optionButton, Text exitButton)
    {
        playButton.text = "Play";
        tutorialButton.text = "Tutorial";
        optionButton.text = "Options";
        exitButton.text = "Quit Game";
    }
}