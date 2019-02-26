using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgChart
{
    public class OrgChartHelper
    {
        public static OrgChartNode BuildTree(IEnumerable<OrgChartNode> nodes)
        {
            OrgChartNode rootNode = nodes.FirstOrDefault(item => string.IsNullOrEmpty(item.ParentId));

            if (rootNode != null)
            {
                if (rootNode.Children.Any())
                {
                    return rootNode;
                }

                rootNode.Level = 0;
                GetOrgChartModelChildren(rootNode, nodes);
            }

            return rootNode;
        }

        private static void GetOrgChartModelChildren(OrgChartNode parentNode, IEnumerable<OrgChartNode> models)
        {
            List<OrgChartNode> children = models.Where(item => item.ParentId == parentNode.Id).ToList();
            int i = 1;
            
            parentNode.Children.AddRange(children);

            children.ForEach(item =>
            {
                item.Order = i++;
                item.Parent = parentNode;
                item.Level = parentNode.Level + 1;

                GetOrgChartModelChildren(item, models);
            });
        }

        public static List<OrgChartNode> ConvertToNodes(OrgChartNode parentNode)
        {
            List<OrgChartNode> nodes = new List<OrgChartNode>();

            GetChildNodes(parentNode, nodes);

            return nodes;
        }

        private static List<OrgChartNode> GetChildNodes(OrgChartNode parentNode, List<OrgChartNode> nodes)
        {
            if(parentNode!=null && !nodes.Any(item=>item.Id==parentNode.Id))
            {
                nodes.Add(parentNode);
            }

            if(parentNode!=null && parentNode.Children.Any())
            {
                nodes.AddRange(parentNode.Children);
                parentNode.Children.ForEach(item=> 
                {
                    GetChildNodes(item, nodes);
                });               
            }
            return nodes;
        }

        public static Bitmap ResizeImage(Bitmap img, int width, int height, bool keepHeight = false)
        {
            int sourceWidth = img.Width;
            int sourceHeight = img.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = (width / (float)sourceWidth);
            nPercentH = (height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
            }
            else
            {
                nPercent = nPercentW;
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = keepHeight ? height : (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(img, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }
    }
}
