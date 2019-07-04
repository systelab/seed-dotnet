namespace Allure.builder.attributes
{
    using System.Collections.Generic;
    using Commons;

    public class step
    {
        public string id { set; get; }
        public string name { set; get; }
        public Stage stage { set; get; }
        public string description { set; get; }
        public List<Parameter> listParamenters { set; get; }
        public List<Attachment> listAttachment { set; get; }
    }
}