using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace EmployeeManagementSy.Models
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry) 
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; set; }

        public string UserId { get; set; }

        public string TableName { get; set; }

        public Dictionary<string, object> KeyValues { get;} = new Dictionary<string, object>();

        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();

        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

        public AuditType AuditType { get; set; }

        public List<string> ChangedColumns { get; } = new List<string>();

        public Audit ToAudi()
        {
            var  audi = new Audit();
            audi.UserId = UserId;
            audi.AuditType = AuditType.ToString();
            audi.TableName = TableName;
            audi.DateTime = DateTime.Now;
            audi.PrimaryKey = JsonConvert.SerializeObject(KeyValues);
            audi.OldValues = OldValues.Count==0? null : JsonConvert.SerializeObject(OldValues);
            audi.NewValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
            audi.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns);

            return audi;
        }
    }
}
