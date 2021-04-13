using MemeSearch.Logic.Models;

namespace MemeSearch.Logic.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateSearch(string query, int page, out string message)
        {
            return ValidateQuery(query, out message) && ValidatePage(page, out message);
        }

        public static bool ValidateAdvancedSearch(string query, SearchParameters parameters, int page, out string message)
        {
            return ValidateQuery(query, out message)
                && ValidatePage(page, out message)
                && ValidateAdvancedSearchParams(parameters, out message);
        }


        // todo: finish
        private static bool ValidateQuery(string query, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(query)) message = "Empty query";

            return true;
        }

        private static bool ValidatePage(int page, out string message)
        {
            message = string.Empty;

            if (page < 0) message = "Page cannot be less than 0";

            return true;
        }

        private static bool ValidateAdvancedSearchParams(SearchParameters parameters, out string message)
        {
            message = string.Empty;
            return true;
        }
    }
}
