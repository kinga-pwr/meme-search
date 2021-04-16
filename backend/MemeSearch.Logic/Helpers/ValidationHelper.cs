﻿using MemeSearch.Logic.Models;

namespace MemeSearch.Logic.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateSearch(string query, int results, int page, out string message)
        {
            return ValidateQuery(query, out message) && ValidatePagination(results, page, out message);
        }

        public static bool ValidateAdvancedSearch(string query, SearchParameters parameters, int results, int page, out string message)
        {
            return ValidateQuery(query, out message)
                && ValidatePagination(results, page, out message)
                && ValidateAdvancedSearchParams(parameters, out message);
        }


        // todo: finish
        private static bool ValidateQuery(string query, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(query)) message = "Empty query";

            return true;
        }

        private static bool ValidatePagination(int results, int page, out string message)
        {
            message = string.Empty;

            if (page < 0) message = "Page cannot be less than 0";
            if (results < 0) message = "Results cannot be less than 0";

            return true;
        }

        private static bool ValidateAdvancedSearchParams(SearchParameters parameters, out string message)
        {
            message = string.Empty;
            return true;
        }
    }
}
