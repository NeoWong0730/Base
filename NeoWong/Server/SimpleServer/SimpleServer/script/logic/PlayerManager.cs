using System.Collections.Generic;

public class PlayerManager
{
    /// <summary>
    /// 玩家列表
    /// </summary>
    static Dictionary<string, Player> players = new Dictionary<string, Player>();

    /// <summary>
    /// 玩家是否在线
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsOnline(string id)
    {
        return players.ContainsKey(id);
    }

    /// <summary>
    /// 获取玩家
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Player GetPlayer(string id)
    {
        return players[id];
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="id"></param>
    /// <param name="player"></param>
    public static void AddPlayer(string id, Player player)
    {
        players.Add(id, player);
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    /// <param name="id"></param>
    public static void RemovePlayer(string id)
    {
        players.Remove(id);
    }
}


