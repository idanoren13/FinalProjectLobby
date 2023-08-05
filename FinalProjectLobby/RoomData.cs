namespace FinalProjectLobby
{
    public class RoomData
    {
        public string RoomCode { get; }
        public string ServerIp { get; set; }
        public List<string> m_Players = new List<string>();
        public string? m_ChosenGame = null;
        public bool m_HostLeft = false;
        public bool m_NeedToGoToNextPage = false;

        public RoomData(string i_RoomCode)
        {
            RoomCode = i_RoomCode;
            ServerIp = string.Empty;
        }

        public void AddPlayer(string i_Name)
        {
            m_Players.Add(i_Name);
        }

        public void RemovePlayer(string i_Name)
        {
            m_Players.Remove(i_Name);
        }

        public bool CheckIfNameExist(string i_Name)
        {
            return m_Players.Contains(i_Name);
        }

        public List<string> GetPlayersList()
        {
            return m_Players;
        }

        public string? GetGame()
        {
            return m_ChosenGame;
        }

        public void SetGame(string i_Game)
        {
            m_ChosenGame = i_Game;
        }

        public void MarkHostLeft()
        {
            m_HostLeft = true;
        }

        public bool CheckIfHostLeft()
        {
            return m_HostLeft;
        }

        public void MarkAsNeedToGoToNextPage()
        {
            m_NeedToGoToNextPage = true;
        }

        public bool CheckIfNeedToGoToNextPage()
        {
            return m_NeedToGoToNextPage;
        }
    }
}
