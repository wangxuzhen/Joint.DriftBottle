using Joint.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Joint.IService
{
    public partial interface IBottleService : IBaseService<Bottle>
    {
        DataTable GetBottleListByRd(int userID, DateTime beginTime);
        List<int> GetNoReadCount(int userID);
    }
}
