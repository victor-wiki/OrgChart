using System.Drawing;
using System.Drawing.Imaging;

namespace OrgChart
{
    public class OrgChartOption
    {
        public ImageFormat ImageFormat { get; set; } = ImageFormat.Png;

        /// <summary>
        /// Width of chart.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of height.
        /// </summary>
        public int Height { get; set; }       

        /// <summary>
        /// Margin of nodes.
        /// </summary>

        public int Margin { get; set; } = 20;

        /// <summary>
        /// Horizontal space of nodes.
        /// </summary>

        public int HorizontalSpace { get; set; } = 30;

        /// <summary>
        /// Vertical space of nodes.
        /// </summary>

        public int VerticalSpace { get; set; } = 30;

        /// <summary>
        /// Font name of chart.
        /// </summary>

        public string FontName { get; set; } = "Times New Roman";

        /// <summary>
        /// Font color of node.
        /// </summary>
        public Color FontColor { get; set; } = Color.Black;

        /// <summary>
        /// Font size of node.
        /// </summary>
        public int FontSize { get; set; } = 10;

        /// <summary>
        /// Connect line color of nodes.
        /// </summary>
        public Color ConnectLineColor { get; set; } = Color.Black;

        /// <summary>
        /// Connect line width of nodes.
        /// </summary>

        public float ConnectLineWidth { get; set; } = 1;

        /// <summary>
        /// Border color of node.
        /// </summary>

        public Color BoxBorderColor { get; set; } = Color.Black;

        /// <summary>
        /// Border width of node.
        /// </summary>
        public float BoxBorderWidth { get; set; } = 1;

        /// <summary>
        /// Fill color of node.
        /// </summary>
        public Color BoxFillColor { get; set; }

        /// <summary>
        /// Width of node.
        /// </summary>

        public int BoxWidth { get; set; } = 100;

        /// <summary>
        /// Height of node.
        /// </summary>
        public int BoxHeight { get; set; } = 35;

        /// <summary>
        /// Whether calculate height of node automatically according to its content.
        /// </summary>
        public bool AutoBoxHeight { get; set; }

        /// <summary>
        /// Whether calculate width of node automatically according to its content.
        /// </summary>
        public bool AutoBoxWidth { get; set; }

        /// <summary>
        /// Background color of chart.
        /// </summary>

        public Color BackgroundColor { get; set; } = Color.White;
      
        /// <summary>
        /// Whether resize node to avoid overlap between nodes.
        /// </summary>

        public bool ResizeToShrink { get; set; }

        /// <summary>
        /// Whether resize chart to fit the given container size.
        /// </summary>
        public bool ResizeToFill { get; set; }

        /// <summary>
        /// Minimum width of node.
        /// </summary>
        public int MinBoxWidth { get; set; }

        /// <summary>
        /// Whether apply MinBoxWidth when it has only one node at one level. 
        /// </summary>
        public bool UseMinBoxWidthWhenHasOnlyOne { get; set; }

        /// <summary>
        /// Whether use default option if it's a simple tree.
        /// </summary>
        public bool UseDefaultIfSampleTree { get; set; }

        /// <summary>
        /// When the count of nodes at one level is equal or less than, see it as a simple tree.
        /// </summary>
        public int SampeTreeLevelCount { get; set; } = 10;
    }
}
