using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    public static string[] buttonsThatAreOccupied = new string[6];
    //public static int amountOfPlayersThatAreReady = 0;

    public async Task TryPickAScreenSpot(string nameOfPlayer, String numberOfButton)
    {
        int chosenButtonNumber;
        //double width = double.Parse(screenWidth);
        //double height = double.Parse(screenHeight);

        if (int.TryParse(numberOfButton, out chosenButtonNumber))
        {
            chosenButtonNumber--;

            if (buttonsThatAreOccupied[chosenButtonNumber] == String.Empty
                || buttonsThatAreOccupied[chosenButtonNumber] == null)
            {
                //Player can pick the spot so we will update all of the Players
                buttonsThatAreOccupied[chosenButtonNumber] = nameOfPlayer;

                await Clients.All.SendAsync("PlacementUpdateRecevied", nameOfPlayer,
                chosenButtonNumber);
                //amountOfPlayersThatAreReady++;
            }

            //if (amountOfPlayersThatAreReady >= 4)
            //{
            //    await Clients.All.SendAsync("GameIsAboutToStart");
            //}
        }
    }

    public async Task TryToDeselectScreenSpot(string nameOfPlayer, String numberOfpreviousChosenButton,
        String buttonThatPlayerWantsToDeselect)
    {
        int chosenButtonNumber;

        if (int.TryParse(numberOfpreviousChosenButton, out chosenButtonNumber))
        {
            chosenButtonNumber--;

            if (buttonThatPlayerWantsToDeselect == nameOfPlayer)
            {
                buttonsThatAreOccupied[chosenButtonNumber] = string.Empty;
                await Clients.All.SendAsync("DeSelectPlacementUpdateReceived", nameOfPlayer,
                chosenButtonNumber);
            }
        }
    }

    public async Task GetAmountOfPlayers()
    {
        int amountOfPlayers = 4;
        await Clients.All.SendAsync("GetAmountOfPlayers", amountOfPlayers);
    }

    public async Task RequestScreenUpdate(string PlayerId)
    {
        await Clients.Client(PlayerId).SendAsync("RecieveScreenUpdate", buttonsThatAreOccupied);
    }

    public async Task GameIsAboutToStart()
    {
        buttonsThatAreOccupied[0] = string.Empty;
        buttonsThatAreOccupied[1] = string.Empty;
        buttonsThatAreOccupied[2] = string.Empty;
        buttonsThatAreOccupied[3] = string.Empty;
        buttonsThatAreOccupied[4] = string.Empty;
        buttonsThatAreOccupied[5] = string.Empty;
        await Clients.All.SendAsync("StartGame");
    }


}