using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Helpers;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Reaction;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class ReactionService : IReactionService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICommonService _commonService;
        private readonly IIdentityService _identityService;
        public ReactionService(URLSDbContext db, IMapper mapper, ICommonService commonService, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _commonService = commonService;
            _identityService = identityService;
        }

        public async Task<Result<ReactionViewModel>> CreateAsync(ReactionCreateModel reaction)
        {
            var postRequest = await _commonService.IsExistWithResultsAsync<Post>(s => s.Id == reaction.PostId);

            if (!postRequest.IsExist)
                return Result<ReactionViewModel>.NotFound(typeof(Post).NotFoundMessage(reaction.PostId));

            var post = postRequest.Results.First();

            if (!ReactionHelper.ValidateReactionByPost(reaction.ReactionId, post))
                return Result<ReactionViewModel>.Error("Reaction not valid");

            var response = await _commonService.IsExistWithResultsAsync<Reaction>(s => s.PostId == reaction.PostId && s.FromId == _identityService.GetUserId());

            if (response.IsExist)
            {
                var reactionToUpdate = response.Results.First();

                reactionToUpdate.ReactionTypeId = reaction.ReactionId;
                reactionToUpdate.PrepareToUpdate(_identityService);

                _db.Reactions.Update(reactionToUpdate);
                await _db.SaveChangesAsync();
                return Result<ReactionViewModel>.SuccessWithData(_mapper.Map<ReactionViewModel>(reactionToUpdate));
            }
            else
            {
                var newReaction = new Reaction
                {
                    FromId = _identityService.GetUserId(),
                    ReactionTypeId = reaction.ReactionId,
                    PostId = reaction.PostId
                };
                newReaction.PrepareToCreate(_identityService);

                await _db.Reactions.AddAsync(newReaction);
                await _db.SaveChangesAsync();

                var viewModel = _mapper.Map<ReactionViewModel>(newReaction);

                return Result<ReactionViewModel>.Created(viewModel);
            }
        }

        public async Task<Result<bool>> DeleteAsync(ReactionCreateModel reaction)
        {
            var reactionToDelete = await _db.Reactions
                .FirstOrDefaultAsync(
                s => s.PostId == reaction.PostId &&
                s.FromId == _identityService.GetUserId());

            if (reactionToDelete == null)
                return Result<bool>.NotFound(typeof(Reaction).NotFoundMessage(reaction.ReactionId));

            _db.Reactions.Remove(reactionToDelete);
            await _db.SaveChangesAsync();

            return Result<bool>.Success();
        }

        public async Task<Result<List<ReactionViewModel>>> GetAllByPostIdAsync(int postId, int offset = 0, int count = 20)
        {
            var query = _db.Reactions.AsNoTracking()
                .Include(s => s.From)
                .Where(s => s.PostId == postId)
                .Skip(offset).Take(count)
                .OrderByDescending(s => s.CreatedAt);

            var reactions = await query.ToListAsync();

            var reactionsToView = _mapper.Map<List<ReactionViewModel>>(reactions);

            var totalCount = await _commonService.CountAsync<Reaction>(s => s.PostId == postId);

            return Result<List<ReactionViewModel>>.SuccessList(reactionsToView, Meta.FromMeta(totalCount, offset, count));
        }

        public Result<Dictionary<int, string>> GetAllReactions()
        {
            return Result<Dictionary<int, string>>.SuccessList(ReactionData.GetAllReactions());
        }

        public async Task<Result<ReactionStatistics>> GetAllReactionsByPostIdAsync(int postId)
        {
            var reactionsByPost = await _db.Reactions.AsNoTracking()
                .Where(s => s.PostId == postId)
                .ToListAsync();

            var results = new Dictionary<string, int>();

            reactionsByPost.ForEach(reaction =>
            {
                var key = ReactionHelper.GetReactionFromId(reaction.ReactionTypeId);
                if (results.ContainsKey(key))
                {
                    var value = results[key];
                    results[key] = value + 1;
                }
                else
                {
                    results.Add(key, 1);
                }
            });

            return Result<ReactionStatistics>.SuccessWithData(new ReactionStatistics
            {
                Reactions = results
            });
        }
    }
}