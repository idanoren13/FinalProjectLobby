﻿namespace FinalProjectLobby
{
    public class RoomsManager
    {
        private static RoomsManager? s_Instance = null;
        private static readonly object sr_Lock = new object();
        private readonly Dictionary<string, RoomData> r_Rooms = new Dictionary<string, RoomData>();
        private Dictionary<string, List<string>> m_RemovedPlayers = new Dictionary<string, List<string>>();
        private const int k_MaxPlayersAmount = 4;

        private RoomsManager()
        {
        }

        public static RoomsManager? Instance
        {
            get
            {
                lock(sr_Lock)
                {
                    return s_Instance ??= new RoomsManager();
                }
            }
        }

        public RoomData CreateNewRoom(string i_RoomCode, string i_HostName)
        {
            RoomData roomData = new RoomData(i_RoomCode);

            roomData.AddPlayer(i_HostName);
            r_Rooms.Add(i_RoomCode, roomData);
            m_RemovedPlayers[i_RoomCode] = new List<string>();
            //r_Rooms.Add(i_RoomCode, new RoomData(i_RoomCode));
            // TODO:
            // deploy the room to the server as a container?
            // return the server ip

            return r_Rooms[i_RoomCode];
        }

        public string JoinRoom(string i_RoomCode)
        {
            if (r_Rooms.ContainsKey(i_RoomCode))
            {
                RoomData room = r_Rooms[i_RoomCode];
                if (room.m_Players.Count == k_MaxPlayersAmount)
                {
                    return Messages.FullCapacity;
                }
            }

            return r_Rooms.ContainsKey(i_RoomCode) ? r_Rooms[i_RoomCode].ServerIp : Messages.CannotJoinRoom;
        }

        public bool AddPlayerToRoom(string i_RoomCode, string i_Name)
        {
            RoomData room = r_Rooms[i_RoomCode];
            bool result;

            if (!room.CheckIfNameExist(i_Name))
            {
                room.AddPlayer(i_Name);
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public List<string>? GetPlayersList(string i_Code)
        {
            if (r_Rooms.ContainsKey(i_Code))
            {
                RoomData room = r_Rooms[i_Code];
                return room.GetPlayersList();
            }

            return null;
        }

        public bool RemovePlayer(string i_RoomCode, string i_PlayerToRemove)
        {
            RoomData room = r_Rooms[i_RoomCode];
            bool result;

            if (room.CheckIfNameExist(i_PlayerToRemove))
            {
                room.RemovePlayer(i_PlayerToRemove);
                m_RemovedPlayers[i_RoomCode].Add(i_PlayerToRemove);
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public bool PlayerLeft(string i_RoomCode, string i_PlayerToRemove)
        {
            RoomData room = r_Rooms[i_RoomCode];
            bool result;

            if (room.CheckIfNameExist(i_PlayerToRemove))
            {
                room.RemovePlayer(i_PlayerToRemove);
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public void ClearRemovedPlayers(string i_RoomCode)
        {
            if (m_RemovedPlayers.ContainsKey(i_RoomCode))
                m_RemovedPlayers[i_RoomCode].Clear();
        }

        public List<string>? GetPlayersToRemove(string i_RoomCode)
        {
            if (m_RemovedPlayers.ContainsKey(i_RoomCode))
                return m_RemovedPlayers[i_RoomCode];
            return null;
        }

        public string? GetChosenGame(string i_RoomCode)
        {
            if (r_Rooms.ContainsKey(i_RoomCode))
                return r_Rooms[i_RoomCode].GetGame();
            return null;
        }

        public void SetChosenGame(string i_RoomCode, string i_GameName)
        {
            r_Rooms[i_RoomCode].SetGame(i_GameName);
        }

        public void MarkHostLeft(string i_RoomCode)
        {
            r_Rooms[i_RoomCode].MarkHostLeft();
        }

        public bool CheckIfHostLeft(string i_RoomCode)
        {
            if (r_Rooms.ContainsKey(i_RoomCode))
                return r_Rooms[i_RoomCode].CheckIfHostLeft();

            return false;
        }

    }
}
