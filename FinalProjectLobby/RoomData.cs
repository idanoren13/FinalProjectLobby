namespace FinalProjectLobby
{
    public class RoomData
    {
        public string RoomCode { get; }
        public string ServerIp { get; set; }
        public List<string> m_Players;

        public RoomData(string i_RoomCode)
        {
            RoomCode = i_RoomCode;
            ServerIp = string.Empty;
        }

        public void AddPlayer(string i_Name)
        {
            m_Players.Add(i_Name);
        }

        public bool CheckIfNameExist(string i_Name)
        {
            return m_Players.Contains(i_Name);
        }
    }
}
