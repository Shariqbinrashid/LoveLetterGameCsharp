using System.Collections.Generic;
using System;
using System.IO;

namespace lovebird
{
    class Program
    {
        static List<Player> players = new List<Player>();  //static global list for players,so its accessible for all the methods in the class
        static int round=0;  //int to store round count
        static void Main(string[] args)
        {   
            //For ecc key hit check
            ConsoleKeyInfo ch;
 
            Console.WriteLine("Welcome to love letter Game\n");

            int noOfPlayers;
            int sum=0;
            //If user dont want to resume the game
            if(!resumeGamePlay()){
                Console.WriteLine("Enter number of Players");
                noOfPlayers=Convert.ToInt32(Console.ReadLine());
                for(int i=0;i<noOfPlayers;i++){
                    Player plr=new Player();
                    players.Add(plr); //adding each player to players list
                }
            }
            //if user want to resume the game
            else{
                string fileName = @"record.txt";
                
                using (StreamReader streamReader = File.OpenText(fileName))
                {
                    string text = streamReader.ReadToEnd();
                    string[] lines = text.Split(Environment.NewLine);
                    noOfPlayers=Int32.Parse(lines[0]);
                    for(int i=0;i<noOfPlayers;i++){
                        Player plr=new Player();
                        players.Add(plr); //adding each player to players list
                    }
                
                    int j=1;
                    for(int i=0;i<noOfPlayers;i++){
                        players[i].Tokens=Int32.Parse(lines[j]);
                        sum+=Int32.Parse(lines[j]);
                        j++;
                    }
                }
                sum+=1;
            }

            //If User resume the game, round is equal to total number of affections + 1
            round=sum;

            
            
            //Game Code
            while(checkWinner() ){
                Console.WriteLine("Press the Escape (Esc) key to quit and store game state and any other key to continue :");
                ch = Console.ReadKey();
                if (ch.Key == ConsoleKey.Escape)
                {
                    string path = @"record.txt";  
                    if (!File.Exists(path)) { // Create a file to write to   
                        using(StreamWriter sw = File.CreateText(path)) {  
                            sw.WriteLine(noOfPlayers);
                            foreach(Player l in players){
                                sw.WriteLine(l.Tokens);
                            }  
                              
                        }  
                    }
                    else{
                        using (StreamWriter sw = new StreamWriter(path))  
                        {  
                            sw.WriteLine(noOfPlayers);
                            foreach(Player l in players){
                                sw.WriteLine(l.Tokens);
                            }   
                        } 
                    }
                    Environment.Exit(0);
                }

                //At each new round, players cards is removed, and active status is changed to true
                ClearPlayers();

                round++;

                //Code below is for each Round Code
                
                bool checkRoundWinner=true;
                Deck deck=new Deck(); //new deck for each round
                Console.WriteLine("\n***** Round {0} *****\n",round);

                Console.WriteLine("Deck Contains");
                deck.printDeck();



                printActiveRoundPlayers(); //printing active players before start of round
                string card; //string to store random card

                Console.WriteLine("Distributing Cards......");

                //loop below is adding one random card to player cards
                for(int i=0;i<players.Count;i++){
                    card=deck.getRandomCard();
                    players[i].addCard(card,deck.getIndex(card)+1);
                }

                Console.WriteLine("************************");
                Console.WriteLine("Press Enter to start round");
                string _temp=Console.ReadLine();
                //loop for round
                while(checkRoundWinner){
                    
                    //if deck is empty,check for round winner and break the round
                    if(deck.isEmpty()){
                        checkRoundWinner=false;
                        Console.WriteLine("Deck is empty. Round over !!!");
                        deckEmptyWinner();
                        break;
                    }
                    else{
                        //loop to iterate each player for their turn
                        for(int i=0;i<players.Count;i++){
                            //condition to check wether player is eliminated from the round or not.
                            if(players[i].IsActive){
                                //If player is protected by handmaid from last turn,changing its status to false
                                if(players[i].IsProtected){
                                    players[i].IsProtected=false;
                                }
                                //Player turn code
                                Console.WriteLine("\nPlayer {0} turn",players[i].PlayerId);
                                Console.WriteLine("Tokens of Affection: {0}",players[i].Tokens);
                                Console.WriteLine("Drawing a Card....");
                                card=deck.getRandomCard();
                                players[i].addCard(card,deck.getIndex(card)+1);
                                Console.WriteLine("Cards to Play:");
                                players[i].printCards();

                                //if player has countess with king/prince, discarding the countess as per rule.
                                if(players[i].Cards.Contains("COUNTESS")){
                                    if(players[i].Cards.Contains("KING") || players[i].Cards.Contains("PRINCE")){
                                        Console.WriteLine("As you have King/Prince, you must discard to countess");
                                        Console.WriteLine("Discarding countess....");
                                        removeCardFromPlayer(players[i],"COUNTESS");
                                        continue; //making loop continue for next player turn.
                                    }
                                }
                                
                                //User input for player card to play    
                                Console.WriteLine("Enter Card name to Play:");
                                string cardToPlay=Console.ReadLine();
                                cardToPlay=cardToPlay.ToUpper();
                                //Below are conditions to check player card, and checking for wether only one player is
                                //active to play, then announce winner through method and start next round

                                //If player plays guard
                                if(String.Equals(cardToPlay,"GUARD")){
                                    playGuard(players[i]);
                                    if(isOnePlayerLeft()){
                                        checkRoundWinner=false;
                                        break;
                                    }
                                }
                                //If player plays priest
                                if(String.Equals(cardToPlay,"PRIEST")){
                                    playPriest(players[i]);
                                }
                                //if player plays baron
                                if(String.Equals(cardToPlay,"BARON")){
                                    playBaron(players[i]);
                                    if(isOnePlayerLeft()){
                                        checkRoundWinner=false;
                                        break;
                                    }
                                }
                                //if player plays handmaid
                                if(String.Equals(cardToPlay,"HANDMAID")){
                                    playHandmaid(players[i]);
                                }
                                //if player plays prince
                                if(String.Equals(cardToPlay,"PRINCE")){
                                    Player tempPlayer=playPrince(players[i]);
                                    if(isOnePlayerLeft()){
                                        checkRoundWinner=false;
                                        break;
                                    }
                                    foreach(Player k in players){
                                        if(k==tempPlayer){
                                            card=deck.getRandomCard();
                                             k.addCard(card,deck.getIndex(card)+1);
                                        }
                                    }
                                }
                                //if player plays king
                                if(String.Equals(cardToPlay,"KING")){
                                    playKing(players[i]);
                                        
                                }
                                //if player plays countess
                                if(String.Equals(cardToPlay,"COUNTESS")){
                                    removeCardFromPlayer(players[i],"COUNTESS");
                                }

                                //if player plays princess
                                if(String.Equals(cardToPlay,"PRINCESS")){
                                    removeCardFromPlayer(players[i],"PRINCESS");
                                    changeActiveStatus(players[i]);
                                    if(isOnePlayerLeft()){
                                        checkRoundWinner=false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    
                }   
            }


            
        }

//***Methods***

        //this method is Printing all active players of game those who are not protected by handmaid
        public static void printActiveRoundPlayers(){
            Console.WriteLine("\nActive Round Players:");
            foreach(Player i in players){
                if(i.IsActive){
                    if(!i.IsProtected)
                        Console.WriteLine("Player id:{0} - Tokens of Affection: {1}",i.PlayerId,i.Tokens);
                }
            }
            Console.WriteLine("\n");
        }
        //this method is Printing all active players of game those who are not protected by handmaid
        //except the player who turn is this
        public static void printActiveRoundPlayersExcept(Player plr){
            Console.WriteLine("\nActive Round Players:");
            foreach(Player i in players){
                if(plr!=i){
                    if(i.IsActive){
                        if(!i.IsProtected)
                            Console.WriteLine("Player id:{0} - Tokens of Affection: {1}",i.PlayerId,i.Tokens);
                    }
                }
                
            }
            Console.WriteLine("\n");
        }
        //This method is for prince play, as with prince you can choose yourself too, 
        //so printing all the active and non protexted players and current player too
        public static void printActivePrincePlayers(Player plr){
            Console.WriteLine("\nActive Round Players:");
            Console.WriteLine("Player id:{0} - Tokens of Affection: {1}",plr.PlayerId,plr.Tokens);
            foreach(Player i in players){
                if(plr!=i){
                    if(i.IsActive){
                        if(!i.IsProtected)
                            Console.WriteLine("Player id:{0} - Tokens of Affection: {1}",i.PlayerId,i.Tokens);
                    }
                }
                
            }
            Console.WriteLine("\n");
        }

        //**** each card play methods*****

        public static void playGuard(Player plr){
            removeCardFromPlayer( plr, "GUARD");
            printActiveRoundPlayersExcept(plr);
            if(isAnyActive(plr)){
                Console.WriteLine("Enter Player ID:");
                int guardPlayID=Convert.ToInt32(Console.ReadLine());
                Player guardToPlayer=findPlayerById(guardPlayID);
                Console.WriteLine("Guess chosen Player Card?");
                string guardGuesscard=Console.ReadLine().ToUpper();
            
                if(guardToPlayer.isCardExist(guardGuesscard)){
                    Console.WriteLine("You guess the card right!!");
                    Console.WriteLine("Player {0} is eliminated from the round!!",guardToPlayer.PlayerId);
                    changeActiveStatus(guardToPlayer);
                    
                }
                else{
                    Console.WriteLine("You guess the card wrong!!");
                    Console.WriteLine("You with PlayerID {0} is eliminated from the round!!",plr.PlayerId);
                    changeActiveStatus(plr);
                }
            }
            else{
                 Console.WriteLine("Everyone is protected by handsmaid!!");
            }
            
            
        }

        public static void playPriest(Player plr){
            removeCardFromPlayer( plr, "PRIEST");
            printActiveRoundPlayersExcept(plr);
            if(isAnyActive(plr)){
                Console.WriteLine("Enter Player ID:");
                int priestPlayID=Convert.ToInt32(Console.ReadLine());
                Player priestToPlayer=findPlayerById(priestPlayID);
                Console.WriteLine("Player card:");
                priestToPlayer.printCards();
            }
            else{
                Console.WriteLine("Everyone is protected by handsmaid!!");
            }
            
            
        }

        public static void playBaron(Player plr){
            removeCardFromPlayer( plr, "BARON");
            printActiveRoundPlayersExcept(plr);
            if(isAnyActive(plr)){
                Console.WriteLine("Enter Player ID:");
                int baronPlayID=Convert.ToInt32(Console.ReadLine());
                Player baronToPlayer=findPlayerById(baronPlayID);
                string playerAnotherCard=plr.getCardExcept("BARON");
                int playerCardValue=plr.getCardValue(playerAnotherCard);
                int baronPlayerCardValue=baronToPlayer.getCardValue( baronToPlayer.getCard());

                if( playerCardValue> baronPlayerCardValue   ){
                    Console.WriteLine("Your card is closer to Princess");
                    Console.WriteLine("Player {0} is eliminated from the round!!",baronToPlayer.PlayerId);
                    changeActiveStatus(baronToPlayer);
                
                }
                if(playerCardValue<baronPlayerCardValue ){
                    Console.WriteLine("Player {0} card is closer to Princess",baronToPlayer.PlayerId);
                    Console.WriteLine("You with Player id {0} is eliminated from the round!!",plr.PlayerId);
                    changeActiveStatus(plr);
                }
            }
            else{
                Console.WriteLine("Everyone is protected by handsmaid!!");
            }
        }
        
        public static void playHandmaid(Player plr){
            removeCardFromPlayer( plr,"HANDMAID");
            Console.WriteLine("You are being protected until your turn next round!!");
            foreach(Player i in players){
                if(i==plr)
                    i.IsProtected=true;;
            }
            
        }
        
        public static Player playPrince(Player plr){
            plr.removeCard("PRINCE");
            printActivePrincePlayers(plr);
            Console.WriteLine("Enter Player ID:");
            int PrincePlayID=Convert.ToInt32(Console.ReadLine());
            Player princeToPlayer=findPlayerById(PrincePlayID);

            bool check=false;
            if(String.Equals(princeToPlayer.getCard(),"PRINCESS")){
                changeActiveStatus(princeToPlayer);
                check=true;
            }

            if(plr==princeToPlayer){
               removeCardFromPlayer( plr,plr.getCardExcept("PRINCE"));
            }
            else{
                removeCardFromPlayer( princeToPlayer,princeToPlayer.getCard());
            }
                
            if(check)    
                return null;
            else    
                return princeToPlayer;
        }

        public static void playKing(Player plr){
            printActiveRoundPlayersExcept(plr);
            removeCardFromPlayer( plr,"KING");
            Console.WriteLine("Enter Player ID:");
            if(isAnyActive(plr)){
                int kingToPlayID=Convert.ToInt32(Console.ReadLine());
                Player kingToPlayer=findPlayerById(kingToPlayID);
                
                string playerCard=plr.getCardExcept("KING");
                string chosenPlayerCard=kingToPlayer.getCard();
                int playerCardValue=plr.getCardValue(playerCard);
                int chosenPlayerCardValue=kingToPlayer.getCardValue(chosenPlayerCard);


                removeCardFromPlayer( kingToPlayer,chosenPlayerCard);
                removeCardFromPlayer( plr,playerCard);
                

                addCardToPlayer(plr,chosenPlayerCard,chosenPlayerCardValue);
                addCardToPlayer(kingToPlayer,playerCard,playerCardValue);
            }
            else{
                Console.WriteLine("Everyone is protected by handsmaid!!");
            }
            

            
            
        }

        //method to find player in the list by ID
        public static Player findPlayerById(int id){
            foreach(Player i in players){
                if(i.PlayerId==id)
                    return i;   
            }
            return null;
        }

        //method to check wether there is only one active player left in the round
        public static bool isOnePlayerLeft(){


            int count=0;
            Player plr=null;
            foreach(Player i in players){
                if(i.IsActive){
                    count++;
                    plr=i;
                }
                    
            }

            if(count==1){
                addTokenOFAffection(plr);
                Console.WriteLine("Round over!!");
                Console.WriteLine("Player with PlayerID {0} wins the round and earn token of affection!!",plr.PlayerId);
            }
            return count==1? true: false;
        }


        //method to chnage player active status to eliminate from round
        public static void changeActiveStatus(Player plr){
            foreach(Player i in players){
                if(i==plr)
                    i.IsActive=false;
            }
        }

        //method to check wether any player is active or not protected by handmaid
        public static bool isAnyActive(Player plr){
            foreach(Player i in players){
                if(!i.IsProtected)
                    if(i!=plr)
                        return true;
            }
            return false;
        }


        //method to remove a card from player 
        public static void removeCardFromPlayer(Player plr,string card){
            foreach(Player i in players){
                if(i==plr)
                    i.removeCard(card);
            }
        }

        //method to add card to player
        public static void addCardToPlayer(Player plr,string card,int v){
            foreach(Player i in players){
                if(i==plr)
                    i.addCard(card, v);
            }
        }

        //method to add token to player
        public static void addTokenOFAffection(Player plr){
            foreach(Player i in players){
                if(i==plr)
                    i.addToken();
            }
        }

        //method to add protected status to player for handmaid
        public static void changeProtected(){
            foreach(Player i in players){
                i.IsProtected=false;;
            }
        }

        //method to clear player cards and chnage their active and protected status

        public static void ClearPlayers(){
            foreach(Player i in  players){
                i.clearCards();
                i.IsActive=true;
                i.IsProtected=false;
                i.TotalValue=0;
            }
        }
        

        //method to check for winner in case of empty deck
        public static void deckEmptyWinner(){
            int max=-1;
            Player plr=null;
            foreach(Player i in players){
                if(i.TotalValue>max){
                    max=i.TotalValue;
                    plr=i;
                }
            }

            Console.WriteLine("Player with PlayerID {0} wins the round and earn token of affection!!",plr.PlayerId);
            addTokenOFAffection(plr);
            
        }

        //method to check for overall game winner.
        public static bool checkWinner(){
            int playerCount=players.Count;
            if(playerCount==2){
                foreach(Player i in players){
                    if(i.Tokens==7){
                        Console.WriteLine("GAME over!!");
                        Console.WriteLine("Player with PlayerID {0} wins the Game",i.PlayerId);
                        return false;
                    }
                }
            }
            if(playerCount==3){
                foreach(Player i in players){
                    if(i.Tokens==5){
                        Console.WriteLine("GAME over!!");
                        Console.WriteLine("Player with PlayerID {0} wins the Game",i.PlayerId);
                        return false;
                    }
                }
            }
            if(playerCount==4){
                foreach(Player i in players){
                    if(i.Tokens==4){
                        Console.WriteLine("GAME over!!");
                        Console.WriteLine("Player with PlayerID {0} wins the Game",i.PlayerId);
                        return false;
                    }
                }
            }
            return true;
        }

        //method for if file exist in the directory
        public static bool checkForFile(){
            string fileName = @"record.txt";
            
            if(File.Exists(fileName)){
                return true;
            }
            else{
                return false;
            }
        }

        //Method for resume gameplay logic
        public static bool resumeGamePlay(){
            string fileName = @"record.txt";
            if(checkForFile()){
                using (StreamReader streamReader = File.OpenText(fileName))
                {
                    string text = streamReader.ReadToEnd();
                    streamReader.Close();
                    if(String.Equals(text,"")){
                        return false;
                    }
                    else{
                        Console.WriteLine("You have previous game play!!");
                        Console.WriteLine("Do you want to resume? Y/N");

                        string input=Console.ReadLine().ToUpper();
                        if(String.Equals(input,"Y")){
                            return true;
                        }
                        else{
                            using (StreamWriter sw = new StreamWriter(@"record.txt"))  
                            {  
                                sw.WriteLine("");
                                 
                            }
                            return false;
                        }
                        
                    }
                    
                }
            }
            return false;
        }


    }
}
