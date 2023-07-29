using System;

namespace Experiments
{
    public class Experiment 
    {
        public int Id { set; get; }  
        public DateTime Time { set; get; }  
        public string? Name { set; get; }  
        public bool Selected { set; get; }  
    }
}
