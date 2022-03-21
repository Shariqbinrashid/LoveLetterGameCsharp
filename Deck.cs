using System.Collections.Generic;
using System;
using System.Linq;


// CLass to store state of Deck and get card for draw
class Deck{
    //dictionary to store the cards and their count
    private Dictionary<string,int> deck = new Dictionary<string,int>()
                    {
                        {"GUARD",5},
                        {"PRIEST",2}, 
                        {"BARON",2},
                        {"HANDMAID",2},
                        {"PRINCE",2},
                        {"KING",1},
                        {"COUNTESS",1},
                        {"PRINCESS",1},            
                    };

    public Dictionary<string,int> Cards{
        get { return deck; }   // get method
        set { deck = value; }  // set method
    } 

    //Function to return random card from deck and minus its count
    public string getRandomCard(){
        Random rnd=new Random();
        int i;
        if(!isEmpty()){
            while(true){
                i=rnd.Next(0,8);
                if(deck.ElementAt(i).Value==0){ //IF CARD COUNT IS 0, continue the loop to get random index
                    continue;
                }
                else{
                    deck[deck.ElementAt(i).Key]=deck.ElementAt(i).Value-1; //decreasing the card count
                    break;
                    
                }
            
            }
            return deck.ElementAt(i).Key;
        }
        return "";
        
    }


    //function to get index of card
    public int getIndex(string card){
        
        for (int i = 0; i < deck.Count; i++){
            if(String.Equals(deck.ElementAt(i).Key,card)){
                return i;
            }
        }
        return -1;
    }
    //function to print deck
    public void printDeck(){
        foreach(var i in deck){
            Console.WriteLine("Key: {0}, No of Cards: {1}", i.Key, i.Value);
        }
    }

    //function to check wether deck is empty or not
    public Boolean isEmpty(){
        int sum=0;
        foreach(var i in deck){
            sum=sum+i.Value;
        }

        return sum==0? true:false;
    }
}