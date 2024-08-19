using System.ComponentModel.DataAnnotations;

namespace TasksManagementServer.DTO
{
    public class UrgencyLevel
    {

        public int UrgencyLevelId { get; set; }
        public string UrgencyLevelName { get; set; } = null!;

    }
}
