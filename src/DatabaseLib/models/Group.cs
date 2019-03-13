namespace gov.sandia.sld.common.db.models
{
    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }

        public Group()
        {
            id = -1;
            name = string.Empty;
        }
    }
}
