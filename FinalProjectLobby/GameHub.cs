using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    public static string[] buttonsThatAreOccupied = new string[6];
    //public static int amountOfPlayersThatAreReady = 0;

    public async Task TryPickAScreenSpot(string nameOfClient, String numberOfButton)
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
                //Client can pick the spot so we will update all of the clients
                buttonsThatAreOccupied[chosenButtonNumber] = nameOfClient;

                await Clients.All.SendAsync("PlacementUpdateRecevied", nameOfClient,
                chosenButtonNumber);
                //amountOfPlayersThatAreReady++;
            }

            //if (amountOfPlayersThatAreReady >= 4)
            //{
            //    await Clients.All.SendAsync("GameIsAboutToStart");
            //}
        }
    }

    public async Task TryToDeselectScreenSpot(string nameOfClient, String numberOfpreviousChosenButton,
        String buttonThatClientWantsToDeselect)
    {
        int chosenButtonNumber;

        if (int.TryParse(numberOfpreviousChosenButton, out chosenButtonNumber))
        {
            chosenButtonNumber--;

            if (buttonThatClientWantsToDeselect == nameOfClient)
            {
                buttonsThatAreOccupied[chosenButtonNumber] = string.Empty;
                await Clients.All.SendAsync("DeSelectPlacementUpdateReceived", nameOfClient,
                chosenButtonNumber);
            }
        }
    }

    public async Task GetAmountOfPlayers()
    {
        int amountOfPlayers = 3;
        await Clients.All.SendAsync("GetAmountOfPlayers", amountOfPlayers);
    }

    public async Task RequestScreenUpdate(string clientId)
    {
        await Clients.Client(clientId).SendAsync("RecieveScreenUpdate", buttonsThatAreOccupied);
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