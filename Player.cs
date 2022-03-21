using System.Collections.Generic;
using System;
using System.Linq;


// Class to store Players data and their cards
class Player{
    private static int playerIDcounter=0; //static field to get unique ID for player
    private int playerID=0; //Player ID
    List<string> cards = new List<string>(); //List to store player cards
    List<int> value = new List<int>();    //List to store players cards value
    private int tokens; //int to store player tokens
    private bool isActive=true; //bool to store player status in round, false for eliminated

    private bool isProtected=false; // bool to store player status of being protected by handmaid

    private int totalValue=0; //to store player cards value for the round

    //dictionary to store each card rule
    private Dictionary<string,string> cardRules = new Dictionary<string,string>()
                    {
                        {"GUARD","Must choose one of the other players,guess the chosen players's card.If correct they are out of round,otherwise no effect"},
                        {"PRIEST","Must choose another player, look at another player card"}, 
                        {"BARON","Must choose other Player,lower value card player eliminated,same cards- no effect"},
                        {"HANDMAID","Protect you from being targeted ,until your next turn"},
                        {"PRINCE","Must choose any player(including yourself),chosen player must discard the card and draw another."},
                        {"KING","Must choose another player, chosen player must swap the cards with you."},
                        {"COUNTESS","If you have the countess in your hand with either King or Prince,then you must discard the Countess"},
                        {"PRINCESS","You may not discard the card, if discarded you are eliminated from the round"},            
                    };
    //Constructors and properties

    public Player(){
        playerIDcounter++;
        playerID=playerIDcounter;
        tokens=0;
    }
    public Player(string cardName,int v){
        cards.Add(cardName);
        value.Add(v);
        tokens=0;
        playerIDcounter++;
        playerID=playerIDcounter;
    }

    public List<string> Cards{
        get { return cards; }   // get method
        set { cards = value; }  // set method
    } 

    public bool IsActive{
        get { return isActive; }   // get method
        set { isActive = value; }  // set method
    } 
    public bool IsProtected{
        get { return isProtected; }   // get method
        set { isProtected = value; }  // set method
    } 
    public int Tokens{
        get { return tokens; }  
        set { tokens = value; }
    }
    public int PlayerId{
        get { return playerID; }  
        set { playerID = value; }
    }

    public int TotalValue{
        get { return totalValue; }  
        set { totalValue = value; }
    }

    //function to add token to player
    public void addToken(){
        tokens++;
    }

    //Function to check wether card exist for player or not
    public  bool isCardExist(string card){
        bool result=false;
        foreach(string i in cards){
            if(string.Equals(i,card)){
                result=true;
            }
        }
        return result;

    }

    //function to add card to player cards
    public void addCard(string cardName,int v){
        if(!String.Equals(cardName,"")){
            cards.Add(cardName);
            value.Add(v);
            totalValue=totalValue+v;
        }
        
    }

    //function to get player hand card
    public string getCard(){
        return cards[0];
    }

    //function to remove card from player
    public void removeCard(string cardName){
        value.RemoveAt(cards.IndexOf(cardName));
        cards.Remove(cardName);
    }

    //function to get total hand value of player

    public int getTotalValue(){
        int sum=0;
        foreach(var i in value){
            sum=sum+i;
        }
        return sum;
    }
    
    //function to print player hand cards
    public void printCards(){
        for(int i=0;i<cards.Count;i++){
             Console.WriteLine("Card : {0}, Value: {1}, Card Rule: {2}", cards[i],value[i],cardRules[cards[i]]);
        }
    }

    //function to return card value by card name
    public int getCardValue(string card){
        
        return value[cards.IndexOf(card)];
    }

    //function to return second card of player except the card passed
    public string getCardExcept(string card){
        string temp="";
        for(int i=0;i<cards.Count;i++){
            if(!String.Equals(card,cards[i])){
                temp=cards[i];
            }
        }
        return temp;
    }


    //function to clear card from player
    public void clearCards(){
        cards.Clear();
        value.Clear();
    }

}