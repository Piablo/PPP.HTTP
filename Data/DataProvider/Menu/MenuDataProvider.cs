using DisplayModels.Menu;
using PPP2.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DataProvider.Menu
{
    public class MenuDataProvider
    {
        public IEnumerable<MenuItemsDisplayModel> GetData(MenuRequestModel lm)
        {
            try
            {
                using (var db = DbContextProvider.GetDbContext().InReadContext())
                {
                    var param = new Dapper.DynamicParameters();
                    //param.Add("Id", lm.Id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                    param.Add("Name", lm.Name, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    var result = db.ExecProcWithResults<MenuItemsDisplayModel>("dbo.spFetchMenu", param);//.ToList().First();
                    return result;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return null;
        }
    }
}
