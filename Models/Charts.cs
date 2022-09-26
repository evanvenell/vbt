using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.Serialization;
using System.Web;



namespace BugTrackerApplication.Models
{

    public class BarChart
    {
        public string[] labels { get; set; }

        public List<Datasets> datasets { get; set; }
    }

    public class DonutChart
    {
        public string[] labels { get; set; }
        public List<DonutDatasets> donutDatasets { get; set; }

    }

    //public class TicketTypeData
    //{
    //    public string label { get; set; }
    //    public string value { get; set; }
    //    public string color { get; set; }
    //    public string highlight { get; set; }
    //}

    public class Datasets
    {
        public string label { get; set; }
        public string[] backgroundColor { get; set; }
        public string[] borderColor { get; set; }
        public string borderWidth { get; set; }
        public int[] data { get; set; }
    }

    public class DonutDatasets
    {
        public string label { get; set; }
        public string[] borderAlign { get; set; }
        public string[] backgroundColor { get; set; }
        public string[] borderColor { get; set; }
        public string borderWidth { get; set; }
        public string[] hoverBackgroundColor { get; set; }
        public string[] hoverBorderColor { get; set; }
        public string hoverBoardWidth { get; set; }
        public string weight { get; set; }
        //public int[] data { get; set; }
        //public int data { get; set; }
        //public double[] data { get; set; }
        public int[] data { get; set; }
        //public string[] data { get; set; }
    }



}