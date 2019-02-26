/***
 参考：https://www.codeproject.com/Articles/18378/Organization-Chart-Generator
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace OrgChart
{
    public class OrgChartGenerator:IDisposable
    {
        public OrgChartOption DefaultOption { get; set; }=new OrgChartOption();

        private OrgChartOption option = new OrgChartOption();
        private int imgHeight = 0;
        private int imgWidth = 0;
        private int width = 0;
        private int height = 0;
        private Graphics gr;
        private OrgChartNode rootNode;
        private List<OrgChartNode> nodes = new List<OrgChartNode>();
        private List<OrgChartNode> offsetNodes = new List<OrgChartNode>();
        private bool isSampleTree = true;

        public List<OrgChartNode> Nodes { get { return this.nodes;  } }       

        public OrgChartGenerator(List<OrgChartNode> nodes, OrgChartOption option)
        {
            this.nodes = nodes;

            if(option!=null)
            {
                this.option = option;
            }           

            this.rootNode = OrgChartHelper.BuildTree(this.nodes);

            int maxLevelNodesCount = (from node in nodes
                                      group node by node.Level into gp
                                      select new { Level = gp.Key, Count = gp.Count() }
                                     ).Max(item => item.Count);

            this.isSampleTree = maxLevelNodesCount < this.option.SampeTreeLevelCount;

            if(this.option.UseDefaultIfSampleTree && this.isSampleTree)
            {
                this.option = this.DefaultOption;
            }
        }         

        public MemoryStream Generate()
        {
            if(this.rootNode==null)
            {
                throw new Exception("rootNode is null.");
            }

            width = this.option.Width;
            height = this.option.Height;

            MemoryStream result = new MemoryStream();
          
            this.CalculatePosition(rootNode, 0);         

            Bitmap bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format32bppRgb);
            gr = Graphics.FromImage(bmp);
            bmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.High;
            gr.Clear(this.option.BackgroundColor);

            this.DrawChart(rootNode);

            bool overranging = (width > 0 && imgWidth > width) || (height > 0 && imgHeight > height);

            Action calculateSize = () =>
            {
                //if caller does not care about size, use original calculated size
                if (width <= 0)
                {
                    width = imgWidth;
                }

                if (height <= 0)
                {
                    height = imgHeight;
                }
            };

            if(this.option.ResizeToShrink && overranging)
            {
                calculateSize();
                this.ResizeBmp(bmp, result, imgWidth> width? width: imgWidth , imgHeight> height? height: imgHeight);
            }
            else if(this.option.ResizeToFill)
            {
                calculateSize();
                this.ResizeBmp(bmp, result, width,  height);
            }
            else
            {
                bmp.Save(result, this.option.ImageFormat);
            }             

            bmp.Dispose();
            gr.Dispose();
            return result;
        }

        private void ResizeBmp(Bitmap bmp, MemoryStream result, int width, int height)
        {
            Bitmap resizedBmp = OrgChartHelper.ResizeImage(bmp, width, height, false);

            //after resize - change the coordinates of the list, in order return the proper coordinates
            resizedBmp.Save(result, this.option.ImageFormat);
            resizedBmp.Dispose();
        }

        private void CalculatePosition(OrgChartNode node, int y)
        {           
            foreach (OrgChartNode child in node.Children.OrderBy(item=>item.Order))
            {               
                this.CalculatePosition(child, y + 1);
            }

            int startX;
            int startY;
            int margin = this.option.Margin;
            int[] resultsArr = new int[] 
            {
                this.GetXPosByOwnChildren(node),
                this.GetXPosByParentPreviousSibling(node),
                this.GetXPosByPreviousSibling(node),
                margin
            };

            Array.Sort(resultsArr);

            startX = resultsArr[3];
            startY = (y * (this.option.BoxHeight + this.option.VerticalSpace)) + margin;
            int width = this.option.BoxWidth;
            int height = this.option.BoxHeight;

            //update the coordinates of this box into the matrix, for later calculations
            Rectangle drawRect = new Rectangle(startX, startY, width, height);

            //update the image size
            if (imgWidth < (startX + width + margin))
            {
                imgWidth = startX + width + margin;
            }
            if (imgHeight < (startY + height + margin))
            {
                imgHeight = startY + height + margin;
            }
            node.Position = drawRect;           
        }

        /************************************************************************************************************************
         * The box position is affected by:
         * 1. The previous sibling (box on the same level)
         * 2. The positions of it's children
         * 3. The position of it's uncle (parents' previous sibling)/ cousins (parents' previous sibling children)
         * What determines the position is the farthest x of all the above. If all/some of the above have no value, the margin 
         * becomes the dtermining factor.
         * **********************************************************************************************************************
        */
        private int GetXPosByPreviousSibling(OrgChartNode node)
        {
            int result = -1;
            OrgChartNode prevSibling = node.PreviousSibling;
            if (prevSibling != null)
            {
                if (prevSibling.Children.Any())
                {
                    result = prevSibling.LastChild.Position.X + this.option.BoxWidth + this.option.HorizontalSpace;
                }
                else
                {
                    result = prevSibling.Position.X + this.option.BoxWidth + this.option.HorizontalSpace;
                }
            }
            return result;
        }

        private int GetXPosByOwnChildren(OrgChartNode node)
        {
            int result = -1;
            if (node.Children.Any())
            {
                result = (((node.LastChild.Position.X + this.option.BoxWidth) -
                    node.FirstChild.Position.X) / 2) -
                    (this.option.BoxWidth / 2) +
                    node.FirstChild.Position.X;
            }
            return result;
        }
        private int GetXPosByParentPreviousSibling(OrgChartNode node)
        {
            int result = -1;
            OrgChartNode parentPrevSibling = node.Parent?.PreviousSibling;
            if (parentPrevSibling != null)
            {
                if (parentPrevSibling.Children.Any())
                {
                    result = parentPrevSibling.LastChild.Position.X + this.option.BoxWidth + this.option.HorizontalSpace;
                }
                else
                {
                    result = parentPrevSibling.Position.X + this.option.BoxWidth + this.option.HorizontalSpace;
                }
            }
            else //ParentPrevSibling == null
            {
                if (!node.IsRoot)
                {
                    result = GetXPosByParentPreviousSibling(node.Parent);
                }
            }
            return result;
        }
        
        /// <summary>
        /// Draws the actual chart image.
        /// </summary>
        private void DrawChart(OrgChartNode node)
        {
            // Create font and brush.
            Font drawFont = new Font(this.option.FontName, this.option.FontSize);
            SolidBrush drawBrush = new SolidBrush(this.option.FontColor);
            Pen boxPen= new Pen(this.option.BoxBorderColor, this.option.BoxBorderWidth);
            Pen connectLinePen = new Pen(this.option.ConnectLineColor, this.option.ConnectLineWidth);
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;

            foreach (OrgChartNode child in node.Children.OrderBy(item=>item.Order))
            {
                this.DrawChart(child);
            }

            //// Create string to draw.
            String drawString = node.Name;

            bool changed = false;
            int boxWidth = this.option.BoxWidth;

            Rectangle position = this.HandleSpeicalPosition(node, out changed);
            if(!changed)
            {
                #region auto height/width
                if (this.option.AutoBoxHeight || this.option.AutoBoxWidth)
                {
                    var height = (int)gr.MeasureString(drawString, drawFont, node.Position.Width).Height;
                    if (height > node.Position.Height)
                    {
                        if (this.option.AutoBoxHeight)
                        {
                            node.Position = new Rectangle(node.Position.X, node.Position.Y, node.Position.Width, height);
                        }
                        else if (this.option.AutoBoxWidth)
                        {
                            int increment = 5;
                            int count = node.Position.Width / increment;
                            for (int i = 1; i <= count; i++)
                            {
                                int width = node.Position.Width + i * increment;
                                height = (int)gr.MeasureString(drawString, drawFont, width).Height;
                                if (height <= node.Position.Height)
                                {                                    
                                    node.Position = new Rectangle(node.Position.X+ (int)((node.Position.Width- width)/2), node.Position.Y, width, node.Position.Height);
                                    boxWidth = node.Position.Width;
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                boxWidth = position.Width;
            }           

            var overlapNodes = this.nodes.Where(item => node.Level == item.Level && (node.Position.X> item.Position.X && node.Position.X < item.Position.X + item.Position.Width));
            if (overlapNodes.Count()>0) //overlap
            {
                OrgChartNode overlapNode = overlapNodes.First();
                int overlapWidth = overlapNode.Position.X + overlapNode.Position.Width - node.Position.X;
                int newX = overlapNode.Position.X + overlapNode.Position.Width + this.option.HorizontalSpace;
                node.OffsetX = newX - node.Position.X;

                node.Position = new Rectangle(newX, node.Position.Y, node.Position.Width, node.Position.Height);

                offsetNodes.Add(node);
            }
            else
            {               
                if(offsetNodes.Any(item=>item.ParentId==node.ParentId))
                {
                    int offsetX = offsetNodes.FirstOrDefault(item => item.ParentId == node.ParentId).OffsetX;
                    node.Position = new Rectangle(node.Position.X+ offsetX, node.Position.Y, node.Position.Width, node.Position.Height);
                }
            }

            gr.DrawRectangle(boxPen, node.Position);
            gr.FillRectangle(new SolidBrush(this.option.BoxFillColor), node.Position);                  

            //String drawString = EmployeeChartData[OrgCharMatrix[x, y]].Position.X.ToString();
            // Draw string to screen.
            gr.DrawString(drawString, drawFont, drawBrush, node.Position, drawFormat);

            //draw connecting lines
            if (!node.IsRoot)
            {
                //all but the top box should have lines growing out of their top
                gr.DrawLine(connectLinePen, node.Position.Left + (boxWidth / 2),
                                            node.Position.Top,
                                            node.Position.Left + (boxWidth / 2),
                                            node.Position.Top - (this.option.VerticalSpace / 2));
            }
            if (node.Children.Any())
            {
                //all nodes which have nodes should have lines coming from bottom down
                gr.DrawLine(connectLinePen, node.Position.Left + (boxWidth / 2),
                                    node.Position.Top + this.option.BoxHeight,
                                    node.Position.Left + (boxWidth / 2),
                                    node.Position.Top + this.option.BoxHeight + (this.option.VerticalSpace / 2));

            }
            if (node.PreviousSibling != null)
            {
                //the prev node has the same parent - connect the 2 nodes
                gr.DrawLine(connectLinePen, node.PreviousSibling.Position.Left + (this.option.BoxWidth / 2) - (this.option.ConnectLineWidth / 2),
                                    node.PreviousSibling.Position.Top - (this.option.VerticalSpace / 2),
                                    node.Position.Left + (this.option.BoxWidth / 2) + (this.option.ConnectLineWidth / 2),
                                    node.Position.Top - (this.option.VerticalSpace / 2));
            }
        }

        private Rectangle HandleSpeicalPosition(OrgChartNode node, out bool changed)
        {
            changed = false;
            int level = node.Level;
            bool isOnlyOne = nodes.Where(item => item.Level == level).Count() == 1;
            if (!this.isSampleTree && isOnlyOne && this.option.UseMinBoxWidthWhenHasOnlyOne && this.option.MinBoxWidth > 0)
            {
                node.Position = new Rectangle(node.Position.X + (this.option.BoxWidth - this.option.MinBoxWidth), node.Position.Y, this.option.MinBoxWidth, node.Position.Height);
                changed = true;
            }
            return node.Position;
        }

        public void Dispose()
        {
            if(this.gr!=null)
            {
                gr.Dispose();
            }
        }
    }
}
