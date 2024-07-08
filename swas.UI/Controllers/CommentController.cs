using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using System.Threading.Tasks;
namespace swas.UI.Controllers
{
    public class CommentController : Controller
    {

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: Comment
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var comments = await _commentRepository.GetAllCommentsAsync();
            return View(comments);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: Comment/Details/5
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: Comment/Create
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var commets = await _commentRepository.GetAllCommentsAsync();
            return View(commets);
           // return View();
        }

        // POST: Comment/Create
        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_Comment comment)
        {
            if (ModelState.IsValid)
            {
                await _commentRepository.AddCommentAsync(comment);
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: Comment/Edit/5
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // POST: Comment/Edit/5
        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _commentRepository.UpdateCommentAsync(comment);
                return RedirectToAction(nameof(Index));
            }

            return View(comment);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: Comment/Delete/5
        [Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // POST: Comment/Delete/5
        [Authorize(Policy = "StakeHolders")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _commentRepository.DeleteCommentAsync(id);
            return RedirectToAction(nameof(Index));
        }


       
    }
}
