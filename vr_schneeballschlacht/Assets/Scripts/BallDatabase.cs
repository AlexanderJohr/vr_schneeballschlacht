using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VR Schneeballschlacht/Ball Database")]
public class BallDatabase : ScriptableObject
{
    public int SnowBallIndex { get; set; }

    public float Health;

    //public float Score = 0;

    //public bool GameOver { get; set; }

    //public float RemainingSeconds { get; set; }

    public Snowball2 PlayersLeftBall { get; set; }
    public Snowball2 PlayersRightBall { get; set; }

    public Snowball2 OpponentLeftBall { get; set; }
    public Snowball2 OpponentRightBall { get; set; }

    public Dictionary<int, Snowball2> PlayersBalls = new Dictionary<int, Snowball2>();
    public Dictionary<int, Snowball2> OpponentsBalls = new Dictionary<int, Snowball2>();

    public void Reset() {
        PlayersLeftBall = null;
        PlayersRightBall = null;

        OpponentLeftBall = null;
        OpponentRightBall = null;

        PlayersBalls = new Dictionary<int, Snowball2>();
        OpponentsBalls = new Dictionary<int, Snowball2>();

        Health = 1f;

        SnowBallIndex = 0;
    }

}