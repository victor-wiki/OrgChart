using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OrgChart
{
    public class OrgChartNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int Order { get; set; }

        public virtual OrgChartNode Parent { get; set; }
        public List<OrgChartNode> Children { get; } = new List<OrgChartNode>();

        public Rectangle Position { get; set; }

        internal int OffsetX { get; set; }
        public int Level { get; internal set; }

        public OrgChartNode() { }

        public OrgChartNode(string id, string name, params OrgChartNode[] content)
        {
            this.Id = id;
            this.Name = name;
            if(content!=null)
            {
                int order = 1;
                this.Children.AddRange(content);
                this.Children.ForEach(item =>
                {
                    item.ParentId = this.Id;
                    item.Parent = this;                    
                    item.Order = order;
                    item.Level = this.Level+1;
                    order++;
                });
            }            
        }

        public OrgChartNode PreviousSibling
        {
            get
            {
                if(this.Parent!=null)
                {
                    return this.Parent.Children.Where(item=>item.Order<this.Order).OrderByDescending(item=>item.Order).FirstOrDefault();
                }
                return null;
            }
        }

        public OrgChartNode FirstChild
        {
            get
            {
                return this.Children.OrderBy(item => item.Order).FirstOrDefault();
            }
        }

        public OrgChartNode LastChild
        {
            get
            {
                return this.Children.OrderBy(item => item.Order).LastOrDefault();
            }
        }

        public bool IsRoot
        {
            get
            {
                return this.Parent == null;
            }
        }
    }
}
