using System.Collections.Generic;

namespace MemeSearch.Logic.Interfaces
{
    public interface IAdditionalInformationService
    {
        HashSet<string> Statuses { get; }
        Dictionary<string, int> Details { get; }
        Dictionary<string, int> Categories { get; }
    }
}
