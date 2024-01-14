using Common;
using Common.Network;
using GameServer.Model;
using Proto.User;

namespace GameServer.Service
{
    public class UserService : Singleton<UserService>
    {
        public void Start()
        {
            MessageRouter.Instance.Subscribe<UserRegisterRequest>(OnUserRegisterRequest);
            MessageRouter.Instance.Subscribe<UserLoginRequest>(OnUserLoginRequest);
        }

        void OnUserRegisterRequest(Connection conn, UserRegisterRequest req)
        {
            if (string.IsNullOrEmpty(req.Username))
                return;

            if (string.IsNullOrEmpty(req.Password))
                return;

            UserRegisterResponse res = new UserRegisterResponse();

            //注册
            if (DbManager.Register(req.Username, req.Password))
            {
                DbManager.CreatePlayer(req.Username);
                res.Code = UserCode.Success;
                res.Message = "Register Success";
            }
            else
            {
                res.Code = UserCode.Usernameexisted;
                res.Message = "Register Fail";
            }          
            conn.Send(res);
        }

        void OnUserLoginRequest(Connection conn, UserLoginRequest req)
        {
            if (string.IsNullOrEmpty(req.Username))
                return;

            if (string.IsNullOrEmpty(req.Password))
                return;

            UserLoginResponse res = new UserLoginResponse();

            //密码校验
            if (!DbManager.CheckPassword(req.Username, req.Password))
            {
                res.Code = UserCode.Passwordinvalid;
                res.Message = "Login Fail";
                conn.Send(res);
                return;
            }

            //不允许再次登陆
            if (conn.Player != null)
            {
                res.Code = UserCode.Usernameexisted;
                res.Message = "Repeat Login Fail";
                conn.Send(res);
                return;
            }

            //获取玩家数据
            PlayerData playerData = DbManager.GetPlayerData(req.Username);
            if (playerData == null)
            {
                res.Code = UserCode.Usernameinvalid;
                res.Message = "Can not find Player";
                conn.Send(res);
                return;
            }

            //构建Player
            Player player = new Player(conn);
            player.id = req.Username;
            player.data = playerData;
            PlayerManager.AddPlayer(player.id, player);
            conn.Player = player;

            //返回协议
            res.Code = UserCode.Success;
            res.Message = $"Player: {player.id} Login Success";
            conn.Send(res);
        }
    }
}
