using MemeSearch.Logic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemeSearch.Logic.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateSearch(string query, int results, int page, out string message)
        {
            var messages = new StringBuilder();
            ValidateQuery(query, messages);
            ValidatePagination(results, page, messages);
            message = messages.ToString();
            return messages.Length == 0;
        }

        public static bool ValidateAdvancedSearch(string query, SearchParameters parameters, int results, int page, out string message)
        {
            var messages = new StringBuilder();
            ValidateQuery(query, messages, true);
            ValidatePagination(results, page, messages);
            ValidateAdvancedSearchParams(parameters, messages);
            message = messages.ToString();
            return message.Length == 0;
        }

        public static bool ValidateImageSearch(ImageSearchParameters parameters, int results, int page, out string message)
        {
            var messages = new StringBuilder();
            ValidatePagination(results, page, messages);
            ValidateImageSearchParams(parameters, messages);
            ValidateAdvancedSearchParams(parameters, messages);
            message = messages.ToString();
            return message.Length == 0;
        }


        // todo: finish
        private static void ValidateQuery(string query, StringBuilder message, bool allowEmpty = false)
        {
            if (string.IsNullOrWhiteSpace(query) && !allowEmpty) message.AppendLine("Empty query");

            var brackets = 0;
            for (int i=0; i<query.Length; i++)
            {
                if (query[i] == '(') brackets++;
                if (query[i] == ')')
                {
                    if (brackets == 0) message.AppendLine("Invalid order of brackets");
                    brackets--;
                }
            }

            if (brackets != 0) message.AppendLine("Invalid number of brackets");
        }

        private static void ValidatePagination(int results, int page, StringBuilder message)
        {
            if (page < 0) message.AppendLine("Page cannot be less than 0");
            if (results < 0) message.AppendLine("Results cannot be less than 0");
        }

        private static readonly List<string> _validSearchFields = new List<string>() { "Title", "Content", "Category", "Details", "Image" };
        private static readonly List<string> _validSortFields = new List<string>() { "Year" };

        private static void ValidateAdvancedSearchParams(SearchParameters parameters, StringBuilder message)
        {
            if (!parameters.Fields.Any()) message.AppendLine("Fields must contain at least 1 element");
            if (!parameters.Fields.All(f => _validSearchFields.Contains(f))) message.AppendLine("Invalid Fields list. Valid options are: " + string.Join(", ", _validSearchFields));
            if (parameters.Sort != null && !_validSortFields.Contains(parameters.Sort))message.AppendLine("Invalid Sort. Valid options are: " + string.Join(", ", _validSortFields));
            if (parameters.YearFrom.HasValue && parameters.YearTo.HasValue && parameters.YearFrom.Value > parameters.YearTo.Value) message.AppendLine("YearFrom cannot be greater than YearTo");
        }

        private static void ValidateImageSearchParams(ImageSearchParameters parameters, StringBuilder message)
        {
            if (string.IsNullOrWhiteSpace(parameters.Url)) message.AppendLine("Image url cannot be empty");
        }
    }
}
