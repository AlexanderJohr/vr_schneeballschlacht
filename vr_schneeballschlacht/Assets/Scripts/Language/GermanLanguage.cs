using UnityEngine.UI;

public class GermanLanguage {

    public static void SetMainScreen(Text playButton, Text tutorialButton, Text optionButton, Text exitButton) {
        playButton.text = "Spielen";
        tutorialButton.text = "Tutorial";
        optionButton.text = "Optionen";
        exitButton.text = "Spiel beenden";
    }
}
