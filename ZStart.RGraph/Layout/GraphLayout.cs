using ZStart.RGraph.Common;
using ZStart.RGraph.DForce;
using ZStart.RGraph.View.Item;

namespace ZStart.RGraph.Layout
{
    public class GraphLayout:BaseLayout
    {
        private Simulation simulation;
       
        public GraphLayout()
        {
            simulation = new Simulation.Builder()
                .AddForce(ForceBody.NAME, new ForceBody().Strength(node => -5000).DistanceMin(1).DistanceMax(100))
                .AddForce(ForceCenter.NAME, new ForceCenter().SetX(0f).SetY(0f))
                //.AddForce(ForceRadial.NAME, new ForceRadial().Strength(node=> 1))
                .AddForce(ForceCollide.NAME, new ForceCollide().Radius(node => 2).Strength(10))
                .AddForce(ForceLink.NAME, new ForceLink().Distance(node=> 10))
                //.AddForce(ForceX.NAME, new ForceX().Strength(node=>0.1f))
                //.AddForce(ForceY.NAME, new ForceY().Strength(node => 0.1f))
                .Build();
        }

        public override void Begin()
        {
            for (int i = 0;i < allNodes.Count;i += 1)
            {
                simulation.AddNode(allNodes[i].Data);
            }
            for (int i = 0;i < allEdges.Count;i += 1)
            {
                simulation.AddLink(allEdges[i].Data);
            }
            simulation.Start();
        }

        public override void Stop()
        {
            simulation.Pause();
        }

        public override void Step()
        {
            simulation.Step();
            for (int i = 0;i < allNodes.Count;i += 1)
            {
                allNodes[i].Move();
            }
            UpdateLines();
        }

        public override void AddNode(RGNode node, string parent, bool add)
        {
            if(add)
                allNodes.Add(node);
        }

        public override void AddEdge(RGEdge edge, string parent)
        {
            allEdges.Add(edge);
        }
    }
}
