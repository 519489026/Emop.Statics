using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MXR.Utilities.Security.Cryptography;
using StatisticalCenter.DataAccess;
using StatisticalCenter.DataAccess.TableDataCollection;
using StatisticalCenter.Model;

namespace StatisticalApi.Controllers
{
    [RoutePrefix("tj")]
    public class StatisticsQrCodeController : ApiController
    {
        private readonly T_CountToolHistory _toolHistory = new T_CountToolHistory();

        [Route("save")]
        [HttpPost]
        public async Task<string> Save([FromBody]RequestSaveQrInfoModel model)
        {

            int qId = Convert.ToInt32(Decryption.MXR(model.QrCode));

            return
                await
                    Task.FromResult(_toolHistory.SaveQrCodeStatistical(qId, model.PhoneType, model.Ip, model.Province, model.EnterType,model.CilentCookie)
                        ? "ok"
                        : "no");
        }

        [Route("get")]
        public async Task<string> GetInfo()
        {
            return await Task.FromResult("123123");
        }
    }
}
