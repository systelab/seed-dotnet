using Allure.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allure.builder.attributes
{
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
