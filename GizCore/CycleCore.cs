using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizCore
{
    public class CycleCore
    {
        public List<EdgeCore> Items { get; set; }
        private bool IsComplete;
        internal void Build(EdgeCore item)
        {
            if (!IsComplete)
            {
                this.IsComplete = item.First == Items[0].Second;
                this.Items.Add(item);
            }
        }
        public CycleCore(EdgeCore firstMember)
        {
            this.Items = new List<EdgeCore>();
            this.Items.Add(firstMember);
        }
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (var member in this.Items)
            {
                result.Append($"{member.First.Id}");
                if (member != Items[Items.Count - 1])
                    result.Append(", ");
            }
            return result.ToString();
        }
    }
}
