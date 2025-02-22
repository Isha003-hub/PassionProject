namespace MuseAndMasterpiece.Models
{
    public class ServiceResponse
    {
        public enum ServiceStatus { NotFound, Created, Updated, Deleted, Error, AlreadyExists, NotLinked }

        public ServiceStatus Status { get; set; }

        public int CreatedId { get; set; }

        public List<string> Messages { get; set; } = new List<string>();
    }
}
