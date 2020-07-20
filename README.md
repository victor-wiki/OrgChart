# OrgChart
Draw organization chart, refer to https://www.codeproject.com/Articles/18378/Organization-Chart-Generator. In addition, it handles overloap
of nodes , and it can set auto height/width according to the content of a node.

# Sample
![Sample](https://github.com/jingwu1986/OrgChart/blob/master/Sample.png?raw=true)

# Usage
```
        static void Main(string[] args)
        {            
            OrgChartOption defaultOption = new OrgChartOption
            {
                BoxFillColor = ColorTranslator.FromHtml("#A7E7FC"),
                BoxBorderColor = ColorTranslator.FromHtml("#A7E7FC"),
                ConnectLineColor = ColorTranslator.FromHtml("#424242")
            };

            OrgChartOption option = new OrgChartOption()
            {
                BoxFillColor = defaultOption.BoxFillColor,
                BoxBorderColor = defaultOption.BoxBorderColor,
                ConnectLineColor = defaultOption.ConnectLineColor,
                FontSize = 9,
                HorizontalSpace = 10,
                VerticalSpace=20,               
                BoxHeight=45,
                BoxWidth = 110,
                //UseMinBoxWidthWhenHasOnlyOne = true,
                //MinBoxWidth = 80
            };       

            OrgChartGenerator orgChartGenerator = new OrgChartGenerator(GetOrgChartNodes(), option) { DefaultOption = defaultOption };

            string filePath = "org.png";
            using (FileStream fs = File.Create(filePath))
            {
                MemoryStream ms = orgChartGenerator.Generate();
                ms.WriteTo(fs);
                fs.Flush();
            } ;

            Process.Start(filePath);
        }

        private static List<OrgChartNode> GetOrgChartNodes()
        {
            List<OrgChartNode> nodes = new List<OrgChartNode>();

            nodes.Add(new OrgChartNode("0", "President", 
                new OrgChartNode("1.1", "Vice President Account Services",
                  new OrgChartNode("1.1.1", "Account Supervisor A",
                     new OrgChartNode("1.1.1.1", "Account Executive A"),
                     new OrgChartNode("1.1.1.2", "Account Executive B")
                  ),
                  new OrgChartNode("1.1.2", "Account Supervisor B")
                ),

                new OrgChartNode("1.2", "Vice President Creative Services",
                  new OrgChartNode("1.2.1", "Art/Copy"),
                  new OrgChartNode("1.2.2", "Production")
                ),

                new OrgChartNode("1.3", "Vice President Marketing Services",
                  new OrgChartNode("1.3.1", "Media"),
                  new OrgChartNode("1.3.2", "Resarch")
                ),

                new OrgChartNode("1.4", "Vice President Management Services",
                  new OrgChartNode("1.4.1", "Accounting"),
                  new OrgChartNode("1.4.2", "Perchasing"),
                  new OrgChartNode("1.4.3", "Personnel")
                )
            ));           

            return nodes;
        }
```
