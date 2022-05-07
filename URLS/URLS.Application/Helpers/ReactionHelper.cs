using URLS.Application.ViewModels.Reaction;

namespace URLS.Application.Helpers
{
    public class ReactionHelper
    {
        public static string GetReactionFromId(int id)
        {
            if (id == 1)
                return ReactionData.Like;
            if (id == 2)
                return ReactionData.Dislike;
            if (id == 3)
                return ReactionData.Heart;
            if (id == 4)
                return ReactionData.Congratulations;
            if (id == 5)
                return ReactionData.Laughter;
            if (id == 6)
                return ReactionData.Shit;
            if (id == 7)
                return ReactionData.Swearing;
            if (id == 8)
                return ReactionData.Cry;
            if (id == 9)
                return ReactionData.Wow;
            return string.Empty;
        }

        public static bool ValidateReactionId(int id)
        {
            if (id >= 1 && id <= 9)
                return true;
            return false;
        }
    }
}
