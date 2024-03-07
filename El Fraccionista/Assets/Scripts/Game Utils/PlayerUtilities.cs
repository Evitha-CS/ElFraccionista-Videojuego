using Photon.Realtime;
using Photon.Pun;

namespace GameUtils
{
    public static class PlayerUtilities
    {
        public static string GetOtherPlayerName()
        {
            Player[] players = PhotonNetwork.PlayerList;
            foreach (Player player in players)
            {
                if (!player.IsMasterClient)
                {
                    return player.NickName;
                }
            }
            return ""; // Manejar el caso en el que no se encuentre otro jugador
        }

        public static int GetOtherPlayerID()
        {
            Player[] players = PhotonNetwork.PlayerList;
            foreach (Player player in players)
            {
                if (!player.IsMasterClient)
                {
                    return (int)player.CustomProperties["userId"];
                }
            }
            return -1; // Manejar el caso en el que no se encuentre otro jugador
        }


    }
}


