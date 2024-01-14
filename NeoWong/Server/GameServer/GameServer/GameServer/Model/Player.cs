using Common.Network;

namespace GameServer.Model
{
    public class Player
    {
        public string id = "";

        //临时数据，如：坐标
        public int x;
        public int y;
        public int z;

        //数据库数据
        public PlayerData data;

        public Connection connection;

        public Player(Connection connection)
        {
            this.connection = connection;
        }
    }
}