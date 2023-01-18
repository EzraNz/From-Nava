using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public int playerHealth;
    public Text healthText;
    public string patternActive = "";
    public string prevNode = "";
    //private List<string> nodesList = new List<string>();
    private List<LineRenderer> nodesList = new List<LineRenderer>();
    public int currLines;
    public Spell _spell;
    public Transform castPoint;
    public IceTowerAI ita;

    [SerializeField] private float maxMagic = 100f;
    [SerializeField] private float currMagic;
    [SerializeField] private float magicRefillRate = 2f;
    [SerializeField] private float refillRateDelay = 1f; //1 second
    private float currMagicRefillTimer;
    [SerializeField] private float magicCastInterval = 0.25f;
    //[SerializeField] private Transform castPoint;

    [SerializeField] private float currCastTimer;

    private bool castingMagic = false;


    public string facingDir;
    public PlayerController pc;
    public ManaMeter mm;

    void Awake() {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        currMagic = maxMagic;
        mm.SetMaxMana(maxMagic);
    }

    [ContextMenu("Increase Health")]
    public void addHealth() {
        playerHealth += 1;
        healthText.text = "Health: " + playerHealth.ToString();
    }
/*
    public void setString(string newNode, LineRenderer lineIn) {
        patternActive += newNode;
        Debug.Log(patternActive);
        prevNode = newNode;
        nodesList.Add(lineIn);
        if (newNode == "9") {
            CastSpell();
            patternActive = "";
            prevNode = "";
        }
    }
    */

    public void setString(string newNode) {
        patternActive += newNode;
        if (newNode == "9") {
            if (patternActive == "59") {
                bool hasReqMagic = ((currMagic - _spell.SpellCast.magicCost) >= 0f);
                if (!castingMagic && hasReqMagic) {
                    castingMagic = true;
                    currMagic -= _spell.SpellCast.magicCost;
                    currCastTimer = 0;
                    currMagicRefillTimer = 0;
                    CastSpell();
                }    
            }
            if (patternActive == "269") {
                //trigger death if on trigger of tower ice
                ita.disableTower = true; 
            }
            //CastSpell();
            patternActive = "";
            prevNode = "";
        }
    }

    private void Update() {
        mm.SetManaMeter(currMagic);
        if (castingMagic) {
                currCastTimer += Time.deltaTime;
                if (currCastTimer > magicCastInterval) {
                    castingMagic = false;
                }
            }

            if (currMagic < maxMagic && (!castingMagic)) {
                currMagicRefillTimer += Time.deltaTime;
                if (currMagicRefillTimer > refillRateDelay) {
                    currMagic += magicRefillRate * Time.deltaTime;            
                    if (currMagic > maxMagic) {
                        currMagic = maxMagic;
                    }
                }
            }   
    }

    public string getPrevNode() {
        if (prevNode == "") {
            Debug.Log("New Pattern Started");
            return "1";
        }
        Debug.Log(prevNode + " - Continuing Pattern");
        return prevNode;
    }

    public List<LineRenderer> getList() {
        return nodesList;
    }

    public int getCurrLines() {
        return currLines;
    }

    public void setCurrLines(int amt) {
        currLines-= amt;
    }

    public string getCurrString() {
        return patternActive;
    }

    public void increaseLines(int amt) {
        currLines+= amt;
    }
    
    public void destroyLine(int num) {
        Destroy(nodesList[num].gameObject);
        nodesList.RemoveAt(num);
    }

    public void CastSpell() {
        facingDir = pc.facingDir;
        Instantiate(_spell, castPoint.position, Quaternion.identity);
    }
}
