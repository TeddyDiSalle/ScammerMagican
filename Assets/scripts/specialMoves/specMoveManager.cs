using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class specialMove
{
    public string name;
    public int chance;//percent out of 100
    public int level;//when can it start doing this
}
public class specMoveManager : MonoBehaviour
{
    public specialMove [] moves;
    public LowerCup lowerCup;
    public rollToOther roller;
    public fakeBall faker;
    public lvlProgress progression;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float shuffleOver()
    {
        return callSpecialMove();//return duration
    }

    public float callSpecialMove()
    {
        foreach (specialMove move in moves){
            if (progression.level>=move.level&&//high level
            Random.Range(0,101)<=move.chance)//random chance works
            {//do this move
                if (move.name=="roll")
                    return roller.doRoll(lowerCup.cups);
                else if (move.name=="fake")
                    return faker.makeBall();
                

                //return;//do no other moves (for now)
            }
        }

        return 0f;
    }
}
