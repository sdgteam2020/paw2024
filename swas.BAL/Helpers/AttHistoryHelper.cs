
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using swas.DAL.Models;


namespace swas.BAL.Helpers
{
    public static class AttHistoryHelpers
    {
        


            public static IHtmlContent YourTableEmittingMethod(this IHtmlHelper helper, int psmId, List<tbl_AttHistory> attachments)
        {

            

            var tableHtml = new TagBuilder("table");
         

            return tableHtml;
        }
    }



}
