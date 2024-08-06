using Microsoft.AspNetCore.Http;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using swas.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using static Grpc.Core.Metadata;
using System.Collections;

namespace swas.BAL.Repository
{
    public class TrnChatMsgRepository : GenericRepositoryDL<TrnChatMsg>, ITrnChatMsgRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TrnChatMsgRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;

        }

        public async Task<List<TrnChatMsg>> GetChat(int UserMapChatId,string FromUserId, string ToUserId)
        {

            List<TrnChatMsg> trnChatMsgs = new List<TrnChatMsg>();  
            var ret = await _context.mUserMapChat.Where(i => i.FromUserId == FromUserId && i.ToUserId==ToUserId).SingleOrDefaultAsync();
			if(ret!=null)
			{
				var friends = _dbContext.TrnChatMsg.Where(f => f.UserMapChatId==ret.UserMapChatId).ToList();
				friends.ForEach(a => a.IsRead = true);
			    _dbContext.SaveChanges();
			}
			
			var chat = await (from msg in _context.TrnChatMsg

							  where msg.UserMapChatId == UserMapChatId

							 select new TrnChatMsg
                             {
								ChatId = msg.ChatId,
								UserMapChatId = msg.UserMapChatId,
								Msg = msg.Msg,
								CreatedOn = msg.CreatedOn,
								type = 1,
								IsRead= msg.IsRead
							 }).ToListAsync();

			//var chat = await _context.TrnChatMsg.Where(i => i.UserMapChatId == UserMapChatId).ToListAsync();
			//var chat1 = await _context.TrnChatMsg.Where(i => i.UserMapChatId == ret.UserMapChatId).ToListAsync();
			if(ret!=null)
			{
				var chat1 = await (from msg in _context.TrnChatMsg

								   where msg.UserMapChatId == ret.UserMapChatId

								   select new TrnChatMsg
								   {
									   ChatId= msg.ChatId,
									   UserMapChatId = msg.UserMapChatId,
									   Msg = msg.Msg,
									   CreatedOn = msg.CreatedOn,
									   type = 2,
									   IsRead = msg.IsRead
								   }).ToListAsync();

				chat.AddRange(chat1);
			}
			//var retww = chat.OrderByDescending(i => i.CreatedOn);
			return chat.OrderBy(i => i.CreatedOn).ToList();
		}

		public async Task<List<DTOIsChat>> GetIsChat(string ToUserId)
		{
			var ret =await (from map in _context.mUserMapChat
					 join chat in _context.TrnChatMsg on map.UserMapChatId equals chat.UserMapChatId
					 where chat.IsRead == false && map.ToUserId == ToUserId
							group map by new { map.FromUserId, chat.CreatedOn } into g
					 select new DTOIsChat
					 {
						 FromUserID = g.Key.FromUserId,
						 CreatedOn=g.Key.CreatedOn,
						 Total = g.Count()
					 }).ToListAsync();

			return ret;
		}
	}
}
