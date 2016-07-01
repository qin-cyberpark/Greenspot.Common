using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using MySql.Data.MySqlClient;

namespace Greenspot.Identity.MySQL
{
    public class UserSnsInfoRepository<TUser> where TUser : IdentityUser
    {
        private MySqlDatabase _database;
        public UserSnsInfoRepository(MySqlDatabase database)
        {
            _database = database;
        }

        public int Insert(string userId, GreenspotUserSnsInfo snsInfo)
        {
            return _database.ExecuteNonQuery(@"INSERT INTO greenspot_user_snsinfos(UserId,SnsName,InfoKey,InfoValue) VALUES(@UserId,@SnsName,@InfoKey,@InfoValue)",
                                     new Dictionary<string, object>{
                                                {"@UserId", userId},
                                                {"@SnsName", snsInfo.SnsName},
                                                {"@InfoKey",snsInfo.InfoKey},
                                                {"@InfoValue",snsInfo.InfoValue}
                                    });
        }

        public int Delete(string userId)
        {
            return _database.ExecuteNonQuery(@"DELETE FROM greenspot_user_snsinfos WHERE UserId=@UserId",
                                     new Dictionary<string, object>{
                                            {"@UserId", userId}
                                     });
        }

        public SortedList<string, GreenspotUserSnsInfo> PopulateSnsInfos(string userId)
        {
            var snsInfos = new SortedList<string, GreenspotUserSnsInfo>();
            var rows = _database.Query(@"SELECT * FROM greenspot_user_snsinfos WHERE UserId=@Id",
                                         new Dictionary<string, object>{
                                            {"@Id", userId}
                                         });

            foreach (var row in rows)
            {
                if (!snsInfos.ContainsKey(row["InfoKey"]))
                {
                    snsInfos.Add(row["InfoKey"], new GreenspotUserSnsInfo(row["SnsName"], row["InfoKey"], row["InfoValue"]));
                }
            }

            return snsInfos;
        }
    }
}
