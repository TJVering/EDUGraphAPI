namespace EDUGraphAPI.DataSync
{
    public class User
    {
        public string ObjectId { get; set; }

        public virtual string JobTitle { get; set; }

        public virtual string Department { get; set; }

        public virtual string Mobile { get; set; }
    }
}