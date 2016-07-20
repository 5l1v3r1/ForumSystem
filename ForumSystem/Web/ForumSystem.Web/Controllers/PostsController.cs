﻿namespace ForumSystem.Web.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;

    using AutoMapper.QueryableExtensions;

    using ForumSystem.Data.Models;
    using ForumSystem.Data.UnitOfWork;
    using ForumSystem.Web.InputModels.Posts;
    using ForumSystem.Web.ViewModels.Post;

    using Microsoft.AspNet.Identity;

    public class PostsController : BaseController
    {
        public PostsController(IForumSystemData data)
            : base(data)
        {
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var post = this.Data.Posts.GetById(id);
            if (post == null)
            {
                return this.HttpNotFound();
            }

            var viewModel = this.Data.Posts.All()
                .Where(p => p.Id == id)
                .ProjectTo<PostViewModel>()
                .FirstOrDefault();

            return this.View(viewModel);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = this.Data.Categories.GetById(id);
            if (category == null)
            {
                return this.HttpNotFound();
            }

            var inputModel = new PostInputModel { CategoryId = category.Id, Category = category.Title };

            return this.View(inputModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Content,CategoryId,Category")] PostInputModel input)
        {
            if (input != null && this.ModelState.IsValid)
            {
                var userId = this.User.Identity.GetUserId();
                var post = new Post
                               {
                                   Title = input.Title, 
                                   Content = input.Content, 
                                   AuthorId = userId, 
                                   CategoryId = input.CategoryId
                               };

                this.Data.Posts.Add(post);
                this.Data.SaveChanges();

                return this.RedirectToAction("Details", "Posts", new { area = string.Empty, id = post.Id });
            }

            return this.View(input);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = this.User.Identity.GetUserId();
            var post = this.Data.Posts.GetById(id);

            if (post == null)
            {
                return this.HttpNotFound();
            }

            if (post.AuthorId != userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = new PostEditModel { Id = post.Id, Title = post.Title, Content = post.Content };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content")] PostEditModel model)
        {
            if (model != null && this.ModelState.IsValid)
            {
                var userId = this.User.Identity.GetUserId();
                var post = this.Data.Posts.GetById(model.Id);

                if (post.AuthorId != userId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                post.Title = model.Title;
                post.Content = model.Content;

                this.Data.Posts.Update(post);
                this.Data.SaveChanges();

                return this.RedirectToAction("Details", "Posts", new { area = string.Empty, id = post.Id });
            }

            return this.View(model);
        }
    }
}