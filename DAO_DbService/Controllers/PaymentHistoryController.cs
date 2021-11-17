using DAO_DbService.Contexts;
using DAO_DbService.Models;
using Helpers.Models.DtoModels.MainDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_DbService.Mapping.AutoMapperBase;


namespace DAO_DbService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentHistoryController : Controller
    {

        [Route("Get")]
        [HttpGet]
        public IEnumerable<PaymentHistoryDto> Get()
        {
            List<PaymentHistory> model = new List<PaymentHistory>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.PaymentHistories.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<PaymentHistory>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<PaymentHistory>, List<PaymentHistoryDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public PaymentHistoryDto GetId(int id)
        {
            PaymentHistory model = new PaymentHistory();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.PaymentHistories.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new PaymentHistory();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<PaymentHistory, PaymentHistoryDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public PaymentHistoryDto Post([FromBody] PaymentHistoryDto model)
        {
            try
            {
                PaymentHistory item = _mapper.Map<PaymentHistoryDto, PaymentHistory>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.PaymentHistories.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<PaymentHistory, PaymentHistoryDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new PaymentHistoryDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<PaymentHistoryDto> PostMultiple([FromBody] List<PaymentHistoryDto> model)
        {
            try
            {
                List<PaymentHistory> item = _mapper.Map<List<PaymentHistoryDto>, List<PaymentHistory>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.PaymentHistories.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<PaymentHistory>, List<PaymentHistoryDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<PaymentHistoryDto>();
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    PaymentHistory item = db.PaymentHistories.FirstOrDefault(s => s.PaymentHistoryID == ID);
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return false;
            }
        }

        [Route("Update")]
        [HttpPut]
        public PaymentHistoryDto Update([FromBody] PaymentHistoryDto model)
        {
            try
            {
                PaymentHistory item = _mapper.Map<PaymentHistoryDto, PaymentHistory>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<PaymentHistory, PaymentHistoryDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new PaymentHistoryDto();
            }
        }

        [Route("GetPaymentHistoryByUserId")]
        [HttpGet]
        public IEnumerable<PaymentHistoryDto> GetPaymentHistoryByUserId(int userid)
        {
            List<PaymentHistory> model = new List<PaymentHistory>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.PaymentHistories.Where(x=>x.UserID== userid).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<PaymentHistory>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<PaymentHistory>, List<PaymentHistoryDto>>(model).ToArray();
        }
    }
}
